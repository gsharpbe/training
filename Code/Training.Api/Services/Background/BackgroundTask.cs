using System;
using System.Threading;
using System.Threading.Tasks;

namespace Training.Api.Services.Background
{
    public class BackgroundTask
    {
        /// <summary>
        /// The id of the background task
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The name of the background task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The task to fulfill
        /// </summary>
        public Func<CancellationToken, Task> WorkItem { get; set; }

        public BackgroundTask()
        {
            Id = Guid.NewGuid();
        }

        public BackgroundTask(Func<CancellationToken, Task> workItem): this()
        {
            WorkItem = workItem;
        }
    }
}
