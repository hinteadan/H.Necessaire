using System.Runtime.InteropServices;

namespace H.Necessaire.Runtime.UI.Razor.Core.Managers
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        { 
            dependencyRegistry
                .AndIf(RuntimeInformation.ProcessArchitecture != Architecture.Wasm, x => x.RegisterAlwaysNew<ConsumerManager>(() => new ConsumerManager()))
                .AndIf(RuntimeInformation.ProcessArchitecture == Architecture.Wasm, x => x.Register<ConsumerManager>(() => new ConsumerManager()))

                .Register<Func<Task<ConsumerIdentity>>>(() => dependencyRegistry.Get<ConsumerManager>().GetCurrentConsumer())

                ;
        }
    }
}
