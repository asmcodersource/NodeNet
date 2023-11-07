using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Communication
{
    internal interface INodeSender
    {
        public void SendMessage(Message.Message message);
    }
}
