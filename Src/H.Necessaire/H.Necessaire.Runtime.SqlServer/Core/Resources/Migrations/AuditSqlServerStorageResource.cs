using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class AuditMetadataSqlServerStorageResource
    {
        const string auditMetaTableName = "H.Necessaire.Audit";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = "Audit",
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{auditMetaTableName}] 
(
	[{nameof(AuditMetadataSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(AuditMetadataSqlEntry.AuditedObjectType)}] [nvarchar](450) NOT NULL,
    [{nameof(AuditMetadataSqlEntry.AuditedObjectID)}] [nvarchar](450) NOT NULL,
    [{nameof(AuditMetadataSqlEntry.HappenedAt)}] [datetime2](7) NOT NULL,
    [{nameof(AuditMetadataSqlEntry.HappenedAtTicks)}] [bigint] NOT NULL,
    [{nameof(AuditMetadataSqlEntry.DoneByID)}] [nvarchar](450) NULL,
    [{nameof(AuditMetadataSqlEntry.DoneByIDTag)}] [nvarchar](450) NULL,
    [{nameof(AuditMetadataSqlEntry.DoneByDisplayName)}] [nvarchar](450) NULL,
    [{nameof(AuditMetadataSqlEntry.DoneByJson)}] [ntext] NULL,
    [{nameof(AuditMetadataSqlEntry.ActionType)}] [int] NOT NULL,
	[{nameof(AuditMetadataSqlEntry.ActionTypeLabel)}] [nvarchar](450) NOT NULL,

	{nameof(AuditMetadataSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(auditMetaTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


{nameof(AuditMetadataSqlEntry.AuditedObjectType).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.AuditedObjectID).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.HappenedAt).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.HappenedAtTicks).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.DoneByID).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.DoneByIDTag).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.DoneByDisplayName).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.ActionType).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}

",
            },
            new SqlMigration
            {
                ResourceIdentifier = "Audit",
                VersionNumber = new VersionNumber(1, 1),
                SqlCommand = $@"
ALTER TABLE [dbo].[{auditMetaTableName}] ADD [{nameof(AuditMetadataSqlEntry.AppVersionJson)}] [ntext] NULL;
ALTER TABLE [dbo].[{auditMetaTableName}] ADD [{nameof(AuditMetadataSqlEntry.AppVersionNumber)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[{auditMetaTableName}] ADD [{nameof(AuditMetadataSqlEntry.AppVersionTimestamp)}] [datetime2](7) NULL;
ALTER TABLE [dbo].[{auditMetaTableName}] ADD [{nameof(AuditMetadataSqlEntry.AppVersionBranch)}] [nvarchar](450) NULL;
ALTER TABLE [dbo].[{auditMetaTableName}] ADD [{nameof(AuditMetadataSqlEntry.AppVersionCommit)}] [nvarchar](450) NULL;

{nameof(AuditMetadataSqlEntry.AppVersionNumber).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.AppVersionTimestamp).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.AppVersionBranch).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
{nameof(AuditMetadataSqlEntry.AppVersionCommit).PrintColumnIndexCreationSqlScriptOn(auditMetaTableName)}
",
            }
        };
    }

    internal partial class AuditPayloadSqlServerStorageResource
    {
        const string auditPayloadTableName = "H.Necessaire.AuditPayload";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = "AuditPayload",
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{auditPayloadTableName}] 
(
	[{nameof(AuditPayloadSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(AuditPayloadSqlEntry.PayloadAsJson)}] [ntext] NULL,

	{nameof(AuditPayloadSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(auditPayloadTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

",
            },
        };
    }
}
