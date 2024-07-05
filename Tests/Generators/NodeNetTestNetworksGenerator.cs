using NodeNet.NodeNet;

namespace Tests.Generators
{
    public record ConnectionPair
    {
        Node node1;
        Node node2;
        string info;

        public ConnectionPair(Node first, Node second, string info)
        {
            node1 = first;
            node2 = second;
            this.info = info;
        }
    }

    /// <summary>
    /// Utility for creating network of connections between NodeNet peers
    /// for testing main features of NodeNet system
    /// </summary>
    public class NodeNetTestNetworksGenerator : IDisposable
    {
        static public NodeNetTestNetworksGenerator Shared { get; protected set; }
        static protected int portIterator = 6000; // used to give uniq socket port for every nodenet peer
        public List<Node>? Nodes { get; protected set; } = null;
        public List<ConnectionPair> ConnectionsList = new List<ConnectionPair>();


        static NodeNetTestNetworksGenerator()
        {
            // Create for test performing
            NodeNetTestNetworksGenerator nodeNetNetworkConnections = new NodeNetTestNetworksGenerator();
            nodeNetNetworkConnections.CreateNetworkPeers(20);
            nodeNetNetworkConnections.CreateNetworkTree(4);
            nodeNetNetworkConnections.PerformRandomConnections(0);
            Shared = nodeNetNetworkConnections;
        }

        public void CreateNetworkPeers(int peersCount)
        {
            if (Nodes is not null)
                throw new Exception("Network already initialized");
            if (peersCount == 0)
                throw new Exception("Are you sure about that? peersCount is 0");

            Nodes = new List<Node>();
            for (int i = 0; i < peersCount; i++)
            {
                var peer = Node.CreateRSAHttpNode(
                    NodeNet.NodeNet.RSAEncryptions.RSAEncryption.CreateSignOptions(),
                    new NodeNet.NodeNet.TcpCommunication.TcpListenerOptions(portIterator)
                );
                Nodes.Add(peer);
                portIterator++;
            }
        }

        /// <summary>
        /// Creates tree of network connections between NodeNet peers
        /// </summary>
        public void CreateNetworkTree(int maxConnectionsPerNode)
        {
            if (Nodes is null)
                throw new Exception("Network is not initialized");

            int firstPeerOffset = 0;
            for (int layer_i = 0; firstPeerOffset < Nodes.Count(); layer_i++)
            {
                int peersOnLayer = (int)Math.Pow(maxConnectionsPerNode, layer_i);
                for (int firstPeerId = firstPeerOffset; firstPeerId < peersOnLayer + firstPeerOffset; firstPeerId++)
                {
                    if (firstPeerId >= Nodes.Count())
                        break;
                    var firstPeer = Nodes[firstPeerId];
                    var layerLocalId = firstPeerId - firstPeerOffset;
                    var secondPeerOffset = firstPeerOffset + peersOnLayer + layerLocalId * maxConnectionsPerNode;
                    for (int secondPeerId = secondPeerOffset; secondPeerId < maxConnectionsPerNode + secondPeerOffset; secondPeerId++)
                    {
                        if (secondPeerId >= Nodes.Count())
                            break;
                        var secondPeer = Nodes[secondPeerId];
                        var success = firstPeer.Connect($"127.0.0.1:{secondPeer.GetNodeTcpPort()}");
                        Assert.True(success);
                        ConnectionsList.Add(new ConnectionPair(firstPeer, secondPeer, $"{firstPeerId} - {secondPeerId}"));
                    }
                }
                firstPeerOffset = firstPeerOffset + peersOnLayer;
            }
        }

        public void PerformRandomConnections(int randomConnectionsCount)
        {
            for (int i = 0; i < randomConnectionsCount; i++)
            {
                var firstPeer = Nodes[Random.Shared.Next(Nodes.Count())];
                var secondPeer = Nodes[Random.Shared.Next(Nodes.Count())];
                if (secondPeer == firstPeer)
                {
                    i--;
                    continue;
                }
                firstPeer.Connect($"127.0.0.1:{secondPeer.GetNodeTcpPort()}");
            }
        }

        public Node GetRandomNode()
        {
            return Nodes[Random.Shared.Next(Nodes.Count())];
        }

        public void Dispose()
        {
            foreach (Node node in Nodes)
                node.Close();
        }
    }
}
