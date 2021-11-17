using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerUserCredentialsStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(UserCredentials),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{nameof(UserCredentials)}] 
(
	[{nameof(UserCredentials.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(UserCredentials.Password)}] [nvarchar](MAX) NOT NULL,

	CONSTRAINT [PK_{nameof(UserCredentials)}] PRIMARY KEY
	(
		[{nameof(UserCredentials.ID)}] ASC
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
