using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class NetworkTraceSqlServerStorageResource
    {
        const string networkTraceTableName = "H.Necessaire.NetworkTrace";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(NetworkTrace),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{networkTraceTableName}] 
(
	[{nameof(NetworkTraceSqlEntry.ID)}] [uniqueidentifier] NOT NULL,
    [{nameof(NetworkTraceSqlEntry.AsOf)}] [datetime2](7) NOT NULL,
    [{nameof(NetworkTraceSqlEntry.AsOfTicks)}] [bigint] NOT NULL,
    [{nameof(NetworkTraceSqlEntry.NetworkTraceProviderJson)}] [ntext] NULL,
    [{nameof(NetworkTraceSqlEntry.NetworkTraceProviderID)}] [uniqueidentifier] NULL,
    [{nameof(NetworkTraceSqlEntry.NetworkTraceProviderIDTag)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.NetworkTraceProviderDisplayName)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.IpAddress)}] [nvarchar](450) NOT NULL,
    [{nameof(NetworkTraceSqlEntry.NetworkServiceProvider)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.Organization)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.ClusterNumber)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.ClusterName)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.GeoLocationJson)}] [ntext] NULL,
    [{nameof(NetworkTraceSqlEntry.GeoLocationAddress)}] [nvarchar](450) NULL,
    [{nameof(NetworkTraceSqlEntry.GeoLocationGpsPosition)}] [nvarchar](450) NULL,
	

	{nameof(NetworkTraceSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(networkTraceTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(NetworkTraceSqlEntry.AsOf).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.AsOfTicks).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderID).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderIDTag).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderDisplayName).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.IpAddress).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkServiceProvider).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.Organization).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.ClusterNumber).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.ClusterName).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.GeoLocationAddress).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}
{nameof(NetworkTraceSqlEntry.GeoLocationGpsPosition).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}

",
            },
            new SqlMigration
            {
                ResourceIdentifier = nameof(NetworkTrace),
                VersionNumber = new VersionNumber(1, 1),
                SqlCommand = $@"

ALTER TABLE [dbo].[{networkTraceTableName}] ADD [{nameof(NetworkTraceSqlEntry.ConsumerIdentityID)}] [uniqueidentifier] NULL;

{nameof(NetworkTraceSqlEntry.ConsumerIdentityID).PrintColumnIndexCreationSqlScriptOn(networkTraceTableName)}

",
            },
        };
    }
}
