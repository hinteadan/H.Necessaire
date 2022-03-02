using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class ConsumerIdentitySqlServerStorageResource : DapperStorageServiceBase<Guid, ConsumerIdentity, ConsumerIdentitySqlServerStorageResource.ConsumerIdentitySqlEntry, IDFilter<Guid>>
    {
        #region Construct
        public ConsumerIdentitySqlServerStorageResource() : base(connectionString: null, tableName: $"H.Necessaire.{nameof(ConsumerIdentity)}", databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(IDFilter<Guid> filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(ConsumerIdentitySqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            return result.ToArray();
        }

        public class ConsumerIdentitySqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }

            public string IDTag { get; set; }

            public string DisplayName { get; set; }

            public string NotesJson { get; set; }

            public DateTime AsOf { get; set; }

            public long AsOfTicks { get; set; }

            public string IpAddress { get; set; }
            public string HostName { get; set; }
            public string Protocol { get; set; }
            public string UserAgent { get; set; }
            public string AiUserID { get; set; }
            public string Origin { get; set; }
            public string Referer { get; set; }
            public string RuntimePlatformJson { get; set; }
        }

        public class ConsumerIdentitySqlEntityMapper : SqlEntityMapperBase<ConsumerIdentity, ConsumerIdentitySqlEntry>
        {
            static ConsumerIdentitySqlEntityMapper() => new ConsumerIdentitySqlEntityMapper().RegisterMapper();

            public override ConsumerIdentity MapSqlToEntity(ConsumerIdentitySqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.Notes = sqlEntity.NotesJson.DeserializeToNotes();
                        x.AsOf = new DateTime(sqlEntity.AsOfTicks);
                        x.RuntimePlatform = sqlEntity.RuntimePlatformJson.JsonToObject<ConsumerPlatformInfo>();
                    });
            }

            public override ConsumerIdentitySqlEntry MapEntityToSql(ConsumerIdentity entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.NotesJson = entity.Notes.ToJsonArray();
                        x.AsOfTicks = entity.AsOf.Ticks;
                        x.RuntimePlatformJson = entity.RuntimePlatform.ToJsonObject();
                    })
                    ;
            }
        }
    }
}
