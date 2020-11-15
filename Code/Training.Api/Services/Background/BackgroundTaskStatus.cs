using System;

namespace Training.Api.Services.Background
{
    public class BackgroundTaskStatus
    {
        /// <summary>
        /// Reference to the id of the background task
        /// </summary>
        public Guid TaskId { get;set; }

        /// <summary>
        /// The name of the background task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The progress in %
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Indicates if the task is completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// The result of the processing of the background task
        /// </summary>
        public object Result { get;set; }

        /// <summary>
        /// Time when the task was queued
        /// </summary>
        public DateTimeOffset? QueuedAt { get; set; }

        /// <summary>
        /// Time when the task finished
        /// </summary>
        public DateTimeOffset? FinishedAt { get; set; }
    }
}
