using System;

namespace H.Necessaire
{
    public interface ImADependencyRegistry : ImADependencyProvider, ImADependencyBrowser
    {
        ImADependencyRegistry Register<T>(Func<object> factory);
        ImADependencyRegistry Register(Type type, Func<object> factory);

        ImADependencyRegistry RegisterAlwaysNew<T>(Func<object> factory);
        ImADependencyRegistry RegisterAlwaysNew(Type type, Func<object> factory);

        ImADependencyRegistry Unregister<T>();
        ImADependencyRegistry Unregister(Type type);
    }
}