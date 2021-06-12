namespace H.Necessaire.Testicles.Unit.Model.IoC
{
    class ComposedDependency : ImADependency
    {
        public PureDependency PureDependency { get; private set; }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            PureDependency = dependencyProvider.Get<PureDependency>();
        }
    }
}
