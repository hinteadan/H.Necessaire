using CommunityToolkit.Maui;
using H.Necessaire.Runtime.MAUI.Core;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.UI;
using System.Reflection;

namespace H.Necessaire.Runtime.MAUI
{
    public static class IoCExtensions
    {
        public static T WithMauiRuntime<T>(this T dependencyRegistry, HMauiApp hMauiApp = null) where T : ImADependencyRegistry
        {
            dependencyRegistry
                .WithHNecessaireRuntimeUI()
                .Register<Components.DependencyGroup>(() => new Components.DependencyGroup())
                .Register<HMauiApp>(() => hMauiApp ?? HMauiApp.Default)
                ;

            return dependencyRegistry;
        }

        public static MauiAppBuilder WithHNecessaire<THMauiApp>(this MauiAppBuilder appBuilder, THMauiApp hMauiApp = null) where THMauiApp : HMauiApp
        {
            ImADependencyRegistry registy = hMauiApp?.DependencyRegistry ?? HMauiApp.Default.DependencyRegistry;
            registy.WithMauiRuntime(hMauiApp);
            if (hMauiApp is not null && hMauiApp.GetType() != typeof(HMauiApp))
            {
                registy.Register<THMauiApp>(() => hMauiApp);
            }

            appBuilder.Services.AddDotNetDependenciesToHNecessaire(registy);

            appBuilder.Services.AddHNecessaireDependenciesToDotNet(registy);

            appBuilder.UseMauiCommunityToolkit();


            return appBuilder;
        }
        public static MauiAppBuilder WithHNecessaire(this MauiAppBuilder appBuilder) => appBuilder.WithHNecessaire(HMauiApp.Default);


        public static IServiceCollection AddDotNetDependenciesToHNecessaire(this IServiceCollection services, ImADependencyRegistry dependencyRegistry)
        {
            //if (services == null)
            //    return services;

            dependencyRegistry.RegisterAlwaysNew<ImAUseCaseContextProvider>(() =>
            {
                //IServiceProvider netCoreServiceProvider = services.BuildServiceProvider();
                return
                    new MauiAppToUseCaseContextProvider();
            });

            return services;
        }

        public static IServiceCollection AddHNecessaireDependenciesToDotNet(this IServiceCollection services, ImADependencyRegistry dependencyRegistry)
        {
            services.AddSingleton<ImADependencyRegistry>(dependencyRegistry);
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

        public static IFontCollection AddHNecessaireFonts(this IFontCollection fonts)
        {
            Assembly assembly = typeof(HMauiApp).Assembly;

            foreach (Typography typography in WellKnownTypography.All)
            {
                fonts.AddEmbeddedResourceFont(assembly, typography.FontFamilyUrls.First(), typography.FontFamily);
            }

            return fonts;
        }
    }
}
