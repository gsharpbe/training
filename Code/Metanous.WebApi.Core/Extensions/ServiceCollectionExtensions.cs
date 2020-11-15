using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Metanous.WebApi.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Register<TInterface, TService>(this IServiceCollection services)
            where TInterface : class
            where TService : class, TInterface
        {
            services.TryAdd(ServiceDescriptor.Scoped<TInterface, TService>());

            return services;
        }

        public static IServiceCollection RegisterTransient<TInterface, TService>(this IServiceCollection services)
            where TInterface : class
            where TService : class, TInterface
        {
            services.TryAdd(ServiceDescriptor.Transient<TInterface, TService>());

            return services;
        }

        public static IServiceCollection Remove<TInterface>(this IServiceCollection services)
            where TInterface : class
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TInterface));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);

            return services;
        }

        public static IServiceCollection ReplaceScoped<TInterface, TService>(this IServiceCollection services)
            where TInterface : class
            where TService : class, TInterface
        {
            services.Remove<TInterface>().Register<TInterface, TService>();

            return services;
        }

        public static IServiceCollection ReplaceSingleton<TInterface, TService>(this IServiceCollection services)
            where TInterface : class
            where TService : class, TInterface
        {
            services.Remove<TInterface>().AddSingleton<TInterface, TService>();

            return services;
        }
    }
}
