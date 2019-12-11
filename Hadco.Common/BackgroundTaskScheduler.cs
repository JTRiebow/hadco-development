using System;
using System.Threading;
using System.Web.Hosting;

namespace Hadco.Common
{
    /// <summary>
    /// add some jobs to the background queue
    /// </summary>
    public static class BackgroundTaskScheduler
    {
        /// <summary>
        /// Sends the work item to the background queue.
        /// During a unit test it just runs the task instead of erroring out due to an invalid hosting environment.
        /// </summary>
        /// <param name="workItem">work item to enqueue</param>
        public static void QueueBackgroundWorkItem(Action<CancellationToken> workItem)
        {
            try
            {
                HostingEnvironment.QueueBackgroundWorkItem(workItem);
            }
            catch (InvalidOperationException)
            {
                workItem.Invoke(new CancellationToken());
            }
        }
    }
}