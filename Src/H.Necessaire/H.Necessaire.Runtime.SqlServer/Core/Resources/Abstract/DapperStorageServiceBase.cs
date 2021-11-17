using Dapper;
using H.Necessaire.Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer
{
    public abstract class DapperStorageServiceBase<TId, TEntity, TSqlEntity, TFilter>
    : DapperSqlResourceBase
        , ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : class, IDentityType<TId>, new()
        where TSqlEntity : ISqlEntry, new()
    {
        #region Construct
        protected DapperStorageServiceBase(string connectionString = null, string tableName = null, string databaseName = null)
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

        protected override bool IsCoreDatabase()
        {
            return
                base.IsCoreDatabase()
                ||
                this.GetType().Assembly.In(
                    typeof(SqlScriptExtensions).Assembly
                );
        }
        #endregion

        public async Task<OperationResult> DeleteByID(TId id)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        await base.DeleteEntitiesByCustomCriteria<TSqlEntity>(new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(id), "=").AsArray(), new { id });

                        result = OperationResult.Win();
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        public async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            OperationResult<TId>[] result = new OperationResult<TId>[0];

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        await base.DeleteEntitiesByCustomCriteria<TSqlEntity>(new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(ids), "IN").AsArray(), new { ids });

                        result = ids.Select(x => OperationResult.Win().WithPayload(x)).ToArray();
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = ids.Select(x => OperationResult.Fail(ex).WithPayload(x)).ToArray()
                );

            return result;
        }

        public async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            OperationResult<TEntity> result = OperationResult.Fail().WithoutPayload<TEntity>();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        TEntity entity =
                            (await base.LoadEntityByCustomCriteria<TSqlEntity>(
                                new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(id), "=").AsArray(),
                                new { id })
                            )?.ToEntity<TEntity, TSqlEntity>();
                        result = OperationResult.Win().WithPayload(entity);
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<TEntity>()
                );

            return result;
        }

        public async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            OperationResult<TEntity>[] result = new OperationResult<TEntity>[0];

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        TEntity[] entities =
                            (await base.LoadEntitiesByCustomCriteria<TSqlEntity>(
                                new SqlFilterCriteria(nameof(IDentityType<TId>.ID), nameof(ids), "IN").AsArray(),
                                new { ids })
                            )
                            ?.Select(x => x.ToEntity<TEntity, TSqlEntity>())
                            .ToArray()
                            ;

                        result = entities.Select(x => OperationResult.Win().WithPayload(x)).ToArray();
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = ids.Select(id => OperationResult.Fail(ex).WithComment($"{nameof(IDentityType<TId>.ID)}:{id}").WithoutPayload<TEntity>()).ToArray()
                );

            return result;
        }

        public async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<Page<TEntity>> result = OperationResult.Fail().WithPayload(Page<TEntity>.Empty());

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        DynamicParameters sqlParams = new DynamicParameters().And(x => x.AddDynamicParams(filter));

                        ILimitedEnumerable<TSqlEntity> sqlResult
                            = await base.LoadEntitiesByCustomCriteria<TSqlEntity>(ApplyFilter(filter, sqlParams), sqlParams, filter?.ToSqlSortCriteria(), filter?.ToSqlLimitCriteria());

                        result
                            = OperationResult
                            .Win()
                            .WithPayload(
                                Page<TEntity>.For(
                                    filter,
                                    sqlResult.TotalNumberOfItems,
                                    sqlResult.Select(
                                        x => x.ToEntity<TEntity, TSqlEntity>()
                                    ).ToArray()
                                )
                            );
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithPayload(Page<TEntity>.Empty())
                );

            return result;
        }

        public async Task<OperationResult> Save(TEntity entity)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        await base.SaveEntity(entity.ToSqlEntity<TEntity, TSqlEntity>());

                        result = OperationResult.Win();
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Fail().WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        DynamicParameters sqlParams = new DynamicParameters().And(x => x.AddDynamicParams(filter));

                        result = OperationResult.Win().WithPayload(
                            await base.StreamAllByCustomCriteria<TSqlEntity, TEntity>(x => x.ToEntity<TEntity, TSqlEntity>(), ApplyFilter(filter, sqlParams), sqlParams, filter?.ToSqlSortCriteria(), filter?.ToSqlLimitCriteria())
                            );
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<IDisposableEnumerable<TEntity>>()
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Fail().WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureDatabaseAndMigrations();

                        result = OperationResult.Win().WithPayload(
                            await base.StreamAll<TSqlEntity, TEntity>(x => x.ToEntity<TEntity, TSqlEntity>())
                            );
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<IDisposableEnumerable<TEntity>>()
                );

            return result;
        }
    }
}
