using System;
using System.Threading;
using System.Threading.Tasks;

namespace Training.Api.Services.Background.Quartz
{
    public interface IQuartzService
    {
        Task StartScheduler(CancellationToken cancellationToken);
        Task StopScheduler(CancellationToken cancellationToken);
        Task ScheduleNow(Type jobType);
        Task ScheduleNow(string jobName);
    }
}