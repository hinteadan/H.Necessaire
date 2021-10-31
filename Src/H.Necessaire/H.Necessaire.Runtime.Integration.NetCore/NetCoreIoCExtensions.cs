using H.Necessaire.Runtime;
using H.Necessaire.Runtime.Integration.NetCore.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public static class NetCoreIoCExtensions
    {
        public static IServiceCollection AddHNecessaireDependencies(this IServiceCollection services, ImADependencyRegistry dependencyRegistry)
        {
            if (services == null)
                return services;

            services.AddHttpContextAccessor();

            dependencyRegistry.RegisterAlwaysNew<ImAUseCaseContextProvider>(() =>
            {
                IServiceProvider netCoreServiceProvider = services.BuildServiceProvider();
                return
                    new HttpContextToUseCaseContextProvider(netCoreServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext);
            });

            dependencyRegistry.RegisterAlwaysNew<ImAConfigProvider>(() =>
            {
                IServiceProvider netCoreServiceProvider = services.BuildServiceProvider();
                return
                    new NetCoreConfigProvider(netCoreServiceProvider.GetService<IConfiguration>());
            });

            foreach (KeyValuePair<Type, Func<object>> transientDependency in dependencyRegistry.GetAllAlwaysNewTypes())
                services.AddTransient(transientDependency.Key, x => dependencyRegistry.Get(transientDependency.Key));

            foreach (KeyValuePair<Type, Func<object>> singletonDependency in dependencyRegistry.GetAllOneTimeTypes())
                services.AddSingleton(singletonDependency.Key, x => dependencyRegistry.Get(singletonDependency.Key));

            return services;
        }
    }
}
