using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class RuntimeTraceSqlServerStorageResource : DapperStorageServiceBase<Guid, RuntimeTrace, RuntimeTraceSqlServerStorageResource.RuntimeTraceSqlEntry, IDFilter<Guid>>
    {
        #region Construct
        public RuntimeTraceSqlServerStorageResource() : base(connectionString: null, tableName: runtimeTraceTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(IDFilter<Guid> filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(NetworkTrace.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            return result.ToArray();
        }

        public class RuntimeTraceSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public Guid? ConsumerIdentityID { get; set; }

            public DateTime AsOf { get; set; }
            public long AsOfTicks { get; set; }

            public string RuntimeTraceProviderJson { get; set; }

            public string DeviceJson { get; set; }
            public string OperatingSystemJson { get; set; }
            public string ClientJson { get; set; }
            public string BrowserJson { get; set; }
            public string BotJson { get; set; }
            public string TimingJson { get; set; }

            public string NotesJson { get; set; }
        }

        public class RuntimeTraceSqlEntryMapper : SqlEntityMapperBase<RuntimeTrace, RuntimeTraceSqlEntry>
        {
            static RuntimeTraceSqlEntryMapper() => new RuntimeTraceSqlEntryMapper().RegisterMapper();

            public override RuntimeTraceSqlEntry MapEntityToSql(RuntimeTrace entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.AsOfTicks = entity.AsOf.Ticks;
                        x.RuntimeTraceProviderJson = entity.RuntimeTraceProvider.ToJsonObject();
                        x.DeviceJson = entity.Device.ToJsonObject();
                        x.OperatingSystemJson = entity.OperatingSystem.ToJsonObject();
                        x.ClientJson = entity.Client.ToJsonObject();
                        x.BrowserJson = entity.Browser.ToJsonObject();
                        x.BotJson = entity.Bot.ToJsonObject();
                        x.TimingJson = entity.Timing.ToJsonObject();
                        x.NotesJson = entity.Notes.ToJsonArray();
                    })
                    ;
            }

            public override RuntimeTrace MapSqlToEntity(RuntimeTraceSqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.AsOf = new DateTime(sqlEntity.AsOfTicks);
                        x.RuntimeTraceProvider = sqlEntity.RuntimeTraceProviderJson.JsonToObject<InternalIdentity>();
                        x.Device = sqlEntity.DeviceJson.JsonToObject<RuntimeDeviceInfo>();
                        x.OperatingSystem = sqlEntity.OperatingSystemJson.JsonToObject<RuntimeOperatingSystemInfo>();
                        x.Client = sqlEntity.ClientJson.JsonToObject<RuntimeClientInfo>();
                        x.Browser = sqlEntity.BrowserJson.JsonToObject<RuntimeBrowserAppInfoInfo>();
                        x.Bot = sqlEntity.BotJson.JsonToObject<RuntimeBotInfo>();
                        x.Timing = sqlEntity.TimingJson.JsonToObject<RuntimeTimingInfo>();
                        x.Notes = sqlEntity.NotesJson.JsonToObject<Note[]>();
                    })
                    ;
            }
        }
    }
}
