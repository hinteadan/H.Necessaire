using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class ConsumerIdentitySqlServerStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] 
(
	[{nameof(ConsumerIdentitySqlEntry.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(ConsumerIdentitySqlEntry.IDTag)}] [nvarchar](450) NOT NULL,
	[{nameof(ConsumerIdentitySqlEntry.DisplayName)}] [nvarchar](450) NOT NULL,
    [{nameof(ConsumerIdentitySqlEntry.NotesJson)}] [ntext] NULL,

	{nameof(ConsumerIdentitySqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


{nameof(ConsumerIdentitySqlEntry.IDTag).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
{nameof(ConsumerIdentitySqlEntry.DisplayName).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}

",
            },
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 1),
                SqlCommand = $@"

ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.IpAddress)}] [nvarchar](450) NULL;

{nameof(ConsumerIdentitySqlEntry.IpAddress).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}

",
            },
        };
    }
}
