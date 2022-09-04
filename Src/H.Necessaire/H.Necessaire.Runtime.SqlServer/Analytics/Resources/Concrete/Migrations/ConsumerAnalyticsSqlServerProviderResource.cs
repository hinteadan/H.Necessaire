using H.Necessaire.Analytics;
using H.Necessaire.Dapper;

namespace H.Necessaire.Runtime.SqlServer.Analytics.Resources.Concrete
{
    internal partial class ConsumerAnalyticsSqlServerProviderResource
    {
        const string networkTraceTableName = "H.Necessaire.NetworkTrace";
        const string consumerAnalyticsViewName = "H.Necessaire.ConsumerAnalytics";

        static readonly SqlMigration[] sqlMigrations = new SqlMigration[]
        {
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerNetworkTrace),
                VersionNumber = new VersionNumber(1, 0),
                SqlCommand = $@"
CREATE VIEW [{consumerAnalyticsViewName}] AS
SELECT
	[ConsumerIdentityID] AS [ConsumerIdentityID]
	,(SELECT TOP 1 [DisplayName] FROM [dbo].[H.Necessaire.ConsumerIdentity] c WHERE c.ID = nt.[ConsumerIdentityID]) AS [ConsumerDisplayName]
	,MAX([AsOfTicks]) AS [LatestVisitTicks]
	,MIN([AsOfTicks]) AS [OldestVisitTicks]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [IpAddress]), 'null'), ',') from (select distinct [IpAddress] from [{networkTraceTableName}] x where x.[ConsumerIdentityID] = nt.[ConsumerIdentityID]) _) AS [IpAddresses]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [GeoLocationAddress]), 'null'), ',') from (select distinct [GeoLocationAddress] from [{networkTraceTableName}] x where x.[ConsumerIdentityID] = nt.[ConsumerIdentityID]) _) AS [GeoLocationAddresses]
	,(
		'[' + 
		(STRING_AGG(
			'{{'
			+ '""ID"":""' + CONVERT(nvarchar(max), [ID]) + '""'
			+ ',""AsOfTicks"":' + CONVERT(nvarchar(max), [AsOfTicks])
			+ ',""IpAddress"":' + ISNULL('""' + CONVERT(nvarchar(max), [IpAddress]) + '""', 'null')
			+ ',""Location"":' + ISNULL(CONVERT(nvarchar(max), [GeoLocationJson]), 'null')
			+ ',""LocationLabel"":' + ISNULL('""' + [GeoLocationAddress] + '""', 'null')
			+ '}}'
		, ',') WITHIN GROUP ( ORDER BY [AsOfTicks] DESC ))
		+ ']'
	) AS [NetworkTracesJson]
FROM
	[{networkTraceTableName}] AS nt
GROUP BY
	[ConsumerIdentityID]
",
            },
            new SqlMigration
            {
                ResourceIdentifier = nameof(ConsumerNetworkTrace),
                VersionNumber = new VersionNumber(1, 1),
                SqlCommand = $@"
ALTER VIEW [{consumerAnalyticsViewName}] AS
SELECT
	[ConsumerIdentityID] AS [ConsumerIdentityID]
	,(SELECT TOP 1 [DisplayName] FROM [dbo].[H.Necessaire.ConsumerIdentity] c WHERE c.ID = nt.[ConsumerIdentityID]) AS [ConsumerDisplayName]
	,MAX([AsOfTicks]) AS [LatestVisitTicks]
	,MIN([AsOfTicks]) AS [OldestVisitTicks]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [IpAddress]), 'null'), ',') from (select distinct [IpAddress] from [{networkTraceTableName}] x where x.[ConsumerIdentityID] = nt.[ConsumerIdentityID]) _) AS [IpAddresses]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [NetworkServiceProvider]), 'null'), ',') from (select distinct [NetworkServiceProvider] from [{networkTraceTableName}] x where x.[ConsumerIdentityID] = nt.[ConsumerIdentityID]) _) AS [NetworkServiceProviders]
	,(select STRING_AGG(ISNULL(CONVERT(nvarchar(max), [GeoLocationAddress]), 'null'), ',') from (select distinct [GeoLocationAddress] from [{networkTraceTableName}] x where x.[ConsumerIdentityID] = nt.[ConsumerIdentityID]) _) AS [GeoLocationAddresses]
	,(
		'[' + 
		(STRING_AGG(
			'{{'
			+ '""ID"":""' + CONVERT(nvarchar(max), [ID]) + '""'
			+ ',""AsOfTicks"":' + CONVERT(nvarchar(max), [AsOfTicks])
			+ ',""IpAddress"":' + ISNULL('""' + CONVERT(nvarchar(max), [IpAddress]) + '""', 'null')
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
	[ConsumerIdentityID]
",
            },
        };
    }
}
