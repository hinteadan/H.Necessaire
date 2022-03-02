using DeviceDetectorNET;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;
using H.Necessaire.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.QdActions.Processors
{
    internal class RuntimePlatformProcessor : QdActionProcessorBase
    {
        protected override string[] SupportedQdActionTypes { get; } = WellKnown.QdActionType.ProcessRuntimePlatform.AsArray();
        static readonly InternalIdentity deviceDetectorDotNETProvider = new InternalIdentity
        {
            ID = Guid.Parse("{2C196A6B-DD9D-4B6F-BEA2-2F06E2695637}"),
            AsOf = new DateTime(2022, 3, 2, 14, 0, 0, DateTimeKind.Utc),
            DisplayName = "Detector.NET",
            Notes = new Note[] {
                "UserAgent parsing".NoteAs("Underlying Method"),
                "https://github.com/totpero/DeviceDetector.NET".NoteAs("ProjectURL"),
                "https://www.nuget.org/packages/DeviceDetector.NET".NoteAs("NugetURL"),
            },
        };
        static readonly InternalIdentity deviceDetectorDotNETPlusUserAgentClientHintsProvider = new InternalIdentity
        {
            ID = Guid.Parse("{85108BA1-5BD5-40A7-99DC-6AD7F7FED8A7}"),
            AsOf = new DateTime(2022, 3, 2, 14, 0, 0, DateTimeKind.Utc),
            DisplayName = "Detector.NET + UserAgent Client Hints API Decorator",
            Notes = new Note[] {
                "UserAgent parsing and decorator based on User-Agent Client Hints API (window.navigator.userAgentData.getHighEntropyValues())".NoteAs("Underlying Method"),
                "https://github.com/totpero/DeviceDetector.NET".NoteAs("ProjectURL"),
                "https://www.nuget.org/packages/DeviceDetector.NET".NoteAs("NugetURL"),
                "https://developer.mozilla.org/en-US/docs/Web/API/NavigatorUAData/getHighEntropyValues".NoteAs("UserAgentClientHintsDocsURL"),
            },
        };


        ImALogger logger = null;
        ImAStorageService<Guid, RuntimeTrace> runtimeStore = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

            logger = dependencyProvider.GetLogger<RuntimePlatformProcessor>();
            runtimeStore = dependencyProvider.Get<ImAStorageService<Guid, RuntimeTrace>>();
        }

        protected override async Task<OperationResult> ProcessQdAction(QdAction action)
        {
            ConsumerIdentity consumerIdentity = action?.Payload?.JsonToObject<ConsumerIdentity>();

            if (consumerIdentity == null)
                return OperationResult.Win($"Nothing to process, QdAction Payload is empty or cannot be parsed. Payload: {action?.Payload ?? "[NULL]"}");

            OperationResult result = null;

            await
                new Func<Task>(async () =>
                {
                    OperationResult<RuntimeTrace> runtimeTraceOperation = await ProcessRuntimeTraceFor(consumerIdentity);

                    if (!runtimeTraceOperation.IsSuccessful)
                    {
                        result = runtimeTraceOperation;
                        return;
                    }

                    result = await runtimeStore.Save(runtimeTraceOperation.Payload);
                    if (!result.IsSuccessful)
                        return;

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(onFail: async ex =>
                {
                    await logger.LogError(ex, consumerIdentity);
                    result = OperationResult.Fail(ex);
                });

            return result;
        }

        private async Task<OperationResult<RuntimeTrace>> ProcessRuntimeTraceFor(ConsumerIdentity consumerIdentity)
        {
            OperationResult<RuntimeTrace> result = OperationResult.Fail("Not yet started").WithoutPayload<RuntimeTrace>();

            await
                new Func<Task>(() =>
                {
                    string userAgent = consumerIdentity.RuntimePlatform?.UserAgent ?? consumerIdentity.UserAgent;

                    RuntimeTrace runtimeTrace
                        = (!string.IsNullOrWhiteSpace(userAgent) ? ProcessUserAgent(userAgent) : new RuntimeTrace())
                        .And(x =>
                        {
                            x.ConsumerIdentityID = consumerIdentity.ID;
                        });

                    runtimeTrace = DecorateRuntimeTraceConsumerContext(runtimeTrace, consumerIdentity);

                    runtimeTrace = DecorateRuntimeTraceWithPlatformInfo(runtimeTrace, consumerIdentity?.RuntimePlatform);

                    runtimeTrace.Notes = runtimeTrace.Notes.Push(userAgent.NoteAs("UserAgent"), checkDistinct: false);

                    result = runtimeTrace.ToWinResult();

                    return true.AsTask();
                })
                .TryOrFailWithGrace(onFail: async ex =>
                {
                    await logger.LogError(ex, consumerIdentity);
                    result = OperationResult.Fail(ex).WithoutPayload<RuntimeTrace>();
                });

            return result;
        }

        private RuntimeTrace ProcessUserAgent(string userAgent)
        {
            RuntimeTrace result = new RuntimeTrace
            {
                RuntimeTraceProvider = deviceDetectorDotNETProvider,
            };

            new Action(() =>
            {

                DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
                DeviceDetector device = new DeviceDetector(userAgent).And(x => x.Parse());
                ParseResult<DeviceDetectorResult> parseResult = DeviceDetector.GetInfoFromUserAgent(userAgent);

                ParseResult<BotMatchResult> bot = device.GetBot();
                ParseResult<BrowserMatchResult> browser = device.GetBrowserClient();
                ParseResult<ClientMatchResult> client = device.GetClient();
                ParseResult<OsMatchResult> os = device.GetOs();

                result.Device = new RuntimeDeviceInfo
                {
                    Type = ParseDeviceType(device),
                    TypeName = parseResult.Match.DeviceType,
                    Brand = device.GetBrand() ?? parseResult.Match.DeviceBrand,
                    BrandFullName = device.GetBrandName(),
                    Model = device.GetModel() ?? parseResult.Match.DeviceModel,
                    Name = device.GetDeviceName(),
                    HasTouchInteraction = device.IsTouchEnabled(),
                }.And(x =>
                {
                    x.IsMobile = x.Type >= RuntimeDeviceType.MobilePhone && x.Type < RuntimeDeviceType.Tablet;
                });

                result.OperatingSystem = new RuntimeOperatingSystemInfo
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
                    result.Device.Type = result.Device.Type >= RuntimeDeviceType.Desktop && isWindows ? RuntimeDeviceType.DesktopWindows : result.Device.Type;
                    x.IsWindows = isWindows;
                    x.WindowsVersion = isWindows ? x.Version : null;
                });

                result.Bot = new RuntimeBotInfo
                {
                    Category = bot.Success ? bot.Match.Category : parseResult.Match.Bot?.Category,
                    Name = bot.Success ? bot.Match.Name : parseResult.Match.Bot?.Name,
                    OwnerName = bot.Success ? bot.Match.Producer?.Name : parseResult.Match.Bot?.Producer?.Name,
                    OwnerUrl = bot.Success ? bot.Match.Producer?.Url : parseResult.Match.Bot?.Producer?.Url,
                    Url = bot.Success ? bot.Match.Url : parseResult.Match.Bot?.Url,
                };

                result.Browser = new RuntimeBrowserAppInfoInfo
                {
                    Engine = browser.Success ? browser.Match.Engine : null,
                    EngineVersion = browser.Success ? browser.Match.EngineVersion : null,
                    Family = parseResult.Success ? parseResult.Match.BrowserFamily : null,
                    Name = browser.Success ? browser.Match.Name : null,
                    ShortName = browser.Success ? browser.Match.ShortName : null,
                    Type = browser.Success ? browser.Match.Type : null,
                    Version = browser.Success ? browser.Match.Version : null,
                };

                result.Client = new RuntimeClientInfo
                {
                    Name = client.Success ? client.Match.Name : parseResult.Match.Client?.Name,
                    Type = client.Success ? client.Match.Type : parseResult.Match.Client?.Type,
                    Version = client.Success ? client.Match.Version : parseResult.Match.Client?.Version,
                };

            })
            .TryOrFailWithGrace(
                onFail: ex => result.Notes
                    = userAgent.NoteAs("UserAgent").AsArray()
                    .Concat(ex.Flatten().Select((x, i) => x.ToString().NoteAs($"Exception{i}")))
                    .ToArray()
            );

            return result;
        }

        private RuntimeTrace DecorateRuntimeTraceConsumerContext(RuntimeTrace runtimeTrace, ConsumerIdentity consumerIdentity)
        {
            new Action(() =>
            {
                runtimeTrace.Device = (runtimeTrace.Device ?? new RuntimeDeviceInfo { }).And(x =>
                {
                    x.DisplayWidthInPixels = consumerIdentity.Notes.Get(WellKnownConsumerIdentityNote.DisplayWidth).ParseToIntOrFallbackTo(null);
                    x.DisplayHeightInPixels = consumerIdentity.Notes.Get(WellKnownConsumerIdentityNote.DisplayHeight).ParseToIntOrFallbackTo(null);
                    x.AppWidthInPixels = consumerIdentity.Notes.Get(WellKnownConsumerIdentityNote.DisplayWindowWidth).ParseToIntOrFallbackTo(null);
                    x.AppHeightInPixels = consumerIdentity.Notes.Get(WellKnownConsumerIdentityNote.DisplayWindowHeight).ParseToIntOrFallbackTo(null);
                    x.ColorDepthInBits = consumerIdentity.Notes.Get(WellKnownConsumerIdentityNote.DisplayColorDepth).ParseToIntOrFallbackTo(null);
                });

                runtimeTrace.Timing = (runtimeTrace.Timing ?? new RuntimeTimingInfo { }).And(x =>
                {
                    x.Connect = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-ConnectStart"), consumerIdentity.Notes.Get("Performance-Timing-ConnectEnd"));
                    x.DomainLookup = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-DomainLookupStart"), consumerIdentity.Notes.Get("Performance-Timing-DomainLookupEnd"));
                    x.DOM = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-DomLoading"), consumerIdentity.Notes.Get("Performance-Timing-DomComplete"));
                    x.Redirect = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-RedirectStart"), consumerIdentity.Notes.Get("Performance-Timing-RedirectEnd"));
                    x.RequestResponse = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-RequestStart"), consumerIdentity.Notes.Get("Performance-Timing-ResponseEnd"));
                    x.Response = ParsePeriodOfTime(consumerIdentity.Notes.Get("Performance-Timing-ResponseStart"), consumerIdentity.Notes.Get("Performance-Timing-ResponseEnd"));
                });
            })
            .TryOrFailWithGrace();

            return runtimeTrace;
        }

        private RuntimeTrace DecorateRuntimeTraceWithPlatformInfo(RuntimeTrace runtimeTrace, ConsumerPlatformInfo platformInfo)
        {
            if (platformInfo?.IsEmpty() ?? true)
                return runtimeTrace;

            new Action(() =>
            {
                runtimeTrace.RuntimeTraceProvider = deviceDetectorDotNETPlusUserAgentClientHintsProvider;

                runtimeTrace.Device = (runtimeTrace.Device ?? new RuntimeDeviceInfo { }).And(x =>
                {
                    x.IsMobile = platformInfo.IsMobile ?? x.IsMobile;
                });

                runtimeTrace.OperatingSystem = (runtimeTrace.OperatingSystem ?? new RuntimeOperatingSystemInfo { }).And(x =>
                {
                    x.Platform = platformInfo.Platform ?? x.Platform;
                    x.Version = platformInfo.PlatformVersion ?? x.Version;
                    x.IsWindows = platformInfo.IsWindows ?? x.IsWindows;
                    x.WindowsVersion = platformInfo.WindowsVersion ?? x.WindowsVersion;
                });

                runtimeTrace.Notes = runtimeTrace.Notes.Push(platformInfo.Attributes, checkDistinct: false);

            })
            .TryOrFailWithGrace();

            return runtimeTrace;
        }

        private RuntimeDeviceType ParseDeviceType(DeviceDetector device)
        {
            RuntimeDeviceType result = RuntimeDeviceType.Unknown;

            new
                Action(() =>
                {
                    if (device.IsBot())
                    {
                        result = RuntimeDeviceType.Bot;
                        return;
                    }

                    if (device.IsMobile())
                    {
                        result = RuntimeDeviceType.MobilePhone;
                        if (device.HasAndroidMobileFragment() || device.HasAndroidTableFragment())
                            result = RuntimeDeviceType.MobilePhoneAndroid;
                        return;
                    }

                    if (device.IsTablet())
                    {
                        result = RuntimeDeviceType.Tablet;
                        if (device.HasAndroidMobileFragment() || device.HasAndroidTableFragment())
                            result = RuntimeDeviceType.TabletAndroid;
                        return;
                    }

                    if (device.IsDesktop())
                    {
                        result = RuntimeDeviceType.Desktop;
                        return;
                    }
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = RuntimeDeviceType.Unknown
                );

            return result;
        }

        private static PeriodOfTime ParsePeriodOfTime(string from, string to)
        {
            double? fromUnix = from.ParseToDoubleOrFallbackTo(null);
            double? toUinx = to.ParseToDoubleOrFallbackTo(null);
            if (fromUnix == null || toUinx == null)
                return null;

            return
                new PeriodOfTime
                {
                    From = fromUnix.Value.UnixTimeStampToDateTime().ToUniversalTime(),
                    To = toUinx.Value.UnixTimeStampToDateTime().ToUniversalTime(),
                };
        }
    }
}
