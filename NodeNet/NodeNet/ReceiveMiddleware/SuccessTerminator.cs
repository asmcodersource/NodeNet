using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    public class SuccessTerminator : IReceiveMiddleware
    {
        public bool Invoke(MessageContext messageContext)
        {
            return true;
        }

        public void SetNext(IReceiveMiddleware next)
        {
            throw new NotImplementedException();
        }
    }
}
