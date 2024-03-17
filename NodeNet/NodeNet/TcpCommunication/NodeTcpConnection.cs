using Newtonsoft.Json;
using NodeNet.NodeNet.Communication;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using NodeNet.NodeNet.JsonStreamParser;

namespace NodeNet.NodeNet.TcpCommunication
{
    public class NodeTcpConnection : INodeConnection
    {
        public bool IsListening { get; set; } = false;
        public TcpClient TcpClient { get; protected set; }
        public ITcpAddressProvider TcpAddressProvider { get; set; }
        public event Action<INodeConnection> MessageReceived;
        public event Action<INodeConnection> ConnectionClosed;
        protected Thread ListeningThread = null;
        protected JsonStreamParser.JsonStreamParser jsonStreamParser = new JsonStreamParser.JsonStreamParser();
        protected Queue<NodeNet.Message.Message> messagesQueue = new Queue<NodeNet.Message.Message>();


        public NodeTcpConnection()
        {
            TcpClient = new TcpClient();
        }

        public NodeTcpConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public bool Connect(string addr)
        {
            string ip = addr.Split(":")[0];
            string port = addr.Split(":")[1];
            try
            {
                jsonStreamParser = new JsonStreamParser.JsonStreamParser();
                TcpClient.Connect(ip, Convert.ToInt32(port));
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public NodeNet.Message.Message? GetLastMessage()
        {
            lock (this)
                return messagesQueue.Count != 0 ? messagesQueue.Dequeue() : null;
        }

        public List<NodeNet.Message.Message> GetMessageList()
        {
            var messageList = messagesQueue.ToList();
            messagesQueue.Clear();
            return messageList;
        }

        public void ListenMessages()
        {
            IsListening = true;
            ListeningThread = new Thread(() => MessageListener());
            ListeningThread.Start();
        }

        public async Task SendMessage(NodeNet.Message.Message message)
        {
            // Serialization can be performed in parallel, so lock is not needed here.
            var jsonMessage = JsonConvert.SerializeObject(message);
            var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage));
            var stream = TcpClient.GetStream();

            // Only one execution thread can write to a data stream at a time, otherwise it is impossible to interpret the data correctly.
            await stream.WriteAsync(segment);
        }

        public async Task SendRawData(byte[] data, CancellationToken cancellationToken)
        {
            await TcpClient.GetStream().WriteAsync(data, cancellationToken);
        }

        public async Task<byte[]> ReceiveRawData(CancellationToken cancellationToken)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 16]);
            try
            {
                var size = await TcpClient.GetStream().ReadAsync(buffer, cancellationToken);
                return buffer.Array.Take(size).ToArray();
            }
            catch (Exception ex)
            {
                throw new WebSocketException("Socket closed");
            }
        }

        public void CloseConnection()
        {
            if (TcpClient == null)
                throw new Exception("Socket is not connected");
            if (IsListening)
                ConnectionClosed?.Invoke(this);
            IsListening = false;

        }

        protected async Task MessageListener()
        {
            // TODO: verify received part is correct json document, receive until correct json will be received
            var inputStream = TcpClient.GetStream();
            try
            {
                while (IsListening)
                {
                    var parsedObject = await jsonStreamParser.ParseJsonObject(inputStream, CancellationToken.None);
                    var messageObject = parsedObject.Deserialize<NodeNet.Message.Message>();
                    if (messageObject is NodeNet.Message.Message message)
                        AddMessageToQueue(message);
                }
            }
            catch (Exception ex)
            {
            }
            CloseConnection();
        }

        protected void AddMessageToQueue(NodeNet.Message.Message message)
        {
            messagesQueue.Enqueue(message);
            MessageReceived?.Invoke(this);
        }

        public string GetConnectionAddress()
        {
            if (TcpClient == null)
                throw new Exception("Connections is not initialized");
            if (TcpClient.Connected == false)
                throw new Exception("Connection is not active");

            string clientAddress = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Address.MapToIPv4().ToString();
            int clientPort = TcpAddressProvider.GetNodeTcpPort();
            return clientAddress + ":" + clientPort;
        }
    }
}
