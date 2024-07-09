using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.NodeActions;
using System.Net;
using System.Net.Sockets;

namespace NodeNet.NodeNet.TcpCommunication
{
    public class NodeTcpListener : INodeListener, ITcpAddressProvider
    {
        public bool IsListening { get; set; } = false;
        public int ListenPort { get; set; } = 8080;
        protected Task? ListeningTask { get; set; } = null;
        public TcpListener TcpListener { get; protected set; }
        public TcpListenerOptions Options { get; set; } = new TcpListenerOptions(8080);

        public event Action<INodeConnection> ConnectionOpened;
        protected CancellationTokenSource cancellationTokenSource { get; set; }

        public void StartListening(string? tellablePublicKey = null)
        {
            if (IsListening == true)
                throw new Exception("Multiple listening");
            ListenPort = Options.Port;
            TcpListener = new TcpListener(Options.Port);
            TcpListener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            ListeningTask = Task.Run(() => Listener(tellablePublicKey));
            IsListening = true;
        }

        public void StartListening()
        {
            StartListening(null);
        }

        public void StopListening()
        {
            if (ListeningTask == null)
                throw new Exception("Is not listening");
            IsListening = false;
            cancellationTokenSource.Cancel();
            TcpListener.Stop();
            ListeningTask.Wait();
        }

        protected async Task Listener(string? tellablePublicKey = null)
        {
            try
            {
                while (IsListening == true)
                {
                    var tcpConnection = await TcpListener.AcceptTcpClientAsync(cancellationTokenSource.Token);
                    var connection = new NodeTcpConnection(tcpConnection);
                    connection.TcpAddressProvider = this;

                    PingPong.Pong(connection, tellablePublicKey).ContinueWith((result) =>
                    {
                        if (result.Result)
                            ConnectionOpened?.Invoke(connection);
                    });
                }
            }
            catch (OperationCanceledException cancelException)
            {
                IsListening = false;
            }
        }

        string INodeListener.GetConnectionAddress()
        {
            throw new NotImplementedException();
        }

        public int GetNodeTcpPort()
        {
            return ((IPEndPoint)TcpListener.LocalEndpoint).Port;
        }

        public string GetNodeTcpIP()
        {
            throw new NotImplementedException();
        }
    }
}
