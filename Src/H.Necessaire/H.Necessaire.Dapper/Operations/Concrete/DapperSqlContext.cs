using Dapper;
using H.Necessaire;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    class DapperSqlContext : IDisposable
    {
        #region Construct
        readonly IDbConnection dbConnection;
        readonly string defaultTableName;
        public DapperSqlContext(string sqlConnectionString, string defaultTableName = null)
        {
            this.defaultTableName = defaultTableName;
            dbConnection = new SqlConnection(sqlConnectionString);
        }

        public void Dispose()
        {
            new Action(dbConnection.Dispose).TryOrFailWithGrace();
        }
        #endregion

        public async Task<TSqlEntity> LoadEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}] WHERE [{idColumnName}] = @{nameof(id)}";
            TSqlEntity result = await dbConnection.QuerySingleOrDefaultAsync<TSqlEntity>(sql, new { id });
            return result;
        }

        public async Task<TSqlEntity[]> LoadEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}] WHERE [{idColumnName}] IN @{nameof(ids)}";
            TSqlEntity[] result = (await dbConnection.QueryAsync<TSqlEntity>(sql, new { ids })).ToArray();
            return result;
        }

        public async Task<TSqlEntity> LoadEntityByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName) where TSqlEntity : ISqlEntry
        {
            string filterSql = string.Join(" AND ", sqlFilters.Select(x => x.ToString()));
            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}] WHERE {filterSql}";
            TSqlEntity result = await dbConnection.QuerySingleOrDefaultAsync<TSqlEntity>(sql, sqlParams);
            return result;
        }

        public async Task<TSqlEntity> LoadEntityByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry
        {
            TSqlEntity result = await dbConnection.QuerySingleOrDefaultAsync<TSqlEntity>(sql, sqlParams);
            return result;
        }

        public async Task<DapperCustomQueryResult<TSqlEntity>> LoadEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias, SqlLimitCriteria limitCriteria, string tableName) where TSqlEntity : ISqlEntry
        {
            string filterSql = string.Join(" AND ", sqlFilters.Select(x => x.ToString()));

            string sortSql = string.Empty;
            if (sortCriterias?.Any() ?? false)
                sortSql = string.Join(",", sortCriterias.Select(x => $"[{x.ColumnName}] {x.SortDirection}"));

            string limitSql = string.Empty;
            if (limitCriteria != null)
            {
                limitSql = $"OFFSET {limitCriteria.Offset} ROWS FETCH NEXT {limitCriteria.Count} ROWS ONLY";
                if (string.IsNullOrWhiteSpace(sortSql))
                    sortSql = "[ID] ASC";
            }

            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}]" +
                $"{(string.IsNullOrWhiteSpace(filterSql) ? string.Empty : $" WHERE {filterSql}")}" +
                $"{(string.IsNullOrWhiteSpace(sortSql) ? string.Empty : $" ORDER BY {sortSql}")}" +
                $"{(string.IsNullOrWhiteSpace(limitSql) ? string.Empty : $" {limitSql}")}";

            string countSql = $"SELECT COUNT(*) FROM [{tableName ?? defaultTableName}]" +
                $"{(string.IsNullOrWhiteSpace(filterSql) ? string.Empty : $" WHERE {filterSql}")}";

            int totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql, sqlParams);

            TSqlEntity[] result = (await dbConnection.QueryAsync<TSqlEntity>(sql, sqlParams)).ToArray();
            return new DapperCustomQueryResult<TSqlEntity>(offset: limitCriteria?.Offset ?? 0, length: result.Length, totalNumberOfItems: totalCount, entries: result);
        }

        public async Task<DapperCustomQueryResult<TSqlEntity>> LoadEntitiesByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry
        {
            TSqlEntity[] result = (await dbConnection.QueryAsync<TSqlEntity>(sql, sqlParams)).ToArray();
            return new DapperCustomQueryResult<TSqlEntity>(offset: 0, length: result.Length, totalNumberOfItems: result.Length, entries: result);
        }

        public async Task<IEnumerable<TSqlEntity>> StreamAll<TSqlEntity>(string tableName) where TSqlEntity : ISqlEntry
        {
            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}]";
            IEnumerable<TSqlEntity> result = await dbConnection.QueryAsync<TSqlEntity>(sql);
            return result;
        }

        public async Task<IEnumerable<TSqlEntity>> StreamAllByCustomSql<TSqlEntity>(string sql) where TSqlEntity : ISqlEntry
        {
            IEnumerable<TSqlEntity> result = await dbConnection.QueryAsync<TSqlEntity>(sql);
            return result;
        }

        public async Task InsertEntity<TSqlEntity>(TSqlEntity entity, string tableName = null)
        {
            System.Reflection.PropertyInfo[] publicProperties = entity.GetType().GetProperties().Where(p => !p.GetGetMethod().IsVirtual).ToArray();
            string columnNames = string.Join(",", publicProperties.Select(p => $"[{p.Name}]"));
            string columnValues = string.Join(",", publicProperties.Select(p => $"@{p.Name}"));
            string sql = $"INSERT INTO [{tableName ?? defaultTableName}] ({columnNames}) VALUES ({columnValues})";
            await dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateEntityByID<TSqlEntity>(Guid id, TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            System.Reflection.PropertyInfo[] publicProperties = entity.GetType().GetProperties();
            string updateColumns = string.Join(",", publicProperties.Where(p => p.Name != idColumnName).Select(p => $"[{p.Name}]=@{p.Name}"));
            string sql = $"UPDATE [{tableName ?? defaultTableName}] SET {updateColumns} WHERE [{idColumnName}] = @{nameof(id)}";
            await dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task UpsertEntityByID<TSqlEntity>(Guid id, TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            bool entityExists = (await dbConnection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM [{tableName ?? defaultTableName}]  WHERE [{idColumnName}] = @{nameof(id)}", new { id })) > 0;

            if (entityExists)
            {
                await UpdateEntityByID(id, entity, tableName, idColumnName);
            }
            else
            {
                await InsertEntity(entity, tableName);
            }
        }

        public async Task DeleteEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            string sql = $"DELETE FROM [{tableName ?? defaultTableName}] WHERE [{idColumnName}] = @{nameof(id)}";
            await dbConnection.ExecuteAsync(sql, new { id });
        }

        private static string PrintSqlColumns(params string[] columnNames)
        {
            return
                string.Join(",", columnNames.Select(x => $"[{x}]"));
        }
    }
}
