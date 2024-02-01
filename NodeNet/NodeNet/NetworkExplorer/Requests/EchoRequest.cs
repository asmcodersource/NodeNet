using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.NetworkExplorer.Requests
{
    internal class EchoRequest : IRequest
    {
        public string MyAddress { get; set; }
        public string MessageType { get { return typeof(EchoRequest).FullName; } }
    }
}
