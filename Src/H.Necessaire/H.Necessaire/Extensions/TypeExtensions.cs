using System;
using System.Linq;

namespace H.Necessaire
{
    public static class TypeExtensions
    {
        public static string TypeName(this object instance)
        {
            if (instance is Type)
                return (instance as Type).FullName;

            return instance.GetType().FullName;
        }

        public static bool IsSameOrSubclassOf(this Type typeToCheck, Type typeToCompareWith)
        {
            return
                typeToCheck == typeToCompareWith
                || typeToCompareWith.IsSubclassOf(typeToCheck);
        }

        public static Type[] GetAllImplementations(this Type baseType)
        {
            return
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(
                    assembly => assembly
                    .GetTypes()
                    .Where(
                        p =>
                        p != baseType
                        && baseType.IsAssignableFrom(p)
                        && !p.IsAbstract
                    )
                )
                .ToArray();
        }

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, string component, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(component, dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, Type type, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(type.TypeName(), dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger<T>(this ImADependencyProvider dependencyProvider, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger<T>(dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static RuntimeConfig GetRuntimeConfig(this ImADependencyProvider dependencyProvider)
            => dependencyProvider?.Get<ImAConfigProvider>()?.GetRuntimeConfig() ?? dependencyProvider?.Get<RuntimeConfig>() ?? RuntimeConfig.Empty;
    }
}
