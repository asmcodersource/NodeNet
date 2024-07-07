using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet;

namespace NodeNet.NodeNetSession.SessionListener
{
    public class SessionListener : IDisposable
    {
        public event Action<Session.Session> NewSessionCreated;
        public string Resource { get; protected set; }

        private CancellationTokenSource? listeningCancellationTokenSource;
        private readonly SessionHandshakeHandler handshakeHandler;
        private readonly Node listeningNode;
        private Task listeningTask;

        public SessionListener( Node node, string resource ) 
        {
            listeningNode = node;
            handshakeHandler = new SessionHandshakeHandler(node, resource);
            Resource = resource;
        }

        public void Dispose()
        {
            if (listeningCancellationTokenSource is null)
                return;
            StopListening();
            listeningTask.Wait();
        }

        public void StartListening()
        {
            lock (this)
            {
                if (listeningCancellationTokenSource is not null)
                    throw new InvalidOperationException("Session is already listening");
                listeningCancellationTokenSource = new CancellationTokenSource();
                handshakeHandler.StartMessageListening();
                listeningTask = Task.Run(() => SessionListeningTask());
            }
        }

        public void StopListening()
        {
            lock (this)
            {
                if (listeningCancellationTokenSource is null)
                    throw new InvalidOperationException("Session wasn't listening");
                listeningCancellationTokenSource.Cancel();
                handshakeHandler.StopMessageListening();
                listeningCancellationTokenSource = null;
            }
        }

        public async Task SessionListeningTask()
        {
            if (listeningCancellationTokenSource is null)
                throw new NullReferenceException("cancellation token is has null value");
            var cancellationToken = listeningCancellationTokenSource.Token;
            while(cancellationToken.IsCancellationRequested is not true)
            {
                try
                {
                    var session = await handshakeHandler.HandleNextRequest(cancellationToken);
                    NewSessionCreated?.Invoke(session);
                } catch { }
            }
        }
    }
}
