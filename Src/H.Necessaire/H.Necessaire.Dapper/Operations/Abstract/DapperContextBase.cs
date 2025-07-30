using Dapper;
using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperContextBase : ImADapperContext
    {
        #region Construct
        protected readonly IDbConnection dbConnection;
        protected readonly string defaultTableName;
        public DapperContextBase(IDbConnection dbConnection, string defaultTableName = null)
        {
            this.defaultTableName = defaultTableName;
            this.dbConnection = dbConnection;
            dbConnection.Open();
        }

        protected abstract string PrintLimitSyntax(int offset, int count);

        public virtual void Dispose()
        {
            new Action(dbConnection.Close).TryOrFailWithGrace();
            new Action(dbConnection.Dispose).TryOrFailWithGrace();
        }
        #endregion

        public IDbTransaction BeginTransaction() => dbConnection.BeginTransaction();

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

        public async Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias, SqlLimitCriteria limitCriteria, string tableName) where TSqlEntity : ISqlEntry
        {
            string filterSql = string.Join(" AND ", sqlFilters.Select(x => x.ToString()));

            string sortSql = string.Empty;
            if (sortCriterias?.Any() ?? false)
                sortSql = string.Join(",", sortCriterias.Select(x => $"[{x.ColumnName}] {x.SortDirection}"));

            string limitSql = string.Empty;
            if (limitCriteria != null)
            {
                limitSql = PrintLimitSyntax(limitCriteria.Offset, limitCriteria.Count);
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

        public async Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry
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



        public async Task<IEnumerable<TSqlEntity>> StreamAllByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias, SqlLimitCriteria limitCriteria, string tableName) where TSqlEntity : ISqlEntry
        {
            string filterSql = string.Join(" AND ", sqlFilters.Select(x => x.ToString()));

            string sortSql = string.Empty;
            if (sortCriterias?.Any() ?? false)
                sortSql = string.Join(",", sortCriterias.Select(x => $"[{x.ColumnName}] {x.SortDirection}"));

            string limitSql = string.Empty;
            if (limitCriteria != null)
            {
                limitSql = PrintLimitSyntax(limitCriteria.Offset, limitCriteria.Count);
                if (string.IsNullOrWhiteSpace(sortSql))
                    sortSql = "[ID] ASC";
            }

            string sql = $"SELECT {PrintSqlColumns(typeof(TSqlEntity).GetColumnNames())} FROM [{tableName ?? defaultTableName}]" +
                $"{(string.IsNullOrWhiteSpace(filterSql) ? string.Empty : $" WHERE {filterSql}")}" +
                $"{(string.IsNullOrWhiteSpace(sortSql) ? string.Empty : $" ORDER BY {sortSql}")}" +
                $"{(string.IsNullOrWhiteSpace(limitSql) ? string.Empty : $" {limitSql}")}";

            IEnumerable<TSqlEntity> result = await dbConnection.QueryAsync<TSqlEntity>(sql, sqlParams);
            return result;
        }

        public async Task<IEnumerable<TSqlEntity>> StreamAllByCustomSql<TSqlEntity>(string sql) where TSqlEntity : ISqlEntry
        {
            IEnumerable<TSqlEntity> result = await dbConnection.QueryAsync<TSqlEntity>(sql);
            return result;
        }

        public async Task InsertEntity<TSqlEntity>(TSqlEntity entity, string tableName = null)
        {
            System.Reflection.PropertyInfo[] publicProperties = entity.GetType().GetPropertiesThatCountAsColumns();
            string columnNames = string.Join(",", publicProperties.Select(p => $"[{p.Name}]"));
            string columnValues = string.Join(",", publicProperties.Select(p => $"@{p.Name}"));
            string sql = $"INSERT INTO [{tableName ?? defaultTableName}] ({columnNames}) VALUES ({columnValues})";
            await dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateEntityByID<TSqlEntity>(TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            System.Reflection.PropertyInfo[] publicProperties = entity.GetType().GetPropertiesThatCountAsColumns();
            string updateColumns = string.Join(",", publicProperties.Where(p => p.Name != idColumnName).Select(p => $"[{p.Name}]=@{p.Name}"));
            string sql = $"UPDATE [{tableName ?? defaultTableName}] SET {updateColumns} WHERE [{idColumnName}] = @{idColumnName}";
            await dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task UpsertEntityByID<TSqlEntity>(TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            bool entityExists = (await dbConnection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM [{tableName ?? defaultTableName}]  WHERE [{idColumnName}] = @{idColumnName}", entity)) > 0;

            if (entityExists)
            {
                await UpdateEntityByID(entity, tableName, idColumnName);
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

        public async Task DeleteEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID")
        {
            string sql = $"DELETE FROM [{tableName ?? defaultTableName}] WHERE [{idColumnName}] IN @{nameof(ids)}";
            await dbConnection.ExecuteAsync(sql, new { ids });
        }

        public async Task DeleteEntitiesByByCustomCriteria(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null)
        {
            string filterSql = string.Join(" AND ", sqlFilters.Select(x => x.ToString()));
            string sql = $"DELETE FROM [{tableName ?? defaultTableName}] WHERE {filterSql}";
            await dbConnection.ExecuteAsync(sql, sqlParams);
        }

        public virtual async Task TruncateTable(string tableName = null)
        {
            string sql = $"TRUNCATE TABLE [{tableName ?? defaultTableName}]";
            await dbConnection.ExecuteAsync(sql);
        }

        protected virtual string PrintSqlColumns(params string[] columnNames)
        {
            return
                string.Join(",", columnNames.Select(x => $"[{x}]"));
        }
    }
}
