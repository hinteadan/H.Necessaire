using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerUserIdentityStorageResource : DapperSqlServerResourceBase, ImAUserInfoStorageResource
    {
        #region Construct
        public SqlServerUserIdentityStorageResource()
            : base(connectionString: null, tableName: nameof(UserInfo), databaseName: "H.Necessaire.Core")
        { }

        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();

        protected override bool IsCoreDatabase() => true;
        #endregion

        public async Task<UserInfo[]> GetUsersByIds(params Guid[] ids)
        {
            return
                (await base.LoadEntitiesByIDs<UserInfoSqlEntity>(ids))?.Select(x => x.ToEntity<UserInfo, UserInfoSqlEntity>()).ToArray();
        }

        public async Task SaveUser(UserInfo userInfo)
        {
            await base.SaveEntity(userInfo.ToSqlEntity<UserInfo, UserInfoSqlEntity>());
        }

        public async Task<UserInfo[]> SearchUsers(UserInfoSearchCriteria searchCriteria)
        {
            DynamicParameters sqlParams = new DynamicParameters();
            sqlParams.AddDynamicParams(searchCriteria);

            return
                (await base.LoadEntitiesByCustomCriteria<UserInfoSqlEntity>(Map(searchCriteria, sqlParams), sqlParams))?.Select(x => x.ToEntity<UserInfo, UserInfoSqlEntity>()).ToArray();
        }

        private ISqlFilterCriteria[] Map(UserInfoSearchCriteria searchCriteria, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (searchCriteria?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(UserInfoSqlEntity.ID), parameterName: nameof(searchCriteria.IDs), @operator: "IN"));
            }

            if (searchCriteria?.Usernames?.Any() ?? false)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    searchCriteria
                    .Usernames
                    .Select(
                        (usr, index) =>
                        {
                            sqlParams.Add($"{nameof(UserInfoSqlEntity.ID)}{index}", usr);
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(UserInfoSqlEntity.ID),
                                    parameterName: $"{nameof(UserInfoSqlEntity.ID)}{index}",
                                    @operator: "like"
                                )
                                { LikeOperatorMatch = SqlFilterCriteria.LikeOperatorMatchType.Exact };
                        }
                    ).ToArray()
                ));
            }

            return result.ToArray();
        }

        class UserInfoSqlEntity : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public string IDTag { get; set; }
            public string DisplayName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string NotesJson { get; set; }
        }

        class UserInfoMapper : SqlEntityMapperBase<UserInfo, UserInfoSqlEntity>
        {
            static UserInfoMapper() => new UserInfoMapper().RegisterMapper();

            public override UserInfoSqlEntity MapEntityToSql(UserInfo entity)
            {
                UserInfoSqlEntity result = base.MapEntityToSql(entity);

                result.NotesJson = entity.Notes?.ToJsonArray();

                return result;
            }

            public override UserInfo MapSqlToEntity(UserInfoSqlEntity sqlEntity)
            {
                UserInfo result = base.MapSqlToEntity(sqlEntity);

                result.Notes = sqlEntity.NotesJson?.DeserializeToNotes();

                return result;
            }
        }
    }
}
