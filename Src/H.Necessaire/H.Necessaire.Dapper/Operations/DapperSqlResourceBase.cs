using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperSqlResourceBase
    {
        #region Construct
        readonly string connectionString;
        readonly string defaultTableName;
        protected DapperSqlResourceBase(string connectionString, string defaultTableName)
        {
            this.connectionString = connectionString;
            this.defaultTableName = defaultTableName;
        }
        #endregion

        protected virtual async Task<TSqlEntity> LoadEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntityByID<TSqlEntity>(id, idColumnName);
            }
        }

        protected virtual async Task<TSqlEntity> LoadEntityByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null) where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntityByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, tableName);
            }
        }

        protected virtual async Task<TSqlEntity> LoadEntityByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext())
            {
                return await dapper.LoadEntityByCustomSql<TSqlEntity>(sql, sqlParams);
            }
        }

        protected virtual async Task<TSqlEntity[]> LoadEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntitiesByIDs<TSqlEntity>(ids, tableName, idColumnName);
            }
        }

        protected virtual async Task<TSqlEntity[]> LoadEntitiesByCustomSql<TSqlEntity>(string sql, object sqlParams) where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext())
            {
                return (await dapper.LoadEntitiesByCustomSql<TSqlEntity>(sql, sqlParams)).ToArray();
            }
        }

        protected virtual async Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias = null, SqlLimitCriteria limitCriteria = null, string tableName = null) where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntitiesByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, sortCriterias, limitCriteria, tableName);
            }
        }

        protected virtual async Task<IDisposableEnumerable<TResult>> StreamAll<TSqlEntity, TResult>(Func<TSqlEntity, TResult> projection, string tableName = null) where TSqlEntity : ISqlEntry
        {
            DapperSqlContext dapper = NewDbContext(tableName);

            return new DapperStream<TResult>(dapper, (await dapper.StreamAll<TSqlEntity>(tableName)).Select(projection));
        }

        protected virtual async Task<IDisposableEnumerable<TResult>> StreamAllByCustomSql<TSqlEntity, TResult>(Func<TSqlEntity, TResult> projection, string sql) where TSqlEntity : ISqlEntry
        {
            DapperSqlContext dapper = NewDbContext();

            return new DapperStream<TResult>(dapper, (await dapper.StreamAllByCustomSql<TSqlEntity>(sql)).Select(projection));
        }

        protected virtual async Task InsertEntity<TSqlEntity>(TSqlEntity entity, string tableName = null) where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                await dapper.InsertEntity(entity);
            }
        }

        protected virtual async Task SaveEntity<TSqlEntity>(Guid id, TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                await dapper.UpsertEntityByID(id, entity, idColumnName: idColumnName);
            }
        }

        protected virtual async Task DeleteEntity<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntityByID<TSqlEntity>(id, tableName, idColumnName);
            }
        }

        protected virtual async Task DeleteEntities<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID")
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntitiesByIDs<TSqlEntity>(ids, tableName, idColumnName);
            }
        }

        protected virtual async Task DeleteEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null)
        {
            using (DapperSqlContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntitiesByByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, tableName);
            }
        }

        private DapperSqlContext NewDbContext(string tableName = null)
        {
            return new DapperSqlContext(connectionString, tableName ?? defaultTableName);
        }
    }
}
