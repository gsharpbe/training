using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Training.Api.Services.Background.Quartz
{
    public class QuartzHostedService : IHostedService
    {
        private readonly IQuartzService _quartzService;

        public QuartzHostedService(IQuartzService quartzService)
        {
            _quartzService = quartzService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _quartzService.StartScheduler(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _quartzService.StopScheduler(cancellationToken);
        }
    }
}
