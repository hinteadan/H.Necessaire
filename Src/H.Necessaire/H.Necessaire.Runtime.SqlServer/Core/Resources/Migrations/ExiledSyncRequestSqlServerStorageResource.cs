using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class ExiledSyncRequestSqlServerStorageResource
    {
        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(ExiledSyncRequest),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[H.Necessaire.{nameof(ExiledSyncRequest)}] 
(
	[{nameof(ExiledSyncRequestSqlEntity.ID)}] [nvarchar](450) NOT NULL,
	[{nameof(ExiledSyncRequestSqlEntity.PayloadIdentifier)}] [nvarchar](450) NOT NULL,
	[{nameof(ExiledSyncRequestSqlEntity.PayloadType)}] [nvarchar](450) NOT NULL,
    [{nameof(ExiledSyncRequestSqlEntity.HappenedAt)}] [datetime2](7) NOT NULL,
	[{nameof(ExiledSyncRequestSqlEntity.HappenedAtTicks)}] [bigint] NOT NULL,
    [{nameof(ExiledSyncRequestSqlEntity.SyncRequestJson)}] [ntext] NULL,
    [{nameof(ExiledSyncRequestSqlEntity.SyncRequestProcessingResultJson)}] [ntext] NULL,

	{nameof(ExiledSyncRequestSqlEntity.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn($"H.Necessaire.{nameof(ExiledSyncRequest)}")}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


{nameof(ExiledSyncRequestSqlEntity.PayloadIdentifier).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ExiledSyncRequest)}")}
{nameof(ExiledSyncRequestSqlEntity.PayloadType).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ExiledSyncRequest)}")}
{nameof(ExiledSyncRequestSqlEntity.HappenedAt).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ExiledSyncRequest)}")}
{nameof(ExiledSyncRequestSqlEntity.HappenedAtTicks).PrintColumnIndexCreationSqlScriptOn($"H.Necessaire.{nameof(ExiledSyncRequest)}")}
",
            },
        };
    }
}
