using Bridge;
using Bridge.Html5;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core
{
    public static class UserAgentData
    {
        public static Task<OperationResult<ConsumerPlatformInfo>> GetPlatformDetails()
        {
            TaskCompletionSource<OperationResult<ConsumerPlatformInfo>> result = new TaskCompletionSource<OperationResult<ConsumerPlatformInfo>>();

            new Action(() =>
            {
                GetHighEntropyValues("platformVersion", "platform", "mobile", "brands")
                .then(x =>
                {
                    result.TrySetResult(Map(GetUserAgentPlaformInfo().And(p =>
                    {
                        p.IdentityAttributes = x.brands.Select(b => new Note(b.brand, b.version)).ToArray();
                        p.PlatformVersion = x.platformVersion;
                        p.Platform = x.platform;
                        p.IsMobile = x.mobile;
                    })).ToWinResult());
                })
                .@catch(x =>
                {
                    result.TrySetResult(Map(GetUserAgentPlaformInfo()).ToWinResult("GetHighEntropyValues not supported on this browser"));
                })
                ;
            }).TryOrFailWithGrace(
                onFail: ex => result.TrySetResult(Map(GetUserAgentPlaformInfo()).ToWinResult("GetHighEntropyValues not supported on this browser"))
            );

            return result.Task;
        }

        private static Brand[] GetBrands() => Script.Eval<Brand[]>("try { window.navigator.userAgentData.brands } catch (err) { null }");
        private static bool? GetIsMobile() => Script.Eval<bool>("try { window.navigator.userAgentData.mobile } catch (err) { null }");
        private static string GetPlatform() => Script.Eval<string>("try { window.navigator.userAgentData.platform } catch (err) { null }");

        private static BrowserPlatformInfo GetUserAgentPlaformInfo()
        {
            return
                new BrowserPlatformInfo
                {
                    IdentityAttributes = GetBrands()?.Select(b => new Note(b.brand, b.version)).ToArray(),
                    IsMobile = GetIsMobile(),
                    Platform = GetPlatform(),
                };
        }

        private static Promise<HighEntropyValues> GetHighEntropyValues(params string[] claims)
            => Script.Eval<Promise<HighEntropyValues>>($"try {{ window.navigator.userAgentData && window.navigator.userAgentData.getHighEntropyValues && window.navigator.userAgentData.getHighEntropyValues([{string.Join(",", claims.Select(x => $"'{x}'"))}]) }} catch (err) {{ null }}");

        private static ConsumerPlatformInfo Map(BrowserPlatformInfo platformInfo)
        {
            return
                new ConsumerPlatformInfo
                {
                    IsMobile = platformInfo.IsMobile,
                    IsWindows = platformInfo.IsWindows,
                    Platform = platformInfo.Platform,
                    PlatformVersion = platformInfo.PlatformVersion,
                    UserAgent = platformInfo.UserAgent,
                    WindowsVersion = platformInfo.WindowsVersion,
                    Attributes = platformInfo.IdentityAttributes,
                };
        }

        private class BrowserPlatformInfo
        {
            public Note[] IdentityAttributes { get; set; }
            public string PlatformVersion { get; set; }
            public string Platform { get; set; }
            public bool? IsMobile { get; set; }

            public string UserAgent => Window.Navigator.UserAgent;
            public bool? IsWindows => string.IsNullOrWhiteSpace(Platform) ? null as bool? : Platform.StartsWith("win", StringComparison.InvariantCultureIgnoreCase);
            public string WindowsVersion => ParseWindowsVersion();

            private string ParseWindowsVersion()
            {
                if (IsWindows != true || string.IsNullOrWhiteSpace(PlatformVersion))
                    return null;

                int? majorPlatformVersion
                    = PlatformVersion
                    .Split(".".AsArray(), StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.ParseToIntOrFallbackTo(null)
                    ;

                if (majorPlatformVersion == null)
                    return null;

                if (majorPlatformVersion >= 13)
                    return "11";

                if (majorPlatformVersion > 0)
                    return "10";

                return null;
            }
        }

        private class HighEntropyValues
        {
            public Brand[] brands { get; set; }
            public string platformVersion { get; set; }
            public string platform { get; set; }
            public bool? mobile { get; set; }
        }

        private class Brand
        {
            public string brand { get; set; }
            public string version { get; set; }
        }
    }
}
