
using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.HttpCommunication;
using NodeNet.NodeNet.Message;
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

        public static Node CreateRSAHttpNode( SenderSignOptions options )
        {

            IMessageSigner messageSigner = new RSASigner.MessageSigner();
            IMessageValidator messageValidator = new RSASigner.MessageValidator();
            messageSigner.SetSignOptions(options);

            Node node = new Node();
            node.SignOptions = options;
            node.MessageValidator = messageValidator;
            node.MessageSigner = messageSigner;
            node.Connections = new HttpConnections();
            node.ConnectionsListener = new NodeHttpListener();

            node.ConnectionsListener.StartListening();
            node.ConnectionsListener.ConnectionOpened += (sender, connection) => { node.Connections.AddConnection(connection as INodeConnection); };
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
    }
}
