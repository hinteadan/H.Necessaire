using System;

namespace H.Necessaire
{
    public interface ImADependencyProvider
    {
        T Get<T>();
        object Get(Type type);
    }
}
