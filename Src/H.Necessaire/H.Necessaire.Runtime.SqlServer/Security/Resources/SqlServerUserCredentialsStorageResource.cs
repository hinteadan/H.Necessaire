using H.Necessaire.Dapper;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerUserCredentialsStorageResource : DapperSqlServerResourceBase, ImAUserCredentialsStorageResource
    {
        #region Construct
        public SqlServerUserCredentialsStorageResource()
            : base(connectionString: null, tableName: nameof(UserCredentials), databaseName: "H.Necessaire.Core")
        { }

        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();

        protected override bool IsCoreDatabase() => true;
        #endregion

        public async Task<string> GetPasswordFor(Guid userID)
        {
            return (await base.LoadEntityByID<UserCredentials>(userID))?.Password;
        }

        public async Task SetPasswordFor(UserInfo userInfo, string password)
        {
            await
                base.SaveEntity(new UserCredentials
                {
                    ID = userInfo.ID,
                    Password = password,
                });
        }

        class UserCredentials : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public string Password { get; set; }
        }
    }
}
