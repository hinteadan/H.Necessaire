namespace H.Necessaire.CLI.Commands.HDoc.BLL
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HDocCsProjParser>(() => new HDocCsProjParser())
                ;
        }
    }
}
