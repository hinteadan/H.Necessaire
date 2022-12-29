using H.Necessaire.Runtime;

namespace H.Necessaire.CLI
{
    class App : CliApp
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                .WithDefaultRuntimeConfig()
                //.With(x => x.Register<Runtime.SqlServer.SqlServerRuntimeDependencyGroup>(() => new Runtime.SqlServer.SqlServerRuntimeDependencyGroup()))
                //.With(x => x.Register<Runtime.RavenDB.RavenDbRuntimeDependencyGroup>(() => new Runtime.RavenDB.RavenDbRuntimeDependencyGroup()))
                ;
        }
    }
}
