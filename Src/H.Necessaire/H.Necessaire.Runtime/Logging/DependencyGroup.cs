﻿namespace H.Necessaire.Runtime.Logging
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<PersistentLogProcessor>(() => new PersistentLogProcessor())
                ;
        }
    }
}
