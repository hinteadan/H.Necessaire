using H.Necessaire.Runtime;
using H.Necessaire.Runtime.CLI;

namespace H.Necessaire.CLI
{
    class App : CliApp
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                //.With(x => x.Register<Runtime.SqlServer.SqlServerRuntimeDependencyGroup>(() => new Runtime.SqlServer.SqlServerRuntimeDependencyGroup()))
                ;
        }
    }
}
