using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImAnAppWireup
    {
        ImADependencyRegistry DependencyRegistry { get; }

        ImAnAppWireup With(Action<ImADependencyRegistry> registry);

        ImAnAppWireup WithEverything();

        Task<ImAnAppWireup> Boot();
    }
}
