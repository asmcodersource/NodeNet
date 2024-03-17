using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.NetworkExplorer.Requests;
using Newtonsoft.Json;
using NodeNet.NodeNet;
using NodeNet.NodeNet.Communication;

namespace NodeNet.NodeNet.NetworkExplorer
{
    /// <summary>
    /// Provide methods of network explore requests
    /// Used to store and remember nodes address
    /// </summary>
    public class NetworkExplorer
    {
        public Node Node { get; protected set; }
        public List<RecentNodeConnection> recentNodeConnections { get; protected set; } = new List<RecentNodeConnection>();
        string filePath = string.Empty;

        public NetworkExplorerMiddleware Middleware { get; set; }

        public NetworkExplorer(Node node, string filePath = "explorer.dat")
        {
            Middleware = new NetworkExplorerMiddleware(node, this);
            this.filePath = filePath;
            Node = node;
        }

        //
        // : change this to something legal
        ~NetworkExplorer()
        {
            SaveRecentConnectionsToFile(filePath);
        }

        public void SendExploreEcho()
        {
            if (Node.MessageSigner == null || Node.SignOptions == null || Node.Connections == null)
                throw new Exception("Node is not initialized!");

            var connections = Node.Connections.Connections();
            foreach (var connection in connections)
            {
                var echoRequest = new EchoRequest();
                echoRequest.MyAddress = connection.GetConnectionAddress();
                string jsonObject = JsonConvert.SerializeObject(echoRequest);
                var messageInfo = new MessageInfo(Node.SignOptions.PublicKey, string.Empty, true);
                var message = new Message.Message(messageInfo, jsonObject);
                Node.MessageSigner.Sign(message);
                connection.SendMessage(message);
            }
        }

        public void UpdateConnectionInfo(INodeConnection nodeConnection)
        {
            UpdateConnectionInfo(nodeConnection.GetConnectionAddress());
        }

        public void UpdateConnectionInfo(string address)
        {
            var recentNodeConnection = RecentNodeConnection.CreateFromNodeConnection(address);
            int index = FindByAddress(recentNodeConnection.Address);
            // its new connection, just save
            if (index == -1)
            {
                recentNodeConnections.Add(recentNodeConnection);
                return;
            }
            // its know connection, update some info
            recentNodeConnections[index].Address = recentNodeConnection.Address;
            recentNodeConnections[index].LastOnlineTime = recentNodeConnection.LastOnlineTime;
        }

        protected int FindByAddress(string address)
        {
            // TODO: invent something fast for this \ Maybe another dictionary with iterator coordinates
            var recentConnections = recentNodeConnections.Where(x => x.Address == address);
            if (recentConnections.Count() == 0)
                return -1;
            int index = recentNodeConnections.IndexOf(recentConnections.First());
            return index;
        }

        public void LoadRecentConnectionsFromFile(string path)
        {
            try
            {
                if (File.Exists(path) == false)
                    return;
                var stream = File.OpenRead(path);
                var file = new StreamReader(stream);
                var savedNodeConnections = JsonConvert.DeserializeObject<List<RecentNodeConnection>>(file.ReadToEnd());
                file.Close();
                recentNodeConnections.AddRange(savedNodeConnections);
            }
            catch (Exception ex)
            {
                throw new Exception("Something wen't wrong with reading connections from file");
            }
        }

        public void SaveRecentConnectionsToFile(string path)
        {
            try
            {
                var serrializedList = JsonConvert.SerializeObject(recentNodeConnections, Formatting.Indented);
                var stream = File.OpenWrite(path);
                var file = new StreamWriter(stream);
                file.Write(serrializedList);
                file.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Something wen't wrong with writting connections to file");
            }
        }
    }
}
