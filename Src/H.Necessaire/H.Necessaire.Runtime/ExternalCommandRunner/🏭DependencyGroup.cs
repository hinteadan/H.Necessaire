namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<ArgsBuilder>(() => new ArgsBuilder())

                .Register<ExternalCommandRunner>(() => new ExternalCommandRunner())
                .Register<ImAnExternalCommandRunner>(() => dependencyRegistry.Get<ExternalCommandRunner>())
                .Register<ImAContextualExternalCommandRunnerFactory>(() => dependencyRegistry.Get<ExternalCommandRunner>())
                ;

        }
    }
}
