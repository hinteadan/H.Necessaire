using H.Necessaire.Runtime.CLI.UI.Concrete;

namespace H.Necessaire.Runtime.CLI.UI
{
    internal static class IoCExtensions
    {
        public static T AddCliUI<T>(this T ioc) where T : ImADependencyRegistry
        {
            ioc
                .Register<Concrete.Logging.DependencyGroup>(() => new Concrete.Logging.DependencyGroup())

                .Register<ImACliUI>(() => CliUI.Shared)
                .Register<ImACliUiProgressIndicator>(() => CliUI.Shared)
                .Register<ImACliUiStatusIndicator>(() => CliUI.Shared)

                ;
            return ioc;
        }

        public static T WithCliUI<T>(this T wireup) where T : ImAnApiWireup
        {
            wireup
                .With(x => x.AddCliUI())
                ;

            return wireup;
        }
    }
}
