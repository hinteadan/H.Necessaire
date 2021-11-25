namespace H.Necessaire.Operations.QdAction
{
    internal class DependenctGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAnActionQer>(() => new ActionQer())
                .Register<QdActionProcessingDaemon>(() => new QdActionProcessingDaemon())
                ;
        }
    }
}
