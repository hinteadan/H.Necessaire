using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using H.Necessaire.WebHooks.Implementations.SQLServer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks.Implementations.SQLServer
{
    class SqlServerWebHookRequestStore : DapperSqlResourceBase, IWebHookRequestStorage
    {
        #region Construct
        public SqlServerWebHookRequestStore(string connectionString)
            : base(connectionString, tableName: "WebHookRequest")
        { }

        protected override Task<SqlMigration[]> GetAllMigrations() => new SqlMigration[0].AsTask();
        #endregion

        public async Task Append(IWebHookRequest request)
        {
            WebHookRequestFullSqlEntry sqlEntry = await Map(request);

            await SaveEntity(sqlEntry);
        }

        public async Task<Page<IWebHookRequest>> Browse(WebHookRequestFilter filter)
        {
            List<ISqlFilterCriteria> sqlFilterCriterias = ConstructSqlFilterCriteria(filter, out DynamicParameters parameters);

            ILimitedEnumerable<WebHookRequestLightSqlEntry> sqlEntities =
                await LoadEntitiesByCustomCriteria<WebHookRequestLightSqlEntry>(
                    sqlFilterCriterias.ToArray(),
                    parameters,
                    filter.SortFilters?.Any() ?? false ? filter.SortFilters.Select(x => new SqlSortCriteria { ColumnName = x.By, SortDirection = x.Direction == SortFilter.SortDirection.Ascending ? "ASC" : "DESC" }).ToArray() : null,
                    filter.PageFilter == null ? null : new SqlLimitCriteria { Offset = filter.PageFilter.PageIndex * filter.PageFilter.PageSize, Count = filter.PageFilter.PageSize }
                );

            if (!sqlEntities?.Any() ?? true)
                return Page<IWebHookRequest>.Empty();

            return
                new Page<IWebHookRequest>
                {
                    Content = sqlEntities.Select(ProjectLightSqlEntry).ToArray(),
                    PageIndex = filter.PageFilter?.PageIndex ?? 0,
                    PageSize = filter.PageFilter?.PageSize ?? sqlEntities.Length,
                    TotalNumberOfPages = (filter.PageFilter?.PageSize ?? 0) > 0
                        ? (int)((sqlEntities.TotalNumberOfItems / filter.PageFilter.PageSize) + (sqlEntities.TotalNumberOfItems % filter.PageFilter.PageSize == 0 ? 0 : 1))
                        : 1,
                };
        }

        public async Task<IDisposableEnumerable<IWebHookRequest>> StreamAll()
        {
            return
                await StreamAll<WebHookRequestLightSqlEntry, IWebHookRequest>(ProjectLightSqlEntry);
        }

        async Task<WebHookRequestFullSqlEntry> Map(IWebHookRequest request)
        {
            return
                new WebHookRequestFullSqlEntry
                {
                    ID = request.ID,
                    HappenedAt = request.HappenedAt,
                    HandlingHost = request.HandlingHost,
                    Source = request.Source,
                    MetaJson = request.Meta.ToJsonArray(),
                    PayloadJson = (await request.GetPayload<object>()).ToJsonObject(),
                };
        }

        DeleagatedJsonPayloadWebHookRequest ProjectLightSqlEntry(WebHookRequestLightSqlEntry sqlEntry)
        {
            return
                new DeleagatedJsonPayloadWebHookRequest(LoadWebHookRequestJsonPayload)
                {
                    ID = sqlEntry.ID,
                    HappenedAt = sqlEntry.HappenedAt,
                    Meta = sqlEntry.MetaJson.DeserializeToNotes(),
                    Source = sqlEntry.Source,
                    HandlingHost = sqlEntry.HandlingHost,
                };
        }

        async Task<string> LoadWebHookRequestJsonPayload(IWebHookRequest webHookRequest)
        {
            return
                (await LoadEntityByID<WebHookRequestFullSqlEntry>(webHookRequest.ID))
                ?.PayloadJson;
        }

        static List<ISqlFilterCriteria> ConstructSqlFilterCriteria(WebHookRequestFilter filter, out DynamicParameters parameters)
        {
            List<ISqlFilterCriteria> sqlFilterCriterias = new List<ISqlFilterCriteria>();
            parameters = new DynamicParameters();

            if (filter.IDs != null)
            {
                sqlFilterCriterias.Add(new SqlFilterCriteria(nameof(WebHookRequestLightSqlEntry.ID), nameof(filter.IDs), "IN"));
                parameters.AddDynamicParams(new { filter.IDs });
            }

            if (filter.Sources != null)
            {
                sqlFilterCriterias.Add(new SqlFilterCriteria(nameof(WebHookRequestLightSqlEntry.Source), nameof(filter.Sources), "IN"));
                parameters.AddDynamicParams(new { filter.Sources });
            }

            if (filter.HandlingHosts != null)
            {
                sqlFilterCriterias.Add(new SqlFilterCriteria(nameof(WebHookRequestLightSqlEntry.HandlingHost), nameof(filter.HandlingHosts), "IN"));
                parameters.AddDynamicParams(new { filter.HandlingHosts });
            }

            if (filter.FromInclusive != null)
            {
                sqlFilterCriterias.Add(new SqlFilterCriteria(nameof(WebHookRequestLightSqlEntry.HappenedAt), nameof(filter.FromInclusive), ">="));
                parameters.AddDynamicParams(new { filter.FromInclusive });
            }

            if (filter.ToInclusive != null)
            {
                sqlFilterCriterias.Add(new SqlFilterCriteria(nameof(WebHookRequestLightSqlEntry.HappenedAt), nameof(filter.ToInclusive), "<="));
                parameters.AddDynamicParams(new { filter.ToInclusive });
            }

            return sqlFilterCriterias;
        }
    }
}
