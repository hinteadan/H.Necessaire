namespace H.Necessaire.Runtime.Azure.CosmosDB
{
    public class AzureCosmosDbRuntimeDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;
        }
    }
}
