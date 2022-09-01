using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer
{
    public class SqlServerRuntimeDependencyGroup : DapperDependencyGroup
    {
        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            base.RegisterDependencies(dependencyRegistry);

            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;

        }
    }
}
