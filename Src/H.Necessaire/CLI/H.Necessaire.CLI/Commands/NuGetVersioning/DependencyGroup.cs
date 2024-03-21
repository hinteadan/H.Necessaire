namespace H.Necessaire.CLI.Commands.NuGetVersioning
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<NuSpecParser>(() => new NuSpecParser())
                .Register<CsprojParser>(() => new CsprojParser())
                .Register<NuSpecDependencyTreeProcessor>(() => new NuSpecDependencyTreeProcessor())
                .Register<NuSpecVersionProcessor>(() => new NuSpecVersionProcessor())
                .Register<CsprojNugetVersionProcessor>(() => new CsprojNugetVersionProcessor())
                .Register<NuSpecFileUpdater>(() => new NuSpecFileUpdater())
                ;
        }
    }
}