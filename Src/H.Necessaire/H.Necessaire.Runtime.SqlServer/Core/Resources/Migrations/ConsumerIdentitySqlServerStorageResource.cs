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
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 2),
                SqlCommand = $@"

ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.AsOf)}] [datetime2](7) NOT NULL DEFAULT '1900-01-01';
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.AsOfTicks)}] [bigint] NOT NULL DEFAULT (0);

",
            }
            ,
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 3),
                SqlCommand = $@"

{nameof(ConsumerIdentitySqlEntry.AsOf).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
{nameof(ConsumerIdentitySqlEntry.AsOfTicks).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}

",
            }
            ,
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 4),
                SqlCommand = $@"

ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.HostName)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.Protocol)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.UserAgent)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.AiUserID)}] [ntext] NULL;
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.Origin)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.Referer)}] [ntext] NULL;


{nameof(ConsumerIdentitySqlEntry.HostName).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
{nameof(ConsumerIdentitySqlEntry.Protocol).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
{nameof(ConsumerIdentitySqlEntry.UserAgent).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}
{nameof(ConsumerIdentitySqlEntry.Origin).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ConsumerIdentity)}")}

",
            },
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerIdentity),
                VersionNumber = new VersionNumber(1, 5),
                SqlCommand = $@"

ALTER TABLE [dbo].[H.Necessaire.{nameof(ConsumerIdentity)}] ADD [{nameof(ConsumerIdentitySqlEntry.RuntimePlatformJson)}] [ntext] NULL;

",
            },
        };
    }
}
