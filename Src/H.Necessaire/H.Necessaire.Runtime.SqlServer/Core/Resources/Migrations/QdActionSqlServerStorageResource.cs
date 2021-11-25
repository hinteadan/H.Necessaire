using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class QdActionSqlServerStorageResource
    {
        const string qdActionTableName = "H.Necessaire.QdAction";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {

            new SqlMigration
            {
                ResourceIdentifier = "QdAction",
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"

CREATE TABLE [dbo].[{qdActionTableName}] 
(
	[{nameof(QdActionSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
    [{nameof(QdActionSqlEntry.QdAt)}] [datetime2](7) NOT NULL,
    [{nameof(QdActionSqlEntry.QdAtTicks)}] [bigint] NOT NULL,
    [{nameof(QdActionSqlEntry.Type)}] [nvarchar](450) NOT NULL,
    [{nameof(QdActionSqlEntry.Payload)}] [ntext] NULL,
    [{nameof(QdActionSqlEntry.Status)}] [int] NOT NULL,
    [{nameof(QdActionSqlEntry.StatusLabel)}] [nvarchar](450) NOT NULL,
    [{nameof(QdActionSqlEntry.RunCount)}] [int] NOT NULL,

	{nameof(QdActionSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(qdActionTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(QdActionSqlEntry.QdAt).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}
{nameof(QdActionSqlEntry.QdAtTicks).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}
{nameof(QdActionSqlEntry.Type).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}
{nameof(QdActionSqlEntry.Status).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}
{nameof(QdActionSqlEntry.StatusLabel).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}
{nameof(QdActionSqlEntry.RunCount).PrintColumnIndexCreationSqlScriptOn(qdActionTableName)}

",
            }

        };
    }
}
