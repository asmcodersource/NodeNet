using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet
{
    internal interface INodeSender
    {
        void SetSendProperties();
        void SendMessage();
    }
}
