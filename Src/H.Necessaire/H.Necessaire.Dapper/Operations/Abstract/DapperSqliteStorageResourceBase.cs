using Dapper;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperSqliteStorageResourceBase<TId, TEntity, TSqlEntity, TFilter>
    : DapperSqliteResourceBase
        , ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : class, IDentityType<TId>, new()
        where TSqlEntity : ISqlEntry, new()
    {
        #region Construct
        protected DapperSqliteStorageResourceBase(string connectionString = null, string tableName = null, string databaseName = null)
            : base(connectionString, tableName, databaseName)
        { }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            SqlEntityConnection specificConnection = GetSpecificEntityConnection();
            if (specificConnection != null)
            {
                ImASqlEntityConnectionRegistry sqlEntityConnectionRegistry = dependencyProvider.Get<ImASqlEntityConnectionRegistry>();
                sqlEntityConnectionRegistry.RegisterConnectionForType(specificConnection.Type, specificConnection);
            }

            base.ReferDependencies(dependencyProvider);
        }

        protected virtual SqlEntityConnection GetSpecificEntityConnection() => null;

        protected abstract ISqlFilterCriteria[] ApplyFilter(TFilter filter, DynamicParameters sqlParams);
        #endregion

        public virtual async Task<OperationResult> DeleteByID(TId id)
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                await base.DeleteEntitiesByCustomCriteria(new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(id), "=").AsArray(), new { id });
            }, $"delete entity by ID ({id})");
        }

        public virtual async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            OperationResult result = await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                await base.DeleteEntitiesByCustomCriteria(new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(ids), "IN").AsArray(), new { ids });
            }, $"delete entities by {ids?.Length ?? 0} IDs");

            return
                result.IsSuccessful
                ? ids.Select(x => OperationResult.Win().WithPayload(x)).ToArray()
                : ids.Select(x => result.WithPayload(x)).ToArray()
                ;
        }

        public virtual async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                return
                    (await base.LoadEntityByCustomCriteria<TSqlEntity>(
                        new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(id), "=").AsArray(),
                        new { id })
                    )
                    ?.ToEntity<TEntity, TSqlEntity>()
                    ;
            }, $"load entity by ID ({id})");
        }

        public virtual async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            OperationResult<TEntity[]> result = await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                return
                    (await base.LoadEntitiesByCustomCriteria<TSqlEntity>(
                        new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(ids), "IN").AsArray(),
                        new { ids })
                    )
                    ?.Select(x => x.ToEntity<TEntity, TSqlEntity>())
                    .ToArray()
                    ;
            }, $"load entities by {ids?.Length ?? 0} IDs");

            return
                result.IsSuccessful
                ? result.Payload.Select(x => OperationResult.Win().WithPayload(x)).ToArray()
                : ids.Select(id => result.WithComment($"{nameof(IDentityType<TId>.ID)}:{id}").WithoutPayload<TEntity>()).ToArray()
                ;
        }

        public virtual async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                DynamicParameters sqlParams = new DynamicParameters().And(x => x.AddDynamicParams(filter));

                ILimitedEnumerable<TSqlEntity> sqlResult
                    = await base.LoadEntitiesByCustomCriteria<TSqlEntity>(ApplyFilter(filter, sqlParams), sqlParams, filter?.ToSqlSortCriteria(), filter?.ToSqlLimitCriteria());

                return
                    Page<TEntity>.For(
                        filter,
                        sqlResult.TotalNumberOfItems,
                        sqlResult.Select(
                            x => x.ToEntity<TEntity, TSqlEntity>()
                        ).ToArray()
                    );
            }, $"load {typeof(TEntity).Name} page index {filter?.PageFilter?.PageIndex ?? 0} of size {filter?.PageFilter?.PageSize ?? 0}");
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                await base.SaveEntity(entity.ToSqlEntity<TEntity, TSqlEntity>());
            }, $"save {typeof(TEntity).Name} entity with id {entity.SafeRead(x => x.ID.ToString(), "[Unknown ID]")}");
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                DynamicParameters sqlParams = new DynamicParameters().And(x => x.AddDynamicParams(filter));

                return
                    await base.StreamAllByCustomCriteria<TSqlEntity, TEntity>(x => x.ToEntity<TEntity, TSqlEntity>(), ApplyFilter(filter, sqlParams), sqlParams, filter?.ToSqlSortCriteria(), filter?.ToSqlLimitCriteria());
            }, $"stream filtered {typeof(TEntity).Name} entities");
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            return await HSafe.Run(async () =>
            {
                await EnsureDatabaseAndMigrations();

                return
                    await base.StreamAll<TSqlEntity, TEntity>(x => x.ToEntity<TEntity, TSqlEntity>());
            }, $"stream all {typeof(TEntity).Name} entities");
        }
    }
}
