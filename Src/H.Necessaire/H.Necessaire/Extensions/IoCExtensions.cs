using System;
using System.Linq;

namespace H.Necessaire
{
    public static class IoCExtensions
    {
        public static T Build<T>(this ImADependencyProvider dependencyProvider, string identifier, T defaultTo = default(T))
            where T : class
        {
            Type concreteType
                = typeof(T)
                .GetAllImplementations()
                .FirstOrDefault(x => x.IsMatch(identifier))
                ;

            if (concreteType == null)
                return defaultTo;

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
