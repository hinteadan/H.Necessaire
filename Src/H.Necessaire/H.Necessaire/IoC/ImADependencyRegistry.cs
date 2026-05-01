using System;

namespace H.Necessaire
{
    public interface ImADependencyRegistry : ImADependencyProvider, ImADependencyBrowser
    {
        ImADependencyRegistry Register<T>(Func<object> factory);
        ImADependencyRegistry Register<T>(Func<Type, object> factory);
        ImADependencyRegistry Register(Type type, Func<object> factory);
        ImADependencyRegistry Register(Type type, Func<Type, object> typedFactory);

        ImADependencyRegistry RegisterAlwaysNew<T>(Func<object> factory);
        ImADependencyRegistry RegisterAlwaysNew<T>(Func<Type, object> factory);
        ImADependencyRegistry RegisterAlwaysNew(Type type, Func<object> factory);
        ImADependencyRegistry RegisterAlwaysNew(Type type, Func<Type, object> typedFactory);

        ImADependencyRegistry Unregister<T>();
        ImADependencyRegistry Unregister(Type type);
    }
}