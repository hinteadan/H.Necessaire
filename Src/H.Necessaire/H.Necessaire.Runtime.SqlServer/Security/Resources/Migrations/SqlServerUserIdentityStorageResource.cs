using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerUserIdentityStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(UserInfo),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{nameof(UserInfo)}] 
(
	[{nameof(UserInfoSqlEntity.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(UserInfoSqlEntity.IDTag)}] [nvarchar](450) NOT NULL,
	[{nameof(UserInfoSqlEntity.Username)}] [nvarchar](450) NOT NULL,
	[{nameof(UserInfoSqlEntity.DisplayName)}] [nvarchar](450) NULL,
	[{nameof(UserInfoSqlEntity.Email)}] [nvarchar](450) NULL,
	[{nameof(UserInfoSqlEntity.NotesJson)}] [ntext] NULL,

	CONSTRAINT [PK_{nameof(UserInfo)}] PRIMARY KEY
	(
		[{nameof(UserInfoSqlEntity.ID)}] ASC
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


CREATE NONCLUSTERED INDEX [IX_{nameof(UserInfo)}_{nameof(UserInfoSqlEntity.IDTag)}] ON [dbo].[{nameof(UserInfo)}]
(
	[{nameof(UserInfoSqlEntity.IDTag)}] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_{nameof(UserInfo)}_{nameof(UserInfoSqlEntity.DisplayName)}] ON [dbo].[{nameof(UserInfo)}]
(
	[{nameof(UserInfoSqlEntity.DisplayName)}] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_{nameof(UserInfo)}_{nameof(UserInfoSqlEntity.Username)}] ON [dbo].[{nameof(UserInfo)}]
(
	[{nameof(UserInfoSqlEntity.Username)}] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [IX_{nameof(UserInfo)}_{nameof(UserInfoSqlEntity.Email)}] ON [dbo].[{nameof(UserInfo)}]
(
	[{nameof(UserInfoSqlEntity.Email)}] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

",
            },
        };
    }
}
