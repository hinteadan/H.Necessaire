using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class QdActionResultSqlServerStorageResource
    {
        const string qdActionResultTableName = "H.Necessaire.QdActionResult";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {

            new SqlMigration
            {
                ResourceIdentifier = "QdActionResult",
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"

CREATE TABLE [dbo].[{qdActionResultTableName}] 
(
	[{nameof(QdActionResultSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
    [{nameof(QdActionResultSqlEntry.QdActionID)}] [uniqueidentifier] NOT NULL,
    [{nameof(QdActionResultSqlEntry.HappenedAt)}] [datetime2](7) NOT NULL,
    [{nameof(QdActionResultSqlEntry.HappenedAtTicks)}] [bigint] NOT NULL,
    [{nameof(QdActionResultSqlEntry.QdActionJson)}] [ntext] NOT NULL,
    [{nameof(QdActionResultSqlEntry.IsSuccessful)}] [bit] NOT NULL,
    [{nameof(QdActionResultSqlEntry.Reason)}] [ntext] NULL,
    [{nameof(QdActionResultSqlEntry.CommentsJson)}] [ntext] NULL,

	{nameof(QdActionResultSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(qdActionResultTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(QdActionResultSqlEntry.QdActionID).PrintColumnIndexCreationSqlScriptOn(qdActionResultTableName)}
{nameof(QdActionResultSqlEntry.HappenedAt).PrintColumnIndexCreationSqlScriptOn(qdActionResultTableName)}
{nameof(QdActionResultSqlEntry.HappenedAtTicks).PrintColumnIndexCreationSqlScriptOn(qdActionResultTableName)}
{nameof(QdActionResultSqlEntry.IsSuccessful).PrintColumnIndexCreationSqlScriptOn(qdActionResultTableName)}

",
            }

        };
    }
}
