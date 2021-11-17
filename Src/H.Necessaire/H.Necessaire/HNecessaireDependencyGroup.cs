namespace H.Necessaire
{
    public class HNecessaireDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ImAPeriodicAction>(() => ConcreteFactory.BuildNewPeriodicAction());

            dependencyRegistry.Register<SyncDependencyGroup>(() => new SyncDependencyGroup());
        }
    }
}
