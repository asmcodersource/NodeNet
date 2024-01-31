using Newtonsoft.Json;
using NodeNet.NodeNet.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.NetworkExplorer
{
    /// <summary>
    /// Provide methods of network explore requests
    /// Used to store and remember nodes address
    /// </summary>
    internal class NetworkExplorer
    {
        List<RecentNodeConnection> recentNodeConnections = new List<RecentNodeConnection>();
        public NetworkExplorerMiddleware Middleware { get; set; }

        public NetworkExplorer(Node node)
        {
            Middleware = new NetworkExplorerMiddleware(node, this);
        }

        public void UpdateConnectionInfo( INodeConnection nodeConnection )
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
            int index = recentNodeConnections.IndexOf(recentNodeConnections.Where(x => x.Address == address).First());
            return index;
        }

        public void LoadRecentConnectionsFromFile(string path)
        {
            try
            {
                var stream = File.OpenRead(path);
                var file = new System.IO.StreamReader(stream);
                var savedNodeConnections = JsonConvert.DeserializeObject<List<RecentNodeConnection>>(file.ReadToEnd());
                recentNodeConnections.AddRange(savedNodeConnections);
            } catch (Exception ex) {
                throw new Exception("Something wen't wrong with reading connections from file");
            }
        }

        public void SaveRecentConnectionsToFile(string path)
        {
            try
            {
                var stream = File.OpenWrite(path);
                var file = new System.IO.StreamWriter(stream);
                var serrializedList = JsonConvert.SerializeObject(recentNodeConnections, Formatting.Indented);
                file.Write(serrializedList);
            } catch (Exception ex)
            {
                throw new Exception("Something wen't wrong with writting connections to file");
            }
        }
    }
}
