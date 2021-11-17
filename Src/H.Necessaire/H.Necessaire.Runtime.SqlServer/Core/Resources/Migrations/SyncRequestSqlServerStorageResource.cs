using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class SyncRequestSqlServerStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(SyncRequest),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[H.Necessaire.{nameof(SyncRequest)}] 
(
	[{nameof(SyncRequestSqlEntity.ID)}] [nvarchar](450) NOT NULL,
	[{nameof(SyncRequestSqlEntity.PayloadIdentifier)}] [nvarchar](450) NOT NULL,
	[{nameof(SyncRequestSqlEntity.PayloadType)}] [nvarchar](450) NOT NULL,
	[{nameof(SyncRequestSqlEntity.Payload)}] [ntext] NULL,
	[{nameof(SyncRequestSqlEntity.SyncStatus)}] [int] NOT NULL,
	[{nameof(SyncRequestSqlEntity.SyncStatusLabel)}] [nvarchar](450) NOT NULL,
	[{nameof(SyncRequestSqlEntity.HappenedAt)}] [datetime2](7) NOT NULL,
	[{nameof(SyncRequestSqlEntity.HappenedAtTicks)}] [bigint] NOT NULL,

	{nameof(SyncRequestSqlEntity.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


{nameof(SyncRequestSqlEntity.PayloadIdentifier).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
{nameof(SyncRequestSqlEntity.PayloadType).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
{nameof(SyncRequestSqlEntity.SyncStatus).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
{nameof(SyncRequestSqlEntity.HappenedAt).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
{nameof(SyncRequestSqlEntity.HappenedAtTicks).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(SyncRequest)}")}
",
            },
            new SqlMigration
            {
                ResourceIdentifier = nameof(SyncRequest),
                VersionNumber = new VersionNumber(1, 1),
                SqlCommand = $@"
ALTER TABLE [dbo].[H.Necessaire.{nameof(SyncRequest)}] ADD [{nameof(SyncRequestSqlEntity.OperationContextJson)}] [ntext] NULL;
",
            }
        };
    }
}
