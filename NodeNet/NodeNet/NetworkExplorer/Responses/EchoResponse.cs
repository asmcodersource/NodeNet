using NodeNet.NodeNet.NetworkExplorer.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.NetworkExplorer.Responses
{
    internal class EchoResponse : IResponse
    {
        public string MyAddress { get; set; }
        public string MessageType { get { return typeof(EchoResponse).FullName; } }
    }
}
