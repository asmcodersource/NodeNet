
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet
{
    internal class Node : INodeReceiver, INodeSender
    {
        Message.Message? INodeReceiver.GetLastMessage()
        {
            throw new NotImplementedException();
        }

        List<Message.Message> INodeReceiver.GetMessageList()
        {
            throw new NotImplementedException();
        }

        void INodeSender.SendMessage()
        {
            throw new NotImplementedException();
        }

        void INodeSender.SetSendProperties()
        {
            throw new NotImplementedException();
        }

        void INodeReceiver.StartListening()
        {
            throw new NotImplementedException();
        }

        void INodeReceiver.StopListening()
        {
            throw new NotImplementedException();
        }
    }
}
