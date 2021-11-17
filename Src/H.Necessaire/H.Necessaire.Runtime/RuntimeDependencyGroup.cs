﻿namespace H.Necessaire.Runtime
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HasherFactory>(() => new HasherFactory())
                ;

            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                ;

            dependencyRegistry
                .Register<Resources.DependencyGroup>(() => new Resources.DependencyGroup())
                .Register<Validation.DependencyGroup>(() => new Validation.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Sync.DependencyGroup>(() => new Sync.DependencyGroup())
                .Register<UseCases.DependencyGroup>(() => new UseCases.DependencyGroup())
                .Register<Daemons.DependencyGroup>(() => new Daemons.DependencyGroup())
                ;
        }
        #endregion
    }
}
