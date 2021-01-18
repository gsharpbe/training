using Metanous.Model.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Training.Api.Services.Background;
using Training.Api.Services.Background.Quartz;
using Training.Api.Services.Base;
using Training.Api.Services.Customer;
using Training.Api.Services.DomainData;
using Training.Api.Services.Project;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Services
{
    public static class ServiceRegistrar
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // background tasks
            services.AddHostedService<BackgroundTaskProcessor>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IBackgroundTaskService, BackgroundTaskService>();

            // quartz
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IQuartzService, QuartzService>();
            services.AddHostedService<QuartzHostedService>();

            // services
            services.Register<CountryService, Country, CountryModel>();
            services.Register<CustomerService, Model.Customer, CustomerModel>();
            services.Register<ProjectService, Model.Project, ProjectModel>();
        }

        private static void Register<TService, TServiceModel, TModel>(this IServiceCollection services)
            where TModel : ModelBase
            where TServiceModel : ServiceModelBase
            where TService : class, IService<TServiceModel, TModel>
        {
            services.TryAdd(ServiceDescriptor.Scoped<IService<TServiceModel, TModel>, TService>());
        }
    }
}
