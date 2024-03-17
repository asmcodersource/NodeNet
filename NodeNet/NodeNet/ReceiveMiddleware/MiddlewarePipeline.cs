using NodeNet.NodeNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNet.ReceiveMiddleware
{
    public class MiddlewarePipeline
    {
        protected List<IReceiveMiddleware> pipeline = new List<IReceiveMiddleware>();

        public void AddHandler(IReceiveMiddleware handler)
        {
            if (pipeline.Count() != 0)
                pipeline.Last().SetNext(handler);
            pipeline.Add(handler);
        }

        public bool Handle(MessageContext messageContext)
            => pipeline.First().Invoke(messageContext);
    }
}
