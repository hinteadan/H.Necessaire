using Microsoft.Extensions.DependencyInjection;

namespace H.Necessaire.Runtime.UI.Razor
{
    public static class IoCExtensions
    {
        public static T WithRazorRuntime<T>(this T dependencyRegistry, HRazorApp hRazorApp = null) where T : ImADependencyRegistry
        {
            dependencyRegistry
                .WithHNecessaireRuntimeUI()
                //.Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                //.Register<Components.DependencyGroup>(() => new Components.DependencyGroup())
                .Register<HRazorApp>(() => hRazorApp ?? HRazorApp.Default)
                ;

            return dependencyRegistry;
        }

        public static IServiceCollection WithHRazorRuntime<THRazorApp>(this IServiceCollection services, THRazorApp hRazorApp = null, Action<ImADependencyRegistry> deps = null) where THRazorApp : HRazorApp
        {
            if (services is null)
                return services;

            ImADependencyRegistry registy = hRazorApp?.DependencyRegistry ?? HRazorApp.Default.DependencyRegistry;
            registy.WithRazorRuntime(hRazorApp);
            if (deps is not null)
                deps.Invoke(registy);
            if (hRazorApp is not null && hRazorApp.GetType() != typeof(HRazorApp))
            {
                registy.Register<THRazorApp>(() => hRazorApp);
            }

            services.AddDotNetDependenciesToHNecessaire(registy);
            services.AddHNecessaireDependenciesToDotNet(registy);

            return services;
        }

        public static IServiceCollection AddDotNetDependenciesToHNecessaire(this IServiceCollection services, ImADependencyRegistry dependencyRegistry)
        {
            //if (services == null)
            //    return services;

            //dependencyRegistry.RegisterAlwaysNew<ImAUseCaseContextProvider>(() =>
            //{
            //    //IServiceProvider netCoreServiceProvider = services.BuildServiceProvider();
            //    return
            //        new MauiAppToUseCaseContextProvider();
            //});

            return services;
        }

        public static IServiceCollection AddHNecessaireDependenciesToDotNet(this IServiceCollection services, ImADependencyRegistry dependencyRegistry)
        {
            services.AddSingleton<ImADependencyProvider>(dependencyRegistry);

            //dependencyRegistry.Register<ImAConfigProvider>(() =>
            //{
            //    IServiceProvider netCoreServiceProvider = services.BuildServiceProvider();
            //    return
            //        new NetCoreConfigProvider(netCoreServiceProvider.GetService<IConfiguration>());
            //});

            foreach (KeyValuePair<Type, Func<object>> singletonDependency in dependencyRegistry.GetAllOneTimeTypes())
            {
                services.AddSingleton(singletonDependency.Key, x => dependencyRegistry.Get(singletonDependency.Key));
                //if (typeof(IHostedService).IsAssignableFrom(singletonDependency.Key))
                //{
                //    services.AddSingleton<IHostedService>(x => (IHostedService)dependencyRegistry.Get(singletonDependency.Key));
                //}
            }

            foreach (KeyValuePair<Type, Func<object>> transientDependency in dependencyRegistry.GetAllAlwaysNewTypes())
            {
                services.AddTransient(transientDependency.Key, x => dependencyRegistry.Get(transientDependency.Key));
            }

            return services;
        }
    }
}
