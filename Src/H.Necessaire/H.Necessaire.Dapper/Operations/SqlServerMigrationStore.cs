﻿using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    [ID("SqlServer")]
    internal class SqlServerMigrationStore : DapperSqlServerResourceBase, ImASqlMigrationStore
    {
        #region Construct
        ImASqlConnectionFactory sqlConnectionFactory = null;
        bool isDatabaseEnsured = false;
        public SqlServerMigrationStore(string connectionString = null) : base(connectionString, tableName: "H.Necessaire.Migration", databaseName: "H.Necessaire.Core") { }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            sqlConnectionFactory = dependencyProvider.Get<ImASqlConnectionFactory>();
        }

        protected override Task<SqlMigration[]> GetAllMigrations() => new SqlMigration[0].AsTask();

        protected override async Task EnsureDatabase()
        {
            await base.EnsureDatabase();

            if (isDatabaseEnsured)
                return;

            using (IDbConnection dbConnection = sqlConnectionFactory.BuildNewConnection(connectionString))
            {
                bool migrationTableExists = await dbConnection.ExecuteScalarAsync<bool>("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'H.Necessaire.Migration')) BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END");

                if (migrationTableExists)
                {
                    isDatabaseEnsured = true;
                    return;
                }

                await dbConnection.ExecuteAsync(await ReadSqlFromEmbedResourceSql("Create_SqlServerMigration_Table.sql"));
            }

            isDatabaseEnsured = true;
        }
        #endregion

        public async Task<OperationResult> Append(SqlMigration migration)
        {
            SqlMigration existingMigration = (await Browse(new SqlMigrationFilter { IDs = migration.ID.AsArray() }))?.SingleOrDefault();

            if (existingMigration != null)
                return OperationResult.Fail($"Migration {migration.ID} already ran on {existingMigration.HappenedAt}");

            await
                SaveEntity(migration.ToSqlEntity<SqlMigration, SqlMigrationSqlEntry>());

            return OperationResult.Win();
        }

        public async Task<SqlMigration[]> Browse(SqlMigrationFilter filter)
        {
            ILimitedEnumerable<SqlMigrationSqlEntry> sqlEntities = await LoadEntitiesByCustomCriteria<SqlMigrationSqlEntry>(MapFilter(filter).ToArray(), filter);
            return
                sqlEntities
                .Select(x => x.ToEntity<SqlMigration, SqlMigrationSqlEntry>())
                .ToArray();
        }

        private IEnumerable<ISqlFilterCriteria> MapFilter(SqlMigrationFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
                yield return new SqlFilterCriteria(nameof(SqlMigrationSqlEntry.ID), nameof(filter.IDs), "IN");

            if (filter?.ResourceIdentifiers?.Any() ?? false)
                yield return new SqlFilterCriteria(nameof(SqlMigrationSqlEntry.ResourceIdentifier), nameof(filter.ResourceIdentifiers), "IN");

            if (filter?.VersionNumbers?.Any() ?? false)
                yield return new SqlFilterCriteria(nameof(SqlMigrationSqlEntry.VersionNumber), nameof(filter.VersionNumbersAsString), "IN");

            if (filter?.FromInclusive != null)
                yield return new SqlFilterCriteria(nameof(SqlMigrationSqlEntry.HappenedAtTicks), nameof(filter.FromInclusiveAsTicks), ">=");

            if (filter?.ToInclusive != null)
                yield return new SqlFilterCriteria(nameof(SqlMigrationSqlEntry.HappenedAtTicks), nameof(filter.ToInclusiveAsTicks), "<=");
        }
    }
}
