using System;

namespace H.Necessaire.Runtime.CLI.UI.Concrete.Logging
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            Type consoleLogProcessorType = Type.GetType("H.Necessaire.ConsoleLogProcessor, H.Necessaire");
            ImALogProcessor consoleLogProcessor = dependencyRegistry.Get(consoleLogProcessorType) as ImALogProcessor;
            dependencyRegistry.Unregister(consoleLogProcessorType);

            dependencyRegistry.Register<CliUiConsoleLogProcessor>(() => new CliUiConsoleLogProcessor(consoleLogProcessor));
        }
    }
}
