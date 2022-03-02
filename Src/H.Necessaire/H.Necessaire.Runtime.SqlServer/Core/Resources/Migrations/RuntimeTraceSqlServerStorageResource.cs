using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class RuntimeTraceSqlServerStorageResource
    {
        const string runtimeTraceTableName = "H.Necessaire.RuntimeTrace";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(RuntimeTrace),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{runtimeTraceTableName}] 
(
	[{nameof(RuntimeTraceSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
    [{nameof(RuntimeTraceSqlEntry.ConsumerIdentityID)}] [uniqueidentifier] NULL,
    [{nameof(RuntimeTraceSqlEntry.AsOf)}] [datetime2](7) NOT NULL,
    [{nameof(RuntimeTraceSqlEntry.AsOfTicks)}] [bigint] NOT NULL,
    [{nameof(RuntimeTraceSqlEntry.RuntimeTraceProviderJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.DeviceJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.OperatingSystemJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.ClientJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.BrowserJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.BotJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.TimingJson)}] [ntext] NULL,
    [{nameof(RuntimeTraceSqlEntry.NotesJson)}] [ntext] NULL,

	{nameof(RuntimeTraceSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(runtimeTraceTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(RuntimeTraceSqlEntry.ConsumerIdentityID).PrintColumnIndexCreationSqlScriptOn(runtimeTraceTableName)}
{nameof(RuntimeTraceSqlEntry.AsOf).PrintColumnIndexCreationSqlScriptOn(runtimeTraceTableName)}
{nameof(RuntimeTraceSqlEntry.AsOfTicks).PrintColumnIndexCreationSqlScriptOn(runtimeTraceTableName)}

",
            },
        };
    }
}
