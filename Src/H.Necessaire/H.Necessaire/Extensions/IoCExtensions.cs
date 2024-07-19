using System;
using System.Linq;

namespace H.Necessaire
{
    public static class IoCExtensions
    {
        public static T Build<T>(this ImADependencyProvider dependencyProvider, string identifier, T defaultTo = default(T))
            where T : class
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return defaultTo;

            Type[] matchedTypes
                = typeof(T)
                .GetAllImplementations()
                .Where(x => x.IsMatch(identifier))
                .ToArray()
                ;

            if (!matchedTypes.Any())
                return defaultTo;

            Type concreteType
                = matchedTypes
                .OrderBy(x => 
                    x.IsTypeIDMatch(identifier)
                    ? 0
                    : x.IsTypeAliasMatch(identifier)
                    ? 1
                    : 2
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
