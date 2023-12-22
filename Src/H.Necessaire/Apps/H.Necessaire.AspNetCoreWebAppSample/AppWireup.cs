using H.Necessaire.Runtime;
using H.Necessaire.Runtime.Integration.NetCore;
using H.Necessaire.Runtime.SqlServer;
using H.Necessaire.Runtime.RavenDB;
using H.Necessaire.Runtime.Azure.CosmosDB;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    public class AppWireup : NetCoreApiWireupBase
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                //.With(x => x.Register<SqlServerRuntimeDependencyGroup>(() => new SqlServerRuntimeDependencyGroup()))
                //.With(x => x.Register<RavenDbRuntimeDependencyGroup>(() => new RavenDbRuntimeDependencyGroup()))
                .With(x => x.Register<AzureCosmosDbRuntimeDependencyGroup>(() => new AzureCosmosDbRuntimeDependencyGroup()))
                ;
        }
    }
}
