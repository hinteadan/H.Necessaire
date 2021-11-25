using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class NetworkTraceSqlServerStorageResource
    {
        const string netowrkTraceTableName = "H.Necessaire.NetworkTrace";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[] {
            new SqlMigration
            {
                ResourceIdentifier = nameof(NetworkTrace),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE TABLE [dbo].[{netowrkTraceTableName}] 
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
	

	{nameof(NetworkTraceSqlEntry.ID).PrintColumnAsPrimaryKeyConstraintSqlScriptOn(netowrkTraceTableName)}
)

ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

{nameof(NetworkTraceSqlEntry.AsOf).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.AsOfTicks).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderID).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderIDTag).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkTraceProviderDisplayName).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.IpAddress).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.NetworkServiceProvider).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.Organization).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.ClusterNumber).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.ClusterName).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.GeoLocationAddress).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}
{nameof(NetworkTraceSqlEntry.GeoLocationGpsPosition).PrintColumnIndexCreationSqlScriptOn(netowrkTraceTableName)}

",
            },
        };
    }
}
