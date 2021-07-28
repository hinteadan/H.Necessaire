using System;

namespace H.Necessaire.Runtime
{
    public interface ImAnApiWireup
    {
        ImADependencyRegistry DependencyRegistry { get; }

        ImAnApiWireup With(Action<ImADependencyRegistry> registry);

        ImAnApiWireup WithEverything();
    }
}
