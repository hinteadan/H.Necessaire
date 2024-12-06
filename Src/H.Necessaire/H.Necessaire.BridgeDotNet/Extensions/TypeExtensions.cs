using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static Type[] GetAllImplementations(this Type baseType, params Assembly[] assembliesToScan)
        {
            assembliesToScan
                = assembliesToScan?.Any() == true
                ? assembliesToScan
                : AppDomain.CurrentDomain.GetAssemblies()
                ;

            return
                assembliesToScan
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

        public static string GetID(this Type type)
        {
            return
                (type?.GetCustomAttributes(typeof(IDAttribute), false)?.SingleOrDefault() as IDAttribute)?.ID
                ;
        }

        public static string[] GetAliases(this Type type)
        {
            return
                type?.GetCustomAttributes(typeof(AliasAttribute), false)?.SelectMany(aliasAttr => (aliasAttr as AliasAttribute)?.Aliases)?.Distinct()?.ToArray()
                ;
        }

        public static string[] GetCategories(this Type type)
        {
            return
                type?.GetCustomAttributes(typeof(CategoryAttribute), true)?.SelectMany(noteAttr => (noteAttr as CategoryAttribute)?.Categories)?.Distinct()?.ToArray()
                ;
        }

        public static bool IsMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (type.IsTypeNameMatch(identifier))
                return true;

            if (type.IsTypeIDMatch(identifier))
                return true;

            if (type.IsTypeAliasMatch(identifier))
                return true;

            if (type.IsTypeNamePartialMatch(identifier))
                return true;

            return false;
        }

        public static bool IsTypeNameMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (identifier.Is(type.Name))
                return true;

            return false;
        }

        public static bool IsTypeIDMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (identifier.Is(type.GetID()))
                return true;

            return false;
        }

        public static bool IsTypeAliasMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (identifier.In(type.GetAliases(), (key, alias) => key.Is(alias)))
                return true;

            return false;
        }

        public static bool IsTypeNamePartialMatch(this Type type, string identifier)
        {
            if (type == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            if (type.Name?.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase) == true)
                return true;

            return false;
        }

        public static bool IsNestedUnder(this Type type, Type declaringTypeToCheck)
        {
            if (type?.IsNested != true)
                return false;

            if (type?.DeclaringType is null)
                return false;

            return type.DeclaringType == declaringTypeToCheck || type.DeclaringType.IsNestedUnder(declaringTypeToCheck);
        }

        public static IEnumerable<Type> FindMatchingConcreteTypes(this Type type, string identifier, params Assembly[] assembliesToScan)
        {
            if (identifier.IsEmpty())
                return Enumerable.Empty<Type>();

            return
                type
                .GetAllImplementations(assembliesToScan)
                .Where(x => x.IsMatch(identifier))
                ;
        }
    }
}
