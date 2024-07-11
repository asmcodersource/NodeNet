using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.NodeActions;
using NodeNet.NodeNet.ReceiveMiddleware;
using NodeNet.NodeNet.TcpCommunication;
using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNet.SignOptions;
using System;

namespace NodeNet.NodeNet
{
    public class Node : ITcpAddressProvider
    {
        public bool AutoRepeater { get; protected set; } = true;
        public IMessageSigner? RsaMessageSigner { get; protected set; } = null;
        public IMessageValidator? MessageValidator { get; protected set; } = null;
        public INodeListener? ConnectionsListener { get; protected set; } = null;
        public INodeConnections? Connections { get; protected set; } = null;
        public ISenderSignOptions? SignOptions { get; protected set; } = null;
        public MiddlewarePipeline MiddlewarePipeline { get; protected set; }
        public NetworkExplorer.NetworkExplorer NetworkExplorer { get; protected set; }

        public event Action<MessageContext> InvalidMessageReceived;
        public event Action<MessageContext> MessageReceived;
        public event Action<MessageContext> PersonalMessageReceived;

        public static Node CreateRSAHttpNode(SenderSignOptions options, TcpListenerOptions listenerOptions)
        {

            IMessageSigner RsaMessageSigner = new RsaMessageSigner();
            IMessageValidator messageValidator = new RsaMessageValidator();
            RsaMessageSigner.SetSignOptions(options);

            Node node = new Node();
            node.SignOptions = options;
            node.MessageValidator = messageValidator;
            node.RsaMessageSigner = RsaMessageSigner;
            node.Connections = new TcpConnections();
            node.NetworkExplorer = new NetworkExplorer.NetworkExplorer(node);
            node.DefaultPipelineInitialize();

            var listener = new NodeTcpListener();
            listener.Options = listenerOptions;
            node.ConnectionsListener = listener;
            node.ConnectionsListener.ConnectionOpened += node.NewConnectionHandler;
            node.ConnectionsListener.StartListening(options);

            Serilog.Log.Verbose($"NodeNet node localhost:{node.GetNodeTcpPort()} | Started");
            return node;
        }

        public void SendMessage(string messageContent, INodeConnection connection, string? receiver = null,  bool isTechnical = false)
        {
            if (RsaMessageSigner == null || SignOptions == null || Connections == null)
                throw new Exception("Node is not initialized!");

            if (receiver == null)
                receiver = string.Empty;
            var messageInfo = new MessageInfo(SignOptions.PublicKey, receiver, isTechnical);
            var message = new Message.Message(messageInfo, messageContent);
            RsaMessageSigner.Sign(message);

            connection.SendMessage(message);
        }

        public void SendMessage(string messageContent, string receiver = null, bool isTechnical = false)
        {
            if (RsaMessageSigner == null || SignOptions == null || Connections == null)
                throw new Exception("Node is not initialized!");

            if (receiver == null)
                receiver = string.Empty;
            var messageInfo = new MessageInfo(SignOptions.PublicKey, receiver, isTechnical);
            var message = new Message.Message(messageInfo, messageContent);
            RsaMessageSigner.Sign(message);

            var connections = Connections.Connections();
            Task.Run(async () =>
            {
                List<Task> tasks = new List<Task>();
                foreach (var connection in connections)
                    tasks.Add( Task.Run( () => connection.SendMessage(message) ) );
                await Task.WhenAll(tasks);
            }).Wait();
        }

        public bool Connect(string url)
        {
            NodeTcpConnection connection = new NodeTcpConnection();
            connection.TcpAddressProvider = this;
            bool result = connection.Connect(url);
            if (result == false)
                return false;
            var pingTask = PingPong.Ping(connection, SignOptions);
            pingTask.Wait();
            if (pingTask.Result)
            {
                Serilog.Log.Verbose($"NodeNet node locahost:{GetNodeTcpPort()} | Succesfully connected to the {url}");
                NewConnectionHandler(connection);
                return true;
            }
            return false;
        }

        public void Close()
        {
            if (ConnectionsListener == null || Connections == null)
                throw new Exception("Node is not initialized!");

            ConnectionsListener.StopListening();
            foreach (var connection in Connections.Connections())
                connection.CloseConnection();
        }

        public void DefaultPipelineInitialize()
        {
            // Create pipeline handlers
            var signMiddleware = new SignVerificationMiddleware(this, MessageValidator);
            var cacheMiddleware = new MessageCacheMiddleware();
            var floodProtectorMiddleware = new FloodProtectorMiddleware();
            var successTerminator = new SuccessTerminator();
            // add them to pipeline
            MiddlewarePipeline = new MiddlewarePipeline();
            MiddlewarePipeline.AddHandler(signMiddleware);
            MiddlewarePipeline.AddHandler(cacheMiddleware);
            MiddlewarePipeline.AddHandler(floodProtectorMiddleware);
            MiddlewarePipeline.AddHandler(NetworkExplorer.Middleware);
            MiddlewarePipeline.AddHandler(successTerminator);
        }

        protected void NewConnectionHandler(INodeConnection nodeConnection)
        {
            Connections.AddConnection(nodeConnection);
            nodeConnection.ConnectionClosed += Connections.RemoveConnection;
            nodeConnection.MessageReceived += NewMessageHandler;
            nodeConnection.ListenMessages();
            NetworkExplorer.UpdateConnectionInfo(nodeConnection);
        }

        protected async void NewMessageHandler(INodeConnection nodeConnection)
        {
            var thread = new Thread(() =>
            {
                var message = nodeConnection.GetLastMessage();
                if (message == null)
                    return;
                var msgContext = new MessageContext(message, nodeConnection);
                if (msgContext.Message.Info.SenderPublicKey == SignOptions.PublicKey)
                    return;
                var msgPassMiddleware = MiddlewarePipeline.Handle(msgContext);
                if (msgPassMiddleware)
                {
                    Serilog.Log.Verbose($"NodeNet node localhost:{GetNodeTcpPort()} | Message received");
                    MessageReceived?.Invoke(msgContext);
                    if (msgContext.Message.Info.ReceiverPublicKey == SignOptions.PublicKey)
                    {
                        PersonalMessageReceived?.Invoke(msgContext);
                        Serilog.Log.Debug($"NodeNet node localhost:{GetNodeTcpPort()} | Personal message received");
                    }
                    if (AutoRepeater is true)
                    {
                        var connections = Connections.Connections();
                        foreach (var connection in connections)
                            connection.SendMessage(message);
                    }
                }
                else
                {
                    InvalidMessageReceived?.Invoke(msgContext);
                }
            });
            thread.Start();
        }

        public ICollection<INodeConnection>? GetNodeConnections()
        {
            return Connections?.Connections();
        }

        public int GetNodeTcpPort()
        {
            if (ConnectionsListener is NodeTcpListener listener)
                return listener.GetNodeTcpPort();
            throw new Exception("Um... Did I invent multiple listeners?");
        }

        public string GetNodeTcpIP()
        {
            throw new NotImplementedException();
        }
    }
}
