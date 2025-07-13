﻿using H.Necessaire.Operations;
using H.Necessaire.Operations.Versioning;
using H.Necessaire.Runtime.MAUI.Core;

namespace H.Necessaire
{
    public class HNecessaireDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<ImAPeriodicAction>(() => ConcreteFactory.BuildNewPeriodicAction())
                .RegisterAlwaysNew<ImAHealthChecker>(() => new HealthChecker())
                ;

            dependencyRegistry
                .Register<CachingDependencyGroup>(() => new CachingDependencyGroup())
                .Register<SyncDependencyGroup>(() => new SyncDependencyGroup())
                .Register<LoggingDependencyGroup>(() => new LoggingDependencyGroup())
                .Register<VersioningDependencyGroup>(() => new VersioningDependencyGroup())
                .Register<ImAConnectivityInfoProvider>(() => new HsConnectivityInfoProvider())
                .Register<Operations.QdAction.DependenctGroup>(() => new Operations.QdAction.DependenctGroup())
                ;
        }
    }
}
