namespace H.Necessaire.Runtime.Sqlite.Core.Resources.Auditing
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<AuditMetadataSqliteRsx>(() => new AuditMetadataSqliteRsx())
                .Register<AuditPayloadSqliteRsx>(() => new AuditPayloadSqliteRsx())
                .Register<AuditSqliteRsx>(() => new AuditSqliteRsx())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditSqliteRsx>())
                ;
        }
    }
}
