using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public static class IoCExtensions
    {
        public static T Build<T>(this ImADependencyProvider dependencyProvider, string identifier, T defaultTo = default(T), params Assembly[] assembliesToScan)
            where T : class
        {
            if (identifier.IsEmpty())
                return defaultTo;

            Type[] matchedTypes = typeof(T).FindMatchingConcreteTypes(identifier, assembliesToScan).ToArray();

            if (!matchedTypes.Any())
                return defaultTo;

            return dependencyProvider.Build(identifier, matchedTypes, defaultTo);
        }

        public static T Build<T>(this ImADependencyProvider dependencyProvider, string identifier, IEnumerable<Type> concreteTypesToScan, T defaultTo = default(T))
            where T : class
        {
            Type[] matchedTypes
                = concreteTypesToScan
                ?.Where(x => !x.IsAbstract)
                .ToArray()
                ;

            if (matchedTypes?.Any() != true)
                return defaultTo;

            Type concreteType
                = matchedTypes
                .OrderBy(x =>
                    x.IsTypeIDMatch(identifier)
                    ? 0
                    : x.IsTypeAliasMatch(identifier)
                    ? 1
                    : x.IsTypeNameMatch(identifier)
                    ? 2
                    : Math.Max(3, x.Name.Length - identifier.Length)
                )
                .First()
                ;

            return
                (dependencyProvider.Get(concreteType) as T)
                ??
                (Activator.CreateInstance(concreteType) as T)
                ??
                defaultTo
                ;
        }
    }
}
