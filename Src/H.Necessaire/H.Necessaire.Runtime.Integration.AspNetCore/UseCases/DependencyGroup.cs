namespace H.Necessaire.Runtime.Integration.AspNetCore.UseCases
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<StorageServiceHttpApiUseCase>(() => new StorageServiceHttpApiUseCase())
                .RegisterAlwaysNew<QdActionHttpApiUseCase>(() => new QdActionHttpApiUseCase())
                ;
        }
    }
}
