using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.TcpCommunication
{
    /// <summary>
    /// Used to get TCP-address of node listener
    /// it used to get specific port in case of multiple network interfaces used by one node
    /// </summary>
    internal interface ITcpAddressProvider
    {
        public int GetNodeTcpPort();
        public string GetNodeTcpIP();
    }
}
