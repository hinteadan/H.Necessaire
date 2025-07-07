using System;

namespace H.Necessaire
{
    public interface ImADependencyProvider
    {
        T Get<T>();
        object Get(Type type);

        bool HasTypeRegistered(Type type);
        bool HasTypeRegistered<T>();
    }
}
