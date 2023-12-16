
using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.ReceiveMiddleware;
using NodeNet.NodeNet.RSASigner;
using NodeNet.NodeNet.SignOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet
{
    internal class Node
    {
        public IMessageSigner? MessageSigner { get; protected set; } = null;
        public IMessageValidator? MessageValidator { get; protected set; } = null;
        public INodeListener? ConnectionsListener { get; protected set; } = null;
        public INodeConnections? Connections { get; protected set; } = null;
        public ISenderSignOptions? SignOptions { get; protected set; } = null;
        public IReceiveMiddleware ReceiveMiddlewareHead { get; protected set; }

        public event Action<MessageContext> MessageReceived;

        public static Node CreateRSAHttpNode( SenderSignOptions options, HttpListenerOptions listenerOptions )
        {

            IMessageSigner messageSigner = new RSASigner.MessageSigner();
            IMessageValidator messageValidator = new RSASigner.MessageValidator();
            messageSigner.SetSignOptions(options);

            Node node = new Node();
            node.SignOptions = options;
            node.MessageValidator = messageValidator;
            node.MessageSigner = messageSigner;
            node.Connections = new HttpConnections();

            // Middleware pipeline
            var signMiddleware = new SignVerificationMiddleware(node, messageValidator);
            var cacheMiddleware = new MessageCacheMiddleware();
            signMiddleware.SetNext(cacheMiddleware);

            node.ReceiveMiddlewareHead = signMiddleware;
            // TODO: add another middlewares in pipeline

            var listener = new NodeHttpListener();
            listener.Options = listenerOptions;
            node.ConnectionsListener = listener;
            node.ConnectionsListener.ConnectionOpened += node.NewConnectionHandler;
            node.ConnectionsListener.StartListening();

            return node;
        }

        public void SendMessage(string messageContent, string receiver = null )
        {
            if (MessageSigner == null || SignOptions == null || Connections == null)
                throw new Exception("Node is not initialized!");

            if( receiver == null )
                receiver = string.Empty;
            var connections = Connections.Connections();
            var messageInfo = new MessageInfo(SignOptions.PublicKey, receiver);
            var message = new Message.Message(messageInfo, messageContent);
            MessageSigner.Sign(message);
            foreach (var connection in connections)
                connection.SendMessage(message);
        }

        public bool Connect(string url)
        {
            NodeHttpConnection connection = new NodeHttpConnection();
            bool result = connection.Connect(url);
            if (result == true)
                NewConnectionHandler(connection);
            // TODO: Verify connection enstabilished on NodeNet level
            // Temporary solution, waiting some time, until server make actions
            Thread.Sleep(500); 
            return result;
        }

        public void Close()
        {
            if (ConnectionsListener == null || Connections == null)
                throw new Exception("Node is not initialized!");

            ConnectionsListener.StopListening();
            foreach (var connection in Connections.Connections())
                connection.CloseConnection();
        }

        protected void NewConnectionHandler(INodeConnection nodeConnection)
        {
            nodeConnection.WebSocketClosed += Connections.RemoveConnection;
            nodeConnection.MessageReceived += NewMessageHandler;
            this.Connections.AddConnection(nodeConnection);
        }

        protected void NewMessageHandler(INodeConnection nodeConnection)
        {
            // TODO: Analyze package sign, receiver addr, TTL, package cache
            var message = nodeConnection.GetLastMessage();
            if (message == null)
                return;
            var msgContext = new MessageContext(message, nodeConnection);
            var msgPassMiddleware = ReceiveMiddlewareHead.Invoke(msgContext);
            if (msgPassMiddleware)
                MessageReceived?.Invoke(msgContext);
        }
    }
}
