using System;

namespace H.Necessaire.Runtime.Wireup.Abstracts
{
    public abstract class ApiWireupBase : ImAnApiWireup
    {
        #region Construct
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();
        #endregion

        public ImADependencyRegistry DependencyRegistry => dependencyRegistry;

        public ImAnApiWireup With(Action<ImADependencyRegistry> registry)
        {
            registry?.Invoke(dependencyRegistry);
            return this;
        }

        public virtual ImAnApiWireup WithEverything()
        {
            return
                this
                .With(x => x.Register<RuntimeDependencyGroup>(() => new RuntimeDependencyGroup()))
                ;
        }
    }
}
