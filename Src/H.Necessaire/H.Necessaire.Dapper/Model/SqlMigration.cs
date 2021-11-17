using System;

namespace H.Necessaire.Dapper
{
    public class SqlMigration : IStringIdentity
    {
        public string ID => $"{ResourceIdentifier}-v{VersionNumber}";

        public string ResourceIdentifier { get; set; }

        public VersionNumber VersionNumber { get; set; }

        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

        public long HappenedAtTicks => HappenedAt.Ticks;

        public string SqlCommand { get; set; }

        public override string ToString() => ID;
    }

    internal class SqlMigrationSqlEntry : SqlEntryBase
    {
        public string ID => $"{ResourceIdentifier}-v{VersionNumber}";

        public string ResourceIdentifier { get; set; }

        public string VersionNumber { get; set; }

        public DateTime HappenedAt => new DateTime(HappenedAtTicks);

        public long HappenedAtTicks { get; set; }

        public string SqlCommand { get; set; }
    }

    internal class SqlMigrationMapper : SqlEntityMapperBase<SqlMigration, SqlMigrationSqlEntry>
    {
        static SqlMigrationMapper() => new SqlMigrationMapper().RegisterMapper();

        public override SqlMigrationSqlEntry MapEntityToSql(SqlMigration entity)
        {
            SqlMigrationSqlEntry result = base.MapEntityToSql(entity);

            result.VersionNumber = entity.VersionNumber.ToString();
            result.HappenedAtTicks = entity.HappenedAtTicks;

            return result;
        }

        public override SqlMigration MapSqlToEntity(SqlMigrationSqlEntry sqlEntity)
        {
            SqlMigration result = base.MapSqlToEntity(sqlEntity);

            result.VersionNumber = VersionNumber.Parse(sqlEntity.VersionNumber);
            result.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks);

            return result;
        }
    }
}
