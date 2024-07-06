using NodeNet.NodeNet.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNet.NodeNet;
using Serilog.Core;
using System.Collections;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public delegate bool MessageFilterPredicate(MessageContext context);

    internal class MessageWaiter
    {
        public MessageFilterPredicate MessageFilterPredicate { get; set; } = (msgContext) => true;
        public bool IsAllowListening { get; set; } = false;

        private Queue<TaskCompletionSource<MessageContext>> currentWaitingTasks = new Queue<TaskCompletionSource<MessageContext>>();
        private readonly Node? listeningNode = null;
        private readonly Queue<MessageContext> messageQueue = new Queue<MessageContext>();

        /// <summary>
        /// Creates queue without linked node
        /// </summary>
        public MessageWaiter(){}

        public MessageWaiter(Node listeningNode)
        {
            this.listeningNode = listeningNode;
            this.listeningNode.MessageReceived += MessageReceivedHandler;
        }

        public void ClearQueue()
        {
            lock (this) // for some special case, maybe it exists? 
            {
                messageQueue.Clear();
            }
        }

        public void AddMessageToQueueManually(MessageContext messageContext)
        {
            lock (this.messageQueue)
            {
                AddMessageToQueue(messageContext);
            }
        }

        public void CancelWaitForMessage(TaskCompletionSource<MessageContext> targetTaskCTS)
        {
            lock (this.messageQueue)
            {
                // Filtering waiting task, to exclude cancelled one
                currentWaitingTasks = new Queue<TaskCompletionSource<MessageContext>>(
                    currentWaitingTasks.Except(new[] { targetTaskCTS })
                );
                // Set waiting task to canceled state
                targetTaskCTS.SetCanceled();
            }
        }

        public async Task<MessageContext> WaitForMessage()
        {
            return await WaitForMessage(CancellationToken.None);
        }

        public Task<MessageContext> WaitForMessage(CancellationToken cancellationToken)
        {
            var completeSource = new TaskCompletionSource<MessageContext>();
            lock (this.messageQueue)
            {
                if( this.messageQueue.Count > 0 )
                {
                    var messageContext = this.messageQueue.Dequeue();
                    return Task.FromResult(messageContext);
                } else
                {
                    currentWaitingTasks.Enqueue(completeSource);
                    cancellationToken.Register(
                        () => CancelWaitForMessage(completeSource) 
                    );
                    return completeSource.Task;
                }
            }
        }

        public void MessageReceivedHandler(MessageContext messageContext)
        {
            lock (this.messageQueue)
            {
                if (IsAllowListening is not true)
                    return;
                AddMessageToQueue(messageContext);
                if ( currentWaitingTasks.Count > 0)
                {
                    var firstWaitingTask = currentWaitingTasks.Dequeue();
                    firstWaitingTask.SetResult(messageContext);
                }
            }
        }

        protected void AddMessageToQueue(MessageContext messageContext)
        {
            // Tt will be possible to immediately filter out messages that are not needed in a given queue;
            // one of the disadvantages is that a larger number of such checks will negatively affect performance.
            if ( MessageFilterPredicate(messageContext) is true )
                messageQueue.Enqueue(messageContext);
        }
    }
}
