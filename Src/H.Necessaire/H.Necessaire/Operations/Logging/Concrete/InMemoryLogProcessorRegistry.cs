using System.Linq;

namespace H.Necessaire.Operations.Logging.Concrete
{
    internal class InMemoryLogProcessorRegistry : ImALogProcessorRegistry, ImADependency
    {
        ImALogProcessor[] allKnownProcessors = new ImALogProcessor[0];
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            allKnownProcessors = typeof(ImALogProcessor).GetAllImplementations().Select(t => dependencyProvider.Get(t) as ImALogProcessor).Where(x => x != null).ToArray();
        }

        public ImALogProcessor[] GetAllKnownProcessors()
        {
            return allKnownProcessors;
        }

        public ImALogProcessorRegistry Register(ImALogProcessor logProcessor)
        {
            allKnownProcessors = allKnownProcessors.Push(logProcessor);
            return this;
        }
    }
}
