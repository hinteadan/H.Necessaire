namespace H.Necessaire.Operations.Concrete
{
    internal class LimitedConcurrencyRunnerFactory : ImALimitedConcurrencyRunnerFactory, ImADependency
    {
        ImADependencyProvider dependencyProvider;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            this.dependencyProvider = dependencyProvider;
        }

        public ImALimitedConcurrencyRunner New()
            => new LimitedConcurrencyRunner().And(x => x.ReferDependencies(dependencyProvider));
        public ImALimitedConcurrencyRunner New(int maxConcurrency = 150)
            => new LimitedConcurrencyRunner(maxConcurrency).And(x => x.ReferDependencies(dependencyProvider));
    }
}
