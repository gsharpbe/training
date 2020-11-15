using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Training.Api.Services.Background
{
    public class BackgroundTaskQueue: IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<BackgroundTask> _backgroundTasks;
        private readonly SemaphoreSlim _signal;

        public BackgroundTaskQueue()
        {
            _backgroundTasks = new ConcurrentQueue<BackgroundTask>();
            _signal = new SemaphoreSlim(0);
        }

        public void QueueBackgroundWorkItem(BackgroundTask backgroundTask)
        {
            if (backgroundTask == null)
            {
                throw new ArgumentNullException(nameof(backgroundTask));
            }

            _backgroundTasks.Enqueue(backgroundTask);
            _signal.Release();
        }

        public async Task<BackgroundTask> DequeueAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            await _signal.WaitAsync(cancellationToken);
            _backgroundTasks.TryDequeue(out var backgroundTask);

            return backgroundTask;
        }
    }
}
