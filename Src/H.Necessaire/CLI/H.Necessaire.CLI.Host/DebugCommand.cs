using DeviceDetectorNET;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;
using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Runtime.Security.Managers;
using H.Necessaire.Serialization;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        ImASecurityManager securityManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            securityManager = dependencyProvider.Get<ImASecurityManager>();
        }

        public override async Task<OperationResult> Run()
        {
            OperationResult<SecurityContext> loginResult = await securityManager.AuthenticateCredentials("ironman", "123qwe");

            ConsumerInfo consumerInfo = Parse("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36 Edg/98.0.1108.62");

            return OperationResult.Win();
        }

        private ConsumerInfo Parse(string userAgent)
        {
            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
            DeviceDetector? device = new DeviceDetector(userAgent).And(x => x.Parse());
            ParseResult<DeviceDetectorResult>? parseResult = DeviceDetector.GetInfoFromUserAgent(userAgent);

            ParseResult<BotMatchResult>? bot = device.GetBot();
            ParseResult<BrowserMatchResult>? browser = device.GetBrowserClient();
            ParseResult<ClientMatchResult>? client = device.GetClient();
            ParseResult<OsMatchResult>? os = device.GetOs();

            DeviceInfo deviceInfo = new DeviceInfo
            {
                Type = ParseDeviceType(device),
                TypeName = parseResult.Match.DeviceType,
                Brand = device.GetBrand() ?? parseResult.Match.DeviceBrand,
                BrandName = device.GetBrandName(),
                Model = device.GetModel() ?? parseResult.Match.DeviceModel,
                Name = device.GetDeviceName(),
                HasTouchInteraction = device.IsTouchEnabled(),
            };

            DeviceOperatingSystemInfo? osInfo = new DeviceOperatingSystemInfo
            {
                Family = parseResult.Success ? parseResult.Match.OsFamily : null,
                Name = os.Success ? os.Match.Name : parseResult.Match.Os?.Name,
                Platform = os.Success ? os.Match.Platform : parseResult.Match.Os?.Platform,
                ShortName = os.Success ? os.Match.ShortName : parseResult.Match.Os?.ShortName,
                Version = (os.Success ? os.Match.Version : parseResult.Match.Os?.Version),
            }.And(x =>
            {
                bool isWindows = string.Equals(x.ShortName, "win", StringComparison.InvariantCultureIgnoreCase);
                x.Version = !isWindows ? x.Version : x.Version == "10" ? "10+" : x.Version;
                deviceInfo.Type = deviceInfo.Type >= DeviceType.Desktop && isWindows ? DeviceType.DesktopWindows : deviceInfo.Type;
            });


            return
                new ConsumerInfo
                {
                    ID = userAgent,
                    Device = deviceInfo,
                    Bot = new BotRuntimeInfo
                    {
                        Category = bot.Success ? bot.Match.Category : parseResult.Match.Bot?.Category,
                        Name = bot.Success ? bot.Match.Name : parseResult.Match.Bot?.Name,
                        OwnerName = bot.Success ? bot.Match.Producer?.Name : parseResult.Match.Bot?.Producer?.Name,
                        OwnerUrl = bot.Success ? bot.Match.Producer?.Url : parseResult.Match.Bot?.Producer?.Url,
                        Url = bot.Success ? bot.Match.Url : parseResult.Match.Bot?.Url,
                    },
                    Browser = new BrowserAppInfo
                    {
                        Engine = browser.Success ? browser.Match.Engine : null,
                        EngineVersion = browser.Success ? browser.Match.EngineVersion : null,
                        Family = parseResult.Success ? parseResult.Match.BrowserFamily : null,
                        Name = browser.Success ? browser.Match.Name : null,
                        ShortName = browser.Success ? browser.Match.ShortName : null,
                        Type = browser.Success ? browser.Match.Type : null,
                        Version = browser.Success ? browser.Match.Version : null,
                    },
                    Client = new ClientRuntimeInfo
                    {
                        Name = client.Success ? client.Match.Name : parseResult.Match.Client?.Name,
                        Type = client.Success ? client.Match.Type : parseResult.Match.Client?.Type,
                        Version = client.Success ? client.Match.Version : parseResult.Match.Client?.Version,
                    },
                    OperatingSystem = osInfo,
                }
                ;
        }

        private DeviceType ParseDeviceType(DeviceDetector device)
        {
            DeviceType result = DeviceType.Unknown;

            new
                Action(() =>
                {
                    if (device.IsBot())
                    {
                        result = DeviceType.Bot;
                        return;
                    }

                    if (device.IsMobile())
                    {
                        result = DeviceType.MobilePhone;
                        if (device.HasAndroidMobileFragment() || device.HasAndroidTableFragment())
                            result = DeviceType.MobilePhoneAndroid;
                        return;
                    }

                    if (device.IsTablet())
                    {
                        result = DeviceType.Tablet;
                        if (device.HasAndroidMobileFragment() || device.HasAndroidTableFragment())
                            result = DeviceType.TabletAndroid;
                        return;
                    }

                    if (device.IsDesktop())
                    {
                        result = DeviceType.Desktop;
                        return;
                    }
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = DeviceType.Unknown
                );

            return result;
        }
    }

    class ConsumerInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DeviceInfo? Device { get; set; }
        public DeviceOperatingSystemInfo? OperatingSystem { get; set; }
        public ClientRuntimeInfo? Client { get; set; }
        public BrowserAppInfo? Browser { get; set; }
        public BotRuntimeInfo? Bot { get; set; }
    }

    public class BotRuntimeInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string OwnerName { get; set; }
        public string OwnerUrl { get; set; }
    }

    public class DeviceInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DeviceType Type { get; set; } = DeviceType.Unknown;
        public string TypeName { get; set; }
        public bool HasTouchInteraction { get; set; } = false;
        public string Name { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
    }

    public class BrowserAppInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Family { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string ShortName { get; set; }
        public string Engine { get; set; }
        public string EngineVersion { get; set; }
    }

    public class ClientRuntimeInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class DeviceOperatingSystemInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Family { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
    }

    public enum DeviceType
    {
        Bot = -1,

        Unknown = 0,

        MobilePhone = 100,
        MobilePhoneAndroid = 101,
        MobilePhoneApple = 102,

        Tablet = 500,
        TabletAndroid = 501,
        TabletApple = 502,

        Desktop = 1000,
        DesktopWindows = 1001,
        DesktopLinux = 1002,
        DesktopMac = 1003,
        DesktopIos = 1004,
    }
}
