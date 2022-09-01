using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.RavenDB.Analytics
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Resources.DependencyGroup>(() => new Resources.DependencyGroup())
                ;
        }
    }
}
