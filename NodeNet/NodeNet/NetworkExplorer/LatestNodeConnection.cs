using NodeNet.NodeNet.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.NetworkExplorer
{
    internal record RecentNodeConnection
    {
        public String Address { get; set; }
        public DateTime LastConnectionTime { get; set; }
        public DateTime LastOnlineTime { get; set; }

        public static RecentNodeConnection CreateFromNodeConnection(string connectionAddress)
        {
            var recentNodeConnection = new RecentNodeConnection();
            recentNodeConnection.Address = connectionAddress;
            recentNodeConnection.LastConnectionTime = DateTime.UtcNow;
            recentNodeConnection.LastOnlineTime = DateTime.UtcNow;
            return recentNodeConnection;
        }
    }
}
