using NodeNet.NodeNet.Message;
using NodeNet.NodeNet.RSASigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    // Message sing verification middleware
    // If message sing isn't correct than next middlewares will not be called
    internal class SignVerificationMiddleware : IReceiveMiddleware
    {
        public IReceiveMiddleware Next { get; protected set; } = null;
        public IMessageValidator MessageValidator { get; protected set; }

        public SignVerificationMiddleware(Node node, IMessageValidator messageValidator)
        {
            MessageValidator = messageValidator; 
        }

        public bool Invoke(MessageContext messageContext)
        {
            MessageValidator.SetValidateOptions(new ReceiverSignOptions(messageContext.Message));
            var signCorrect = MessageValidator.Validate(messageContext.Message);
            if (signCorrect)
                return Next != null ? Next.Invoke(messageContext) : true;
            else
                return false;
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }
    }
}
