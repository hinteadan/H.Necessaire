using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources
{
    internal class LogEntrySqliteRsx
        : DapperSqliteStorageResourceBase<
            Guid,
            LogEntry,
            LogEntrySqliteRsx.LogEntrySqlEntry,
            LogFilter
        >
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        public LogEntrySqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.Log", databaseName: "H.Necessaire.Core") { }
        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = nameof(LogEntry),
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
                await new SqlMigration{
                    ResourceIdentifier = nameof(LogEntry),
                    VersionNumber = new VersionNumber(1, 1),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(LogFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.Levels?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.Level), parameterName: nameof(filter.Levels), @operator: "IN"));
            }

            if (filter?.ScopeIDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.ScopeID), parameterName: nameof(filter.ScopeIDs), @operator: "IN"));
            }

            if (filter?.Methods?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.Method), parameterName: nameof(filter.Methods), @operator: "IN"));
            }

            if (filter?.Components?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.Component), parameterName: nameof(filter.Components), @operator: "IN"));
            }

            if (filter?.Applications?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.Application), parameterName: nameof(filter.Applications), @operator: "IN"));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(LogEntrySqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }

        public class LogEntrySqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public LogEntryLevel Level { get; set; }
            public string LevelLabel { get; set; }
            public Guid ScopeID { get; set; }
            public string OperationContextJson { get; set; }
            public DateTime HappenedAt { get; set; }
            public long HappenedAtTicks { get; set; }
            public string Message { get; set; } = null;
            public string Method { get; set; }
            public string StackTrace { get; set; }
            public string Component { get; set; }
            public string Application { get; set; }
            public string ExceptionJson { get; set; }
            public string PayloadJson { get; set; }
            public string NotesJson { get; set; }
            public string AppVersionJson { get; set; }
            public string AppVersionNumber { get; set; }
            public DateTime? AppVersionTimestamp { get; set; }
            public string AppVersionBranch { get; set; }
            public string AppVersionCommit { get; set; }
        }

        public class LogEntrySqlEntityMapper : SqlEntityMapperBase<LogEntry, LogEntrySqlEntry>
        {
            static LogEntrySqlEntityMapper() => new LogEntrySqlEntityMapper().RegisterMapper();

            public override LogEntrySqlEntry MapEntityToSql(LogEntry entity)
            {
                return
                    base
                    .MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.LevelLabel = entity.Level.ToString();
                        x.OperationContextJson = entity.OperationContext?.ToJsonObject();
                        x.HappenedAtTicks = entity.HappenedAt.EnsureUtc().Ticks;
                        x.ExceptionJson = entity.Exception?.ToJsonObject();
                        x.PayloadJson = entity.Payload?.ToJsonObject();
                        x.NotesJson = entity.Notes?.ToJsonArray();
                        x.AppVersionJson = entity.AppVersion?.ToJsonObject();
                        x.AppVersionNumber = entity.AppVersion?.Number?.ToString();
                        x.AppVersionTimestamp = entity.AppVersion?.Timestamp;
                        x.AppVersionBranch = entity.AppVersion?.Branch;
                        x.AppVersionCommit = entity.AppVersion?.Commit;
                    })
                    ;
            }

            public override LogEntry MapSqlToEntity(LogEntrySqlEntry sqlEntity)
            {
                return
                    base
                    .MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.OperationContext = sqlEntity.OperationContextJson?.JsonToObject<OperationContext>();
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks, DateTimeKind.Utc);
                        x.Exception = sqlEntity.ExceptionJson?.JsonToObject<Exception>();
                        x.Payload = sqlEntity.PayloadJson;
                        x.Notes = sqlEntity.NotesJson?.DeserializeToNotes();
                        x.AppVersion = sqlEntity.AppVersionJson?.JsonToObject<Version>();
                    })
                    ;
            }
        }
    }
}
