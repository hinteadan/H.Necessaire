using System.Reflection;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    internal static class HAppConfig
    {
#if DEBUG
        public const bool IsDebug = true;
#else
        public const bool IsDebug = false;
#endif

        static readonly Assembly mainAssembly = typeof(HAppConfig).Assembly;
        static readonly RuntimeConfig defaultRuntimeConfig = new()
        {
            Values = [
                "App".ConfigWith(
                    "Name".ConfigWith(mainAssembly.GetName().Name)
                )
            ],
        };

        public static T WithDefaultHAppConfig<T>(this T depsRegistry) where T : ImADependencyRegistry
        {
            depsRegistry.Register<RuntimeConfig>(() => defaultRuntimeConfig);
            return depsRegistry;
        }
    }
}
