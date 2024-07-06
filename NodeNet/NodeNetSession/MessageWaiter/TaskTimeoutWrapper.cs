using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNet.NodeNetSession.MessageWaiter
{
    public static class TaskTimeoutWrapper<T>
    {
        /// <summary>
        /// Creates a new task that will be canceled if the task has been running for more time than specified in the argument
        /// At the moment I am not sure that this will be used, since a better solution has been found.
        /// </summary>
        /// <param name="wrappingTask">The task on which the timeout wrapper is applied</param>
        /// <param name="timeout">The time for which the timeout is applied</param>
        /// <returns>If executed before the timeout occurs, returns the wrapped task</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public static async Task<T> Timeout(Task<T> wrappingTask, TimeSpan timeout)
        {
            var delayTask = Task.Delay(timeout);
            await Task.WhenAny(wrappingTask, delayTask);
            if (wrappingTask.IsCompleted is not true)
                throw new OperationCanceledException();
            else if (wrappingTask.IsFaulted)
                throw wrappingTask.Exception ?? new Exception();
            else
                return wrappingTask.Result;
        }
    }
}
