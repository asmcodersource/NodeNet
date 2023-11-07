using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNet.Communication
{
    internal interface INodeReceiver
    {
        Message.Message? GetLastMessage();
        List<Message.Message> GetMessageList();

    }
}
