#pragma warning disable 4014

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Training.Api.Services.Background
{
    public class BackgroundTaskProcessor: IHostedService
    {
        private const int MaxNumberOfParallelTasks = 20;

        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<BackgroundTaskProcessor> _logger;

        private Task _backgroundTask;
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();

        public BackgroundTaskProcessor(IBackgroundTaskQueue taskQueue, ILogger<BackgroundTaskProcessor> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundTaskService is starting");

            _backgroundTask = Task.Run(BackgroundProcessing);

            _logger.LogInformation("BackgroundTaskService is started");

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundTaskService is stopping");

            _shutdown.Cancel();

            await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));

            _logger.LogInformation("BackgroundTaskService is stopped");
        }

        private async Task BackgroundProcessing()
        {
            var semaphore = new SemaphoreSlim(MaxNumberOfParallelTasks);

            void HandleTask(Task task)
            {
                semaphore.Release();
            }

            while (!_shutdown.IsCancellationRequested)
            {
                await semaphore.WaitAsync();

                var backgroundTask = await _taskQueue.DequeueAsync(_shutdown.Token);
                if (backgroundTask != null)
                {
                    var task = backgroundTask.WorkItem(_shutdown.Token);

                    // fire and forget: in order to support parallelism, we are not going to wait for the execution of this task to finish
                    task.ContinueWith(HandleTask);
                }
                
            }
        }
    }
}
