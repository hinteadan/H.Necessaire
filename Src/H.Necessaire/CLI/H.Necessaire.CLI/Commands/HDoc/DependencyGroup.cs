namespace H.Necessaire.CLI.Commands.HDoc
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<BLL.DependencyGroup>(() => new BLL.DependencyGroup())
                .Register<HDocManager>(() => new HDocManager())
                ;
        }
    }
}
