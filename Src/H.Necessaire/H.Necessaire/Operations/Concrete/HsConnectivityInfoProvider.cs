using H.Necessaire.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.MAUI.Core
{
    public class HsConnectivityInfoProvider : ImAConnectivityInfoProvider, ImADependency
    {
        public event AsyncEventHandler<ConnectivityInfoChangedEventArgs> OnConnectivityInfoChanged { add => onConnectivityInfoChangedEventRaiser.OnEvent += value; remove => onConnectivityInfoChangedEventRaiser.OnEvent -= value; }

        static readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(2);
        static readonly TimeSpan connectivityInfoTimeout = TimeSpan.FromSeconds(5);
        ImAHealthChecker connectivityChecker;
        ImAPeriodicAction refreshAction;
        ConnectivityInfo latestRaisedConnectivityInfo = null;
        EphemeralType<ConnectivityInfo> connectivityInfo = null;
        readonly SemaphoreSlim refreshConnectivityInfoSemaphore = new SemaphoreSlim(1, 1);
        readonly AsyncEventRaiser<ConnectivityInfoChangedEventArgs> onConnectivityInfoChangedEventRaiser;
        public HsConnectivityInfoProvider()
        {
            onConnectivityInfoChangedEventRaiser = new AsyncEventRaiser<ConnectivityInfoChangedEventArgs>(this);
        }


        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            connectivityChecker = dependencyProvider.Get<ImAHealthChecker>();
            refreshAction = dependencyProvider.Get<ImAPeriodicAction>();
            refreshAction.Start(refreshInterval, RefreshConnectivityInfo);
        }

        public async Task<ConnectivityInfo> GetConnectivityInfo()
        {
            if (connectivityInfo?.IsActive() != true)
                await RefreshConnectivityInfo();

            return connectivityInfo.Payload;
        }

        public async Task ForceRefresh()
        {
            if (connectivityInfo != null)
                connectivityInfo.ExpireAt(DateTime.UtcNow.AddSeconds(-1));

            await RefreshConnectivityInfo();
        }

        async Task RefreshConnectivityInfo()
        {
            await refreshConnectivityInfoSemaphore.WaitAsync();

            try
            {
                if (connectivityInfo?.IsActive() == true)
                {
                    if (latestRaisedConnectivityInfo != connectivityInfo.Payload)
                        await onConnectivityInfoChangedEventRaiser.Raise(connectivityInfo.Payload.RefTo(out latestRaisedConnectivityInfo));
                    return;
                }

                OperationResult<TaggedValue<OperationResult>[]> checkResult = await connectivityChecker.CheckHealth();

                OperationResult defaultHttpOpRes = checkResult.Payload.SingleOrDefault(x => x.Name == "DefaultCheck")?.Value;

                connectivityInfo = new EphemeralType<ConnectivityInfo>
                {
                    Payload = new ConnectivityInfo
                    {
                        HasConnectivity = !checkResult ? false : true,
                        LinkSpeedLevel = !checkResult ? ConnectivityLinkSpeedLevel.NoConnectivity : MapLinkSpeedLevelFrom(defaultHttpOpRes),
                        Reasons = !checkResult ? checkResult.FlattenReasons().ToNonEmptyArray() : defaultHttpOpRes?.FlattenReasons().ToNonEmptyArray(),
                        AvailableProfiles = MapProfilesFrom(defaultHttpOpRes).Distinct().ToArrayNullIfEmpty(),
                        LatestResponseDuration = ParseLatestDurationFrom(defaultHttpOpRes),
                    },
                    ValidFor = connectivityInfoTimeout,
                };

                if (latestRaisedConnectivityInfo != connectivityInfo.Payload)
                    await onConnectivityInfoChangedEventRaiser.Raise(connectivityInfo.Payload.RefTo(out latestRaisedConnectivityInfo));
            }
            finally
            {
                refreshConnectivityInfoSemaphore.Release();
            }
        }

        static ConnectivityLinkSpeedLevel MapLinkSpeedLevelFrom(OperationResult httpOpRes)
        {
            if (httpOpRes is null || !httpOpRes.HasWarnings())
                return ConnectivityLinkSpeedLevel.OK;

            if (httpOpRes.Warnings.Any(w => w.Is("SuperSlow")))
                return ConnectivityLinkSpeedLevel.SuperSlow;

            if (httpOpRes.Warnings.Any(w => w.Is("VerySlow")))
                return ConnectivityLinkSpeedLevel.VerySlow;

            if (httpOpRes.Warnings.Any(w => w.Is("Slow")))
                return ConnectivityLinkSpeedLevel.Slow;

            return ConnectivityLinkSpeedLevel.OK;
        }
        static TimeSpan? ParseLatestDurationFrom(OperationResult httpOpRes)
        {
            if ((httpOpRes?.Comments).IsEmpty())
                return null;

            string comment
                = httpOpRes.Comments.FirstOrDefault(x => x.StartsWith("HttpRequestDurationTicks::"))
                ?? httpOpRes.Comments.FirstOrDefault(x => x.StartsWith("HttpRequestDuration::"))
                ;

            if (comment.IsEmpty())
                return null;

            string value = comment.Substring(comment.IndexOf("::") + "::".Length);

            if (long.TryParse(value, out var ticks))
                return new TimeSpan(ticks);

            if (TimeSpan.TryParse(value, out var span))
                return span;

            return null;
        }
        static IEnumerable<ConnectivityProfile> MapProfilesFrom(OperationResult httpOpRes)
        {
            if ((httpOpRes?.Comments).IsEmpty())
                yield break;

            foreach (var comment in httpOpRes.Comments)
            {
                ConnectivityProfile? profile = ParseCommentAsConnectivityProfile(comment);
                if (profile is null)
                    continue;

                yield return profile.Value;
            }
        }

        static ConnectivityProfile? ParseCommentAsConnectivityProfile(string comment)
        {
            if (comment?.StartsWith("ConnectionProfile::") != true)
                return null;

            if (!Enum.TryParse<ConnectivityProfile>(comment.Substring("ConnectionProfile::".Length), out var parseResult))
                return null;

            return parseResult;
        }
    }
}
