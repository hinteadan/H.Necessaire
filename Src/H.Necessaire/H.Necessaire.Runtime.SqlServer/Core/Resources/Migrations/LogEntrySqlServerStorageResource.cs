using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class LogEntrySqlServerStorageResource
    {
        const string logEntryTableName = "H.Necessaire.Log";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(LogEntry),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{logEntryTableName}] 
(
	[{nameof(LogEntrySqlEntry.ID)}] [uniqueidentifier] NOT NULL,
	[{nameof(LogEntrySqlEntry.Level)}] [int] NOT NULL,
    [{nameof(LogEntrySqlEntry.LevelLabel)}] [nvarchar](450) NOT NULL,
    [{nameof(LogEntrySqlEntry.ScopeID)}] [uniqueidentifier] NOT NULL,
    [{nameof(LogEntrySqlEntry.OperationContextJson)}] [ntext] NULL,
    [{nameof(LogEntrySqlEntry.HappenedAt)}] [datetime2](7) NOT NULL,
	[{nameof(LogEntrySqlEntry.HappenedAtTicks)}] [bigint] NOT NULL,
    [{nameof(LogEntrySqlEntry.Message)}] [ntext] NULL,
    [{nameof(LogEntrySqlEntry.Method)}] [nvarchar](450) NULL,
    [{nameof(LogEntrySqlEntry.StackTrace)}] [ntext] NULL,
    [{nameof(LogEntrySqlEntry.Component)}] [nvarchar](450) NULL,
    [{nameof(LogEntrySqlEntry.Application)}] [nvarchar](450) NULL,
    [{nameof(LogEntrySqlEntry.ExceptionJson)}] [ntext] NULL,
    [{nameof(LogEntrySqlEntry.PayloadJson)}] [ntext] NULL,
    [{nameof(LogEntrySqlEntry.NotesJson)}] [ntext] NULL,

    {nameof(LogEntrySqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(logEntryTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


{nameof(LogEntrySqlEntry.ID).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.Level).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.LevelLabel).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.ScopeID).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.HappenedAt).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.HappenedAtTicks).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.Method).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.Component).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}
{nameof(LogEntrySqlEntry.Application).PrintColumnIndexCreationSqlScriptOn(logEntryTableName)}

"
            }
        };
    }
}
