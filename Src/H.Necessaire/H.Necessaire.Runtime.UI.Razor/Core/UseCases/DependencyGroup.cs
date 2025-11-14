namespace H.Necessaire.Runtime.UI.Razor.Core.UseCases
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<ImAConsumerUseCase>(() => new ConsumerUseCase())
                ;
        }
    }
}
