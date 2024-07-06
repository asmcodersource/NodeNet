using NodeNet.NodeNet.RSAEncryptions;
using NodeNet.NodeNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Generators
{
    public class NodeNetConnectionPair : IDisposable
    {
        public static int PortCounter { get; protected set; } = 2000;
        public Node first_node { get; protected set; }
        public Node second_node { get; protected set; }
        public bool IsConnectionSuccess { get; protected set; }

        public NodeNetConnectionPair()
        {
            first_node = Node.CreateRSAHttpNode(
                RSAEncryption.CreateSignOptions(),
                new NodeNet.NodeNet.TcpCommunication.TcpListenerOptions(PortCounter++)
            );
            second_node = Node.CreateRSAHttpNode(
                RSAEncryption.CreateSignOptions(),
                new NodeNet.NodeNet.TcpCommunication.TcpListenerOptions(PortCounter++)
            );
            IsConnectionSuccess = first_node.Connect($"127.0.0.1:{PortCounter - 1}");
        }

        public void Dispose()
        {
            first_node.Close();
            second_node.Close();
        }
    }
}
