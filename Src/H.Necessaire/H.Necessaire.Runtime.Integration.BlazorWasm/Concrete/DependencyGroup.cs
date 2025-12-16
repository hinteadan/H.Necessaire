using System;

namespace H.Necessaire.Runtime.Integration.BlazorWasm.Concrete
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HttpApiStorageService<Guid, ConsumerIdentity>>(() => new HttpApiStorageService<Guid, ConsumerIdentity>())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(dependencyRegistry.Get<HttpApiStorageService<Guid, ConsumerIdentity>>)
                ;
        }
    }
}
