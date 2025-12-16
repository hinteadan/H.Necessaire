using H.Necessaire.Runtime.Integration.AspNetCore.Concrete;
using H.Necessaire.Runtime.Integration.DotNet.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace H.Necessaire.Runtime.Integration.AspNetCore
{
    public static class IoCExtensions
    {
        public static THost BindToHNecessaireAspNetRuntime<THost>(this THost dotNetHost, ImADependencyRegistry dependencyRegistry) where THost : IHost
        {
            dotNetHost?.Services?.BindToHNecessaireAspNetRuntime(dependencyRegistry);
            return dotNetHost;
        }

        public static IServiceCollection WithHNecessaireAspNetRuntime(this IServiceCollection services, ImADependencyRegistry dependencyRegistry, IConfiguration configuration = null)
        {
            dependencyRegistry.Register<DependencyGroup>(() => new DependencyGroup(configuration));

            services.AddSingleton(dependencyRegistry);
            services.AddSingleton<ImADependencyProvider>(dependencyRegistry);
            services.AddSingleton<ImADependencyBrowser>(dependencyRegistry);

            if (configuration != null)
            {
                NetCoreConfigProvider.RegisterWith(dependencyRegistry, configuration);
            }

            foreach (KeyValuePair<Type, Func<object>> singletonDependency in dependencyRegistry.GetAllOneTimeTypes())
            {
                services.AddSingleton(singletonDependency.Key, x => dependencyRegistry.Get(singletonDependency.Key));
                if (typeof(IHostedService).IsAssignableFrom(singletonDependency.Key))
                {
                    services.AddSingleton<IHostedService>(x => (IHostedService)dependencyRegistry.Get(singletonDependency.Key));
                }
            }

            foreach (KeyValuePair<Type, Func<object>> transientDependency in dependencyRegistry.GetAllAlwaysNewTypes())
            {
                services.AddTransient(transientDependency.Key, x => dependencyRegistry.Get(transientDependency.Key));
            }

            return services.ConfigureHNecessaireAspNetRuntime();
        }

        static IServiceCollection ConfigureHNecessaireAspNetRuntime(this IServiceCollection services)
        {
            services.AddCors(opts => opts.AddDefaultPolicy(bld => bld.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            services.AddRouting();
            services.AddControllers().AddControllersAsServices();

            return services;
        }

        static void BindToHNecessaireAspNetRuntime(this IServiceProvider serviceProvider, ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ImAUseCaseContextProvider>(() => new HttpContextToUseCaseContextProvider(serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext));
        }
    }
}
