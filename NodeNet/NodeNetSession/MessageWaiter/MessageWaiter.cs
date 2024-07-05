using NodeNet.NodeNet.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    internal class MessageWaiter
    {
        private readonly Node listeningNode;
        private readonly Queue<MessageContext> messageQueue = new Queue<MessageContext>();
        private readonly Queue<TaskCompletionSource<MessageContext>> currentWaitingTasks = new Queue<TaskCompletionSource<MessageContext>>();
        public MessageWaiter(Node listeningNode)
        {
            this.listeningNode = listeningNode;
            this.listeningNode.MessageReceived += MessageReceivedHandler;
        }

        public Task<MessageContext> WaitForMessage()
        {
            var completedSource = new TaskCompletionSource<MessageContext>();
            lock (this.messageQueue)
            {
                if( this.messageQueue.Count > 0)
                {
                    var messageContext = this.messageQueue.Dequeue();
                    return Task.FromResult(messageContext);
                } else
                {
                    currentWaitingTasks.Enqueue(completedSource);
                    return completedSource.Task;
                }
            }
        }

        public void MessageReceivedHandler(MessageContext messageContext)
        {
            lock (this.messageQueue)
            {
                messageQueue.Enqueue(messageContext);
                if( currentWaitingTasks.Count > 0)
                {
                    var firstWaitingTask = currentWaitingTasks.Dequeue();
                    firstWaitingTask.SetResult(messageContext);
                }
            }
        }
    }
}
