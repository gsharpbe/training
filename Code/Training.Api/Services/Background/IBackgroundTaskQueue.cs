using System.Threading;
using System.Threading.Tasks;

namespace Training.Api.Services.Background
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(BackgroundTask backgroundTask);

        Task<BackgroundTask> DequeueAsync(CancellationToken cancellationToken);
    }
}