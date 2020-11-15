using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Training.Api.Services.Background.Quartz
{
    public static class JobRegistrar
    {
        public static void ScheduleJobs(this IServiceCollection services)
        {
        }

        private static void ScheduleJob<TJob>(this IServiceCollection services, string cronExpression) where TJob: class, IJob
        {
            services.AddSingleton<TJob>();
            
            var jobSchedule = new JobSchedule(typeof(TJob), cronExpression);
            services.AddSingleton(jobSchedule);
        }
    }
}
