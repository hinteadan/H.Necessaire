﻿using H.Necessaire.Dapper.Operations.Concrete;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;
using Dapper;

namespace H.Necessaire.Dapper
{
    public abstract class DapperResourceBase : ImADependency
    {
        #region Construct
        ImASqlConnectionFactory sqlConnectionFactory = null;
        ImASqlMigrationStore sqlMigrationStore = null;

        protected string connectionString;
        protected string connectionStringWithoutDatabase;
        protected string databaseName;
        protected string tableName;
        bool isMigrationEnsured = false;
        bool isMigrating = false;

        protected DapperResourceBase(string connectionString = null, string tableName = null, string databaseName = null)
        {
            this.connectionString = connectionString;
            this.connectionStringWithoutDatabase = connectionString?.WithoutDatabase();
            this.tableName = tableName;
            this.databaseName = databaseName;
            if (this.databaseName != this.connectionString?.GetDatabaseName()) this.connectionString = this.connectionString?.WithDatabase(this.databaseName);
        }

        protected abstract Task EnsureDatabase();
        protected abstract ImADapperContext NewDbContext(string tableName = null);
        protected abstract Task<SqlMigration[]> GetAllMigrations();

        protected virtual string GetCoreDatabaseName(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig runtimeConfig = dependencyProvider?.GetRuntimeConfig();
            string coreDatabaseNameFromConfig = runtimeConfig?.Get("SqlConnections")?.Get("DatabaseNames")?.Get("Core")?.ToString();
            return coreDatabaseNameFromConfig ?? this.databaseName;
        }

        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            sqlConnectionFactory = dependencyProvider.Get<ImASqlConnectionFactory>();
            if (sqlConnectionFactory is null)
                throw new ArgumentNullException(nameof(sqlConnectionFactory), $"A concrete implementation for {nameof(ImASqlConnectionFactory)} must be registered within the current dependecy registry");

            if (IsCoreDatabase())
                this.databaseName = GetCoreDatabaseName(dependencyProvider);

            this.sqlMigrationStore = typeof(ImASqlMigrationStore).IsAssignableFrom(this.GetType()) ? this as ImASqlMigrationStore : dependencyProvider.Get<ImASqlMigrationStore>();
            ImASqlEntityConnectionProvider sqlEntityConnectionProvider = dependencyProvider.Get<ImASqlEntityConnectionProvider>();
            SqlEntityConnection connectionInfo = sqlEntityConnectionProvider?.GetConnectionStringByType(this.GetType());

            this.connectionString = this.connectionString ?? connectionInfo?.ConnectionString;
            this.connectionStringWithoutDatabase = this.connectionStringWithoutDatabase ?? connectionInfo?.ConnectionString?.WithoutDatabase();
            this.databaseName = this.databaseName ?? connectionInfo?.DatabaseName;
            this.tableName = this.tableName ?? connectionInfo?.TableName ?? this.GetType().Name.ToSafeFileName();
            if (this.databaseName != this.connectionString?.GetDatabaseName())
                this.connectionString = this.connectionString?.WithDatabase(this.databaseName);
        }
        #endregion

        protected async Task EnsureDatabaseAndMigrations()
        {
            await EnsureDatabase();
            await EnsureMigrations();
        }

        protected virtual async Task<TSqlEntity> LoadEntityByID<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntityByID<TSqlEntity>(id, tableName, idColumnName);
            }
        }

        protected virtual async Task<TSqlEntity> LoadEntityByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntityByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, tableName);
            }
        }

        protected virtual async Task<TSqlEntity> LoadEntityByCustomSql<TSqlEntity>(string sql, object sqlParams, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntityByCustomSql<TSqlEntity>(sql, sqlParams);
            }
        }

        protected virtual async Task<TSqlEntity[]> LoadEntitiesByIDs<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntitiesByIDs<TSqlEntity>(ids, tableName, idColumnName);
            }
        }

        protected virtual async Task<TSqlEntity[]> LoadEntitiesByCustomSql<TSqlEntity>(string sql, object sqlParams, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return (await dapper.LoadEntitiesByCustomSql<TSqlEntity>(sql, sqlParams)).ToArray();
            }
        }

        protected virtual async Task<ILimitedEnumerable<TSqlEntity>> LoadEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias = null, SqlLimitCriteria limitCriteria = null, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                return await dapper.LoadEntitiesByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, sortCriterias, limitCriteria, tableName);
            }
        }

        protected virtual async Task<IDisposableEnumerable<TResult>> StreamAll<TSqlEntity, TResult>(Func<TSqlEntity, TResult> projection, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            ImADapperContext dapper = NewDbContext(tableName);

            return new DapperStream<TResult>(dapper, (await dapper.StreamAll<TSqlEntity>(tableName)).Select(projection));
        }

        protected virtual async Task<IDisposableEnumerable<TResult>> StreamAllByCustomCriteria<TSqlEntity, TResult>(Func<TSqlEntity, TResult> projection, ISqlFilterCriteria[] sqlFilters, object sqlParams, SqlSortCriteria[] sortCriterias = null, SqlLimitCriteria limitCriteria = null, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            ImADapperContext dapper = NewDbContext(tableName);

            return new DapperStream<TResult>(dapper, (await dapper.StreamAllByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, sortCriterias, limitCriteria, tableName)).Select(projection));
        }

        protected virtual async Task<IDisposableEnumerable<TResult>> StreamAllByCustomSql<TSqlEntity, TResult>(Func<TSqlEntity, TResult> projection, string sql) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            ImADapperContext dapper = NewDbContext(tableName);

            return new DapperStream<TResult>(dapper, (await dapper.StreamAllByCustomSql<TSqlEntity>(sql)).Select(projection));
        }

        protected virtual async Task InsertEntity<TSqlEntity>(TSqlEntity entity, string tableName = null) where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                await dapper.InsertEntity(entity);
            }
        }

        protected virtual async Task SaveEntity<TSqlEntity>(TSqlEntity entity, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                await dapper.UpsertEntityByID(entity, idColumnName: idColumnName);
            }
        }

        protected virtual async Task DeleteEntity<TSqlEntity>(Guid id, string tableName = null, string idColumnName = "ID") where TSqlEntity : ISqlEntry
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntityByID<TSqlEntity>(id, tableName, idColumnName);
            }
        }

        protected virtual async Task DeleteEntities<TSqlEntity>(Guid[] ids, string tableName = null, string idColumnName = "ID")
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntitiesByIDs<TSqlEntity>(ids, tableName, idColumnName);
            }
        }

        protected virtual async Task DeleteEntitiesByCustomCriteria<TSqlEntity>(ISqlFilterCriteria[] sqlFilters, object sqlParams, string tableName = null)
        {
            await EnsureDatabaseAndMigrations();

            using (ImADapperContext dapper = NewDbContext(tableName))
            {
                await dapper.DeleteEntitiesByByCustomCriteria<TSqlEntity>(sqlFilters, sqlParams, tableName);
            }
        }

        protected async Task<string> ReadSqlFromEmbedResourceSql(string sqlFileName, string sqlFileNamespace = "H.Necessaire.Dapper")
        {
            using (Stream stream = Assembly.GetAssembly(typeof(DapperResourceBase)).GetManifestResourceStream($"{sqlFileNamespace}.{sqlFileName}"))
            {
                return await stream.ReadAsStringAsync();
            }
        }

        protected virtual async Task EnsureMigrations()
        {
            if (isMigrating)
                return;

            using (new ScopedRunner(onStart: () => isMigrating = true, onStop: () => isMigrating = false))
            {
                if (isMigrationEnsured)
                    return;

                if (sqlMigrationStore == null)
                {
                    isMigrationEnsured = true;
                    return;
                }

                SqlMigration[] allMigrations = await GetAllMigrations();
                if (!allMigrations?.Any() ?? true)
                {
                    isMigrationEnsured = true;
                    return;
                }

                SqlMigration[] ranMigrations = (await sqlMigrationStore.Browse(new SqlMigrationFilter { IDs = allMigrations.Select(x => x.ID).ToArray() })) ?? new SqlMigration[0];

                SqlMigration[] migrationsToRun = allMigrations.Where(x => x.NotIn(ranMigrations, (a, b) => a.ID == b.ID)).OrderBy(x => x.VersionNumber, VersionNumber.Comparer).ToArray();

                if (!migrationsToRun?.Any() ?? true)
                {
                    isMigrationEnsured = true;
                    return;
                }

                using (IDbConnection dbConnection = sqlConnectionFactory.BuildNewConnection(connectionString))
                {
                    foreach (SqlMigration migrationToRun in migrationsToRun)
                    {
                        await dbConnection.ExecuteAsync(migrationToRun.SqlCommand);
                        await sqlMigrationStore.Append(migrationToRun);
                    }
                }

                isMigrationEnsured = true;
            }
        }

        protected virtual bool IsCoreDatabase()
        {
            Assembly assembly = GetType().Assembly;

            return
                assembly == typeof(DapperSqlServerResourceBase).Assembly
                || assembly.FullName.StartsWith("H.Necessaire.")
                ;
        }

        protected static async Task<string> ReadMigrationSqlCommand(SqlMigration sqlMigration, params Assembly[] assembliesToScan)
        {
            return
                await $"{sqlMigration.ResourceIdentifier}_v{sqlMigration.VersionNumber.Major}-{sqlMigration.VersionNumber.Minor}.sql"
                .OpenEmbeddedResource(assembliesToScan)
                .ReadAsStringAsync(isStreamLeftOpen: false)
                ;
        }
    }
}
