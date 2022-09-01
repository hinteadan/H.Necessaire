using H.Necessaire.Analytics;
using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Analytics.Resources.Concrete
{
    internal partial class NetworkAddressAnalyticsSqlServerProviderResource
    {
        const string networkTraceTableName = "H.Necessaire.NetworkTrace";
        const string networkAddressAnalyticsViewName = "H.Necessaire.NetworkAddressAnalytics";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[]
        {
            new SqlMigration
            {
                ResourceIdentifier = nameof(IpAddressNetworkTrace),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE VIEW [{networkAddressAnalyticsViewName}] AS
SELECT
	[IpAddress] AS [IpAddress]
	,MAX([AsOfTicks]) AS [LatestVisitTicks]
	,MIN([AsOfTicks]) AS [OldestVisitTicks]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [ConsumerIdentityID]), 'null'), ',') from (select distinct [ConsumerIdentityID] from [{networkTraceTableName}] x where x.[IpAddress] = nt.[IpAddress]) _) AS [ConsumerIdentityIDs]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [NetworkServiceProvider]), 'null'), ',') from (select distinct [NetworkServiceProvider] from [{networkTraceTableName}] x where x.[IpAddress] = nt.[IpAddress]) _) AS [NetworkServiceProviders]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [GeoLocationAddress]), 'null'), ',') from (select distinct [GeoLocationAddress] from [{networkTraceTableName}] x where x.[IpAddress] = nt.[IpAddress]) _) AS [GeoLocationAddresses]
	,(
		'[' + 
		(STRING_AGG(
			'{{'
			+ '""ID"":""' + CONVERT(nvarchar(max), [ID]) + '""'
			+ ',""AsOfTicks"":' + CONVERT(nvarchar(max), [AsOfTicks])
			+ ',""ConsumerIdentityID"":' + ISNULL('""' + CONVERT(nvarchar(max), [ConsumerIdentityID]) + '""', 'null')
			+ ',""NetworkServiceProvider"":' + ISNULL('""' + [NetworkServiceProvider] + '""', 'null')
			+ ',""Location"":' + ISNULL(CONVERT(nvarchar(max), [GeoLocationJson]), 'null')
			+ ',""LocationLabel"":' + ISNULL('""' + [GeoLocationAddress] + '""', 'null')
			+ '}}'
		, ',') WITHIN GROUP ( ORDER BY [AsOfTicks] DESC ))
		+ ']'
	) AS [NetworkTracesJson]
FROM
	[{networkTraceTableName}] AS nt
GROUP BY
	[IpAddress]
",
            },
        };
    }
}
