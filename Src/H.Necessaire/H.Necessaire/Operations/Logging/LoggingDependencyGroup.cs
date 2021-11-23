using H.Necessaire.Operations.Logging.Concrete;

namespace H.Necessaire
{
    internal class LoggingDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ConsoleLogProcessor>(() => new ConsoleLogProcessor())
                ;

            dependencyRegistry
                .RegisterAlwaysNew<InMemoryLogger>(() => new InMemoryLogger())
                .Register<ImALogProcessorRegistry>(() => new InMemoryLogProcessorRegistry())
                .Register<ImALoggerFactory>(() => new LoggerFactory())
                ;
        }
    }
}
