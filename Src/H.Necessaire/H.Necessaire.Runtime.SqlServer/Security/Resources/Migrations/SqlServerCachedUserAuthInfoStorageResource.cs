using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerCachedUserAuthInfoStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(UserAuthKey),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{nameof(UserAuthKey)}] 
(
	[{nameof(UserAuthKey.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(UserAuthKey.Key)}] [nvarchar](MAX) NOT NULL,
	[{nameof(UserAuthKey.NotesJson)}] [ntext] NULL,

	CONSTRAINT [PK_{nameof(UserAuthKey)}] PRIMARY KEY
	(
		[{nameof(UserAuthKey.ID)}] ASC
	) 
	WITH 
	(
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
	)
	ON [PRIMARY]

)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

",
            },
        };
    }
}
