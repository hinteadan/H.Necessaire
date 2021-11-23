using H.Necessaire.Runtime.Sync.Processors;

namespace H.Necessaire.Runtime.Sync
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ConsumerIdentityProcessor>(() => new ConsumerIdentityProcessor())
                .Register<LogEntryProcessor>(() => new LogEntryProcessor())
                ;
        }
    }
}
