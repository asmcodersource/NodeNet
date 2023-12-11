using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.HttpCommunication
{
    internal class HttpListenerOptions
    {
        public int Port { get; set; } = 8080;
        public string Resource { get; set; } = "websock";


        public HttpListenerOptions() { }

        public HttpListenerOptions(int port, string resource)
        {
            Port = port;
            Resource = resource;
        }
    }
}
