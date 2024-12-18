namespace H.Necessaire.Runtime.CLI.Common
{
    internal static class IoCExtensions
    {
        public static T AddCliCommons<T>(this T ioc) where T : ImADependencyRegistry
        {
            ioc
                .Register<CustomCommandRunner>(() => new CustomCommandRunner())
                .Register<ArgsParser>(() => new ArgsParser())

                .Register<CliUseCaseContextProvider>(() => new CliUseCaseContextProvider())

                .Register<CustomizableCliContextProvider>(() => new CustomizableCliContextProvider(ioc.Get<CliUseCaseContextProvider>()))

                .Register<ImAUseCaseContextProvider>(() => ioc.Get<CustomizableCliContextProvider>())

                .Register<CliCommandFactory>(() => new CliCommandFactory())
                ;


            return ioc;
        }

        public static T WithCliCommons<T>(this T wireup) where T : ImAnApiWireup
        {
            wireup
                .With(x => x.AddCliCommons())
                ;

            return wireup;
        }
    }
}
