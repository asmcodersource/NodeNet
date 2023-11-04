using NodeNet.NodeNet.SignOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.Message
{
    internal interface IMessageValidator
    {
        void SetValidateOptions(IReceiverSignOptions options);
        bool Validate(Message message);
    }
}
