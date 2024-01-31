using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.NodeActions;

namespace NodeNet.NodeNet.TcpCommunication
{
    internal class NodeTcpListener : INodeListener
    {
        public bool IsListening { get; set; } = false;
        public int ListenPort { get; set; } = 8080;
        protected Task? ListeningTask { get; set; } = null;
        public TcpListener TcpListener { get; protected set; }
        public TcpListenerOptions Options { get; set; } = new TcpListenerOptions(8080);

        public event Action<INodeConnection> ConnectionOpened;
        protected CancellationTokenSource cancellationTokenSource { get; set; }

        public void StartListening()
        {
            if (IsListening == true)
                throw new Exception("Multiple listening");
            TcpListener = new TcpListener(Options.Port);
            TcpListener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            ListeningTask = Task.Run(() => Listener());
            IsListening = true;
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

        protected async Task Listener()
        {
            try
            {
                while (IsListening == true)
                {
                    var tcpConnection = await TcpListener.AcceptTcpClientAsync(cancellationTokenSource.Token);
                    var connection = new NodeTcpConnection(tcpConnection);

                    PingPong.Pong(connection).ContinueWith((result) => {
                        if (result.Result)
                            ConnectionOpened?.Invoke(connection);
                    });
                }
            } catch (OperationCanceledException cancelException) { 
                IsListening = false;
            }
        }

        string INodeListener.GetConnectionAddress()
        {
            throw new NotImplementedException();
        }
    }
}
