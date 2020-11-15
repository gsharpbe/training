using System;
using System.Collections.Generic;
using Training.Dal.Context;

namespace Training.Api.Services.Background
{
    public interface IBackgroundTaskService
    {
        void Queue(BackgroundTask backgroundTask);

        IList<BackgroundTaskStatus> GetStatuses();
        BackgroundTaskStatus GetStatus(Guid taskId);

        void ReportProgress(Guid taskId, double progress);
        void IncreaseProgress(Guid taskId, double progress);

        void MarkAsCompleted(Guid taskId, object result = null);

        DataContext GetDataContext();
    }
}