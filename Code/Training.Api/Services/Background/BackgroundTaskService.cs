using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Training.Dal.Context;

namespace Training.Api.Services.Background
{
    public class BackgroundTaskService: IBackgroundTaskService
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceProvider _services;
        private ConcurrentDictionary<Guid, BackgroundTaskStatus> Statuses { get; }

        public BackgroundTaskService(IBackgroundTaskQueue backgroundTaskQueue, IServiceProvider services)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _services = services;

            Statuses = new ConcurrentDictionary<Guid, BackgroundTaskStatus>();
        }

        public void Queue(BackgroundTask backgroundTask)
        {
            if (backgroundTask == null)
                return;

            if (!Statuses.ContainsKey(backgroundTask.Id))
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);

                var taskStatus = new BackgroundTaskStatus {TaskId = backgroundTask.Id, Name = backgroundTask.Name, QueuedAt = DateTimeOffset.UtcNow};
                Statuses.TryAdd(backgroundTask.Id, taskStatus);
            }
        }

        public IList<BackgroundTaskStatus> GetStatuses()
        {
            return Statuses.Values.ToList();
        }

        public BackgroundTaskStatus GetStatus(Guid taskId)
        {
            if (Statuses.ContainsKey(taskId))
            {
                return Statuses[taskId];
            }

            return null;
        }

        public void ReportProgress(Guid taskId, double progress)
        {
            var status = GetStatus(taskId);
            if (status == null)
                return;

            status.Progress = progress;
        }

        public void IncreaseProgress(Guid taskId, double progress)
        {
            var status = GetStatus(taskId);
            if (status == null)
                return;

            status.Progress += progress;
        }

        public void MarkAsCompleted(Guid taskId, object result = null)
        {
            var status = GetStatus(taskId);
            if (status == null)
                return;

            status.Progress = 1;
            status.IsCompleted = true;
            status.Result = result;
            status.FinishedAt = DateTimeOffset.UtcNow;
        }

        public DataContext GetDataContext()
        {
            var scope = _services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            return dataContext;
        }
    }
}
