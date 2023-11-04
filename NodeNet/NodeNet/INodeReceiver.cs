using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNet
{
    internal interface INodeReceiver
    {
        void StartListening();
        void StopListening();
        Message.Message? GetLastMessage();
        List<Message.Message> GetMessageList();
    }
}
