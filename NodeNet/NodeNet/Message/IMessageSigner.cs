using NodeNet.NodeNet.SignOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Message
{
    internal interface IMessageSigner
    {
        void SetSignOptions(ISenderSignOptions options);
        void Sign(Message message);
    }
}
