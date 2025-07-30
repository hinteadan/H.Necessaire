using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public interface ImADapperContext : IDisposable
    {
        Task<TSqlEntity> LoadEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry;
        Task<TSqlEntity[]> LoadEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry;

        Task<TSqlEntity> LoadEntityByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName) where TSqlEntity : ISqlEntry;

        Task<TSqlEntity> LoadEntityByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry;

        Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias, SqlLimitCriteria limitCriteria, string tableName) where TSqlEntity : ISqlEntry;
        Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry;

        Task<IEnumerable<TSqlEntity>> StreamAll<TSqlEntity>(string tableName) where TSqlEntity : ISqlEntry;
        Task<IEnumerable<TSqlEntity>> StreamAllByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias, SqlLimitCriteria limitCriteria, string tableName) where TSqlEntity : ISqlEntry;
        Task<IEnumerable<TSqlEntity>> StreamAllByCustomSql<TSqlEntity>(string sql) where TSqlEntity : ISqlEntry;

        Task InsertEntity<TSqlEntity>(TSqlEntity entity, string tableName = null);

        Task UpdateEntityByID<TSqlEntity>(TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry;

        Task UpsertEntityByID<TSqlEntity>(TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry;

        Task DeleteEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry;
        Task DeleteEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID");

        Task DeleteEntitiesByByCustomCriteria(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null);

        Task TruncateTable(string tableName = null);

        IDbTransaction BeginTransaction();
    }
}
