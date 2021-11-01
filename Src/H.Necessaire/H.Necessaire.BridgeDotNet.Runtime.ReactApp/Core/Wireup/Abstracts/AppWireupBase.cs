using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class AppWireupBase : ImAnAppWireup
    {
        #region Construct
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();
        #endregion

        public ImADependencyRegistry DependencyRegistry => dependencyRegistry;

        public ImAnAppWireup With(Action<ImADependencyRegistry> registry)
        {
            registry?.Invoke(dependencyRegistry);
            return this;
        }

        public virtual ImAnAppWireup WithEverything()
        {
            return
                this
                .With(x => x.Register<RuntimeDependencyGroup>(() => new RuntimeDependencyGroup()))
                ;
        }

        public virtual Task<ImAnAppWireup> Boot()
        {
            return (this as ImAnAppWireup).AsTask();
        }
    }
}
