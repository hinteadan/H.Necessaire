namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    public class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<NuSpecParser>(() => new NuSpecParser());
            dependencyRegistry.Register<NuSpecDependencyTreeProcessor>(() => new NuSpecDependencyTreeProcessor());
            dependencyRegistry.Register<NuSpecVersionProcessor>(() => new NuSpecVersionProcessor());
            dependencyRegistry.Register<NuSpecFileUpdater>(() => new NuSpecFileUpdater());
        }
    }
}