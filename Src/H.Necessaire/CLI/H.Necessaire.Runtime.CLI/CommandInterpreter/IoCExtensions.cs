namespace H.Necessaire.Runtime.CLI.CommandInterpreter
{
    internal static class IoCExtensions
    {
        public static T WithCliInterpreter<T>(this T iocRegistry) where T : ImADependencyRegistry
        {
            iocRegistry.Register<CliConsoleInteractor>(() => new CliConsoleInteractor());
            return iocRegistry;
        }
    }
}
