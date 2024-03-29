﻿namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HasherFactory>(() => new HasherFactory())
                ;

            dependencyRegistry.Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup());

            dependencyRegistry.Register<CoreDependencyGroup>(() => new CoreDependencyGroup());

            dependencyRegistry.Register<ResourcesDependencyGroup>(() => new ResourcesDependencyGroup());
            dependencyRegistry.Register<ManagersDependencyGroup>(() => new ManagersDependencyGroup());

            dependencyRegistry.Register<Logging.DependencyGroup>(() => new Logging.DependencyGroup());
        }
        #endregion
    }
}
