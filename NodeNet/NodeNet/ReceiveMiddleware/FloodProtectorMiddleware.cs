using NodeNet.NodeNet.Communication;
using NodeNet.NodeNet.Message;

namespace NodeNet.NodeNet.ReceiveMiddleware
{

    public class FloodProtectorMiddleware : IReceiveMiddleware
    {
        protected Dictionary<INodeConnection, ulong> dataPerConnection = new Dictionary<INodeConnection, ulong>();
        protected Queue<ReceivedDataInfo> receiveDataQueue = new Queue<ReceivedDataInfo>();
        public IReceiveMiddleware Next { get; protected set; } = null;
        public ulong MaxScorePerConnection { get; set; } = 50_000_000;

        public bool Invoke(MessageContext messageContext)
        {
            lock (this)
            {
                var dataInfo = new ReceivedDataInfo(messageContext);
                receiveDataQueue.Enqueue(dataInfo);
                if (dataPerConnection.ContainsKey(messageContext.SenderConnection))
                    dataPerConnection[messageContext.SenderConnection] += dataInfo.ReceivedDataScore;
                else
                    dataPerConnection.Add(messageContext.SenderConnection, dataInfo.ReceivedDataScore);
                RemoveOutOfWindow();
                if (dataPerConnection[messageContext.SenderConnection] > MaxScorePerConnection)
                    return false;
                else if (Next != null)
                    return Next.Invoke(messageContext);
                else
                    return true;
            }
        }

        public void RemoveOutOfWindow()
        {
            while (receiveDataQueue.Count > 0)
            {
                var firstDataScore = receiveDataQueue.Peek();
                if (firstDataScore.ReceiveTime > DateTime.UtcNow)
                    return;
                receiveDataQueue.Dequeue();
                dataPerConnection[firstDataScore.Connection] -= firstDataScore.ReceivedDataScore;
            }
        }

        public void SetNext(IReceiveMiddleware next)
        {
            Next = next;
        }
    }

    public class ReceivedDataInfo
    {
        public ulong ReceivedDataScore { get; protected set; }
        public INodeConnection Connection { get; protected set; }
        public DateTime ReceiveTime { get; protected set; }


        public ReceivedDataInfo(MessageContext messageContext)
        {
            ulong inputMessageSizeScore = 50;
            double inputMessageDateScaleScore = 1.0;
            ReceivedDataScore = inputMessageSizeScore + (ulong)(inputMessageDateScaleScore * messageContext.Message.Data.Length);
            Connection = messageContext.SenderConnection;
            ReceiveTime = DateTime.UtcNow.AddSeconds(10);
        }
    }
}
