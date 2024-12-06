using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB
{
    public class GoogleFirestoreDbRuntimeDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;
        }
    }
}
