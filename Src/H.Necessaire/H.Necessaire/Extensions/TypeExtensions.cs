using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public static class TypeExtensions
    {
        static readonly string[] coreAssemblyPrefixes = new string[] { "Microsoft.", "System.", "WinRT.", "Skia" };
        public static string TypeName(this object instance)
        {
            if (instance is null)
                return null;

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
                : AppDomain.CurrentDomain.GetNonCoreAssemblies()
                ;

            return
                assembliesToScan
                .SelectMany(
                    assembly => assembly
                    .GetTypes()
                    .Where(
                        p =>
                        p != baseType
                        && !p.IsAbstract
                        && IsAssignableFrom(baseType, p)
                    )
                )
                .ToArray();
        }

        public static Type[] GetAllInterfaces(this Type interfaceType, params Assembly[] assembliesToScan)
        {
            assembliesToScan
                = assembliesToScan?.Any() == true
                ? assembliesToScan
                : AppDomain.CurrentDomain.GetNonCoreAssemblies()
                ;

            return
                assembliesToScan
                .SelectMany(
                    assembly => assembly
                    .GetTypes()
                    .Where(
                        p => (p == interfaceType) || (p.GetInterfaces()?.Any(i => i == interfaceType) == true) || IsAssignableFrom(interfaceType, p)
                    )
                )
                .ToArray();
        }

        private static bool IsAssignableFrom(Type baseType, Type typeToCheck)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                if (typeToCheck.IsGenericType)
                    return baseType.IsAssignableFrom(typeToCheck.GetGenericTypeDefinition());

                return IsInstanceOfGenericType(baseType, typeToCheck);
            }

            if (baseType.IsGenericType && typeToCheck.IsGenericTypeDefinition)
            {
                return IsInstanceOfGenericType(baseType.GetGenericTypeDefinition(), typeToCheck);
            }

            return baseType.IsAssignableFrom(typeToCheck);
        }

        private static bool IsInstanceOfGenericType(Type openGenericType, Type typeToCheck)
        {
            if (openGenericType.IsInterface)
            {
                Type[] implementedInterfaces = typeToCheck.GetInterfaces()?.Where(i => i.IsGenericType).ToNoNullsArray();
                if (implementedInterfaces is null)
                    return false;

                return implementedInterfaces.Any(i => i.GetGenericTypeDefinition() == openGenericType);
            }

            Type type = typeToCheck;

            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
                {
                    return true;
                }
                type = type.BaseType;
            }

            return false;
        }

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, string component, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(component, dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger(this ImADependencyProvider dependencyProvider, Type type, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger(type.TypeName(), dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static ImALogger GetLogger<T>(this ImADependencyProvider dependencyProvider, string application = "H.Necessaire")
            => dependencyProvider?.Get<ImALoggerFactory>()?.BuildLogger<T>(dependencyProvider.GetRuntimeConfig()?.Get("App")?.Get("Name")?.ToString() ?? application);

        public static RuntimeConfig GetRuntimeConfig(this ImADependencyProvider dependencyProvider)
            => dependencyProvider?.Get<ImAConfigProvider>()?.GetRuntimeConfig() ?? dependencyProvider?.Get<RuntimeConfig>() ?? RuntimeConfig.Empty;

        public static ImACacher<T> GetCacher<T>(this ImADependencyProvider dependencyProvider, string cacherID = null)
            => dependencyProvider?.Get<ImACacherFactory>()?.BuildCacher<T>(cacherID);

        public static string GetID(this Type type)
        {
            return
                (type?.GetCustomAttributes(typeof(IDAttribute), false)?.SingleOrDefault() as IDAttribute)?.ID
                ;
        }

        public static string GetIDOrTypeName(this Type type, string fallbackTo = null)
        {
            return
                (type?.GetID()).IfEmpty(type?.Name).IfEmpty(fallbackTo)
                ;
        }

        public static string GetID(this PropertyInfo propertyInfo)
        {
            return
                (propertyInfo?.GetCustomAttributes(typeof(IDAttribute), false)?.SingleOrDefault() as IDAttribute)?.ID
                ?? propertyInfo?.Name
                ;
        }

        public static string GetID(this Type type, string propertyName)
            => type?.GetProperty(propertyName)?.GetID() ?? propertyName;

        public static string GetDisplayLabel(this PropertyInfo propertyInfo)
        {
            return
                (propertyInfo?.GetCustomAttributes(typeof(DisplayLabelAttribute), false)?.SingleOrDefault() as DisplayLabelAttribute)?.DisplayLabel
                ?? propertyInfo?.Name.ToDisplayLabel()
                ;
        }

        public static string GetDisplayLabel(this Type type)
        {
            return
                (type?.GetCustomAttributes(typeof(DisplayLabelAttribute), false)?.SingleOrDefault() as DisplayLabelAttribute)?.DisplayLabel
                ?? type?.Name.ToDisplayLabel()
                ;
        }

        public static string[] GetAliases(this Type type)
        {
            return
                type
                ?.GetCustomAttributes(typeof(AliasAttribute), inherit: false)
                ?.SelectMany(
                    aliasAttr => (aliasAttr as AliasAttribute)?.Aliases
                )
                ?.Distinct()
                ?.ToArray()
                ;
        }

        public static string[] GetCategories(this Type type)
        {
            return
                type
                ?.GetCustomAttributes(typeof(CategoryAttribute), inherit: true)
                ?.SelectMany(
                    noteAttr => (noteAttr as CategoryAttribute)?.Categories
                )
                ?.Distinct()
                ?.ToArray()
                ;
        }

        public static string[] GetCategories(this PropertyInfo propertyInfo)
        {
            return
                propertyInfo
                ?.GetCustomAttributes(typeof(CategoryAttribute), inherit: true)
                ?.SelectMany(
                    noteAttr => (noteAttr as CategoryAttribute)?.Categories
                )
                ?.Distinct()
                ?.ToArray()
                ;
        }

        public static int GetPriority(this Type type)
        {
            return
                (
                    type
                    ?.GetCustomAttributes(typeof(PriorityAttribute), inherit: false)
                    ?.SingleOrDefault() as PriorityAttribute
                )
                ?.Priority
                ?? 0
                ;
        }

        public static int GetPriority(this PropertyInfo propertyInfo)
        {
            return
                (propertyInfo?.GetCustomAttributes(typeof(PriorityAttribute), false)?.SingleOrDefault() as PriorityAttribute)?.Priority
                ?? 0
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

        public static bool IsCoreAssembly(this Assembly assembly)
        {
            if (assembly?.FullName is null)
                return false;

            return assembly.FullName.In(coreAssemblyPrefixes, (name, prefix) => name.StartsWith(prefix, StringComparison.InvariantCulture));
        }

        public static Assembly[] GetNonCoreAssemblies(this AppDomain appDomain)
        {
            return
                appDomain
                .GetAssemblies()
                .Where(a => !a.IsCoreAssembly())
                .ToArray()
                ;
        }
    }
}
