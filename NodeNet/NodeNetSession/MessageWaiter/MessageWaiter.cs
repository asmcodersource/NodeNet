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
using NodeNet.NodeNetSession.SessionMessage;
using System.Text.Json;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public class MessageWaiter
    {
        public MessageFilterPredicate MessageFilterPredicate { get; set; } = (msgContext) => true;
        public bool IsAllowListening { get; set; } = false;

        private Queue<TaskCompletionSource<SessionMessageContext>> currentWaitingTasks = new Queue<TaskCompletionSource<SessionMessageContext>>();
        private readonly Node? listeningNode = null;
        private readonly Queue<SessionMessageContext> messageQueue = new Queue<SessionMessageContext>();

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

        public void AddMessageToQueueManually(SessionMessageContext messageContext)
        {
            lock (this.messageQueue)
            {
                AddMessageToQueue(messageContext);
            }
        }

        public void CancelWaitForMessage(TaskCompletionSource<SessionMessageContext> targetTaskCTS)
        {
            lock (this.messageQueue)
            {
                // Filtering waiting task, to exclude cancelled one
                currentWaitingTasks = new Queue<TaskCompletionSource<SessionMessageContext>>(
                    currentWaitingTasks.Except(new[] { targetTaskCTS })
                );
                // Set waiting task to canceled state
                targetTaskCTS.TrySetCanceled();
            }
        }

        public async Task<SessionMessageContext> WaitForMessage()
        {
            return await WaitForMessage(CancellationToken.None);
        }

        public Task<SessionMessageContext> WaitForMessage(CancellationToken cancellationToken)
        {
            var completeSource = new TaskCompletionSource<SessionMessageContext>();
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
            SessionMessage.SessionMessage? sessionMsg = null;
            try
            {
                sessionMsg = JsonSerializer.Deserialize<SessionMessage.SessionMessage>(messageContext.Message.Data);
            } catch { }
            var sessionMsgContext = new SessionMessageContext(messageContext, sessionMsg);
            lock (this.messageQueue)
            {
                if (IsAllowListening is not true)
                    return;
                AddMessageToQueue(sessionMsgContext);
                if (currentWaitingTasks.Count > 0 && messageQueue.Count > 0)
                {
                    var firstWaitingTask = currentWaitingTasks.Dequeue();
                    firstWaitingTask.SetResult(messageQueue.Dequeue());
                }
            }
        }

        protected void AddMessageToQueue(SessionMessageContext messageContext)
        {
            // Tt will be possible to immediately filter out messages that are not needed in a given queue;
            // one of the disadvantages is that a larger number of such checks will negatively affect performance.
            if ( MessageFilterPredicate(messageContext) is true )
                messageQueue.Enqueue(messageContext);
        }
    }
}
