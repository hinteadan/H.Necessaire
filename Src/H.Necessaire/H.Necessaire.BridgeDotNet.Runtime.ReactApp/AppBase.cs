using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Components;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState.Abstract;
using H.Necessaire.Models.Branding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AppBase
    {
        static ImALogger appLogger = null;
        static ImAnAppWireup appWireup;

        static bool isSecurityContextRestoreDisabled = false;
        static Func<Task> appInitializer;

        static Func<IDispatcher, AppNavigationRegistryBase> navigationRegistryFactory;

        static BrandingStyle branding = BrandingStyle.Default;
        static string[] extraLibs = null;

        public static Element CurtainContainer { get; private set; }

        public static RuntimeConfig Config => appWireup?.DependencyRegistry?.GetRuntimeConfig();

        public static T Get<T>() => appWireup.DependencyRegistry.Get<T>();
        public static ImALogger GetLoggerFor(string component) => appWireup.DependencyRegistry.GetLogger(component, "H.Necessaire.BridgeDotNet.Runtime.ReactApp");
        public static ImALogger GetLoggerFor(Type type) => appWireup.DependencyRegistry.GetLogger(type, "H.Necessaire.BridgeDotNet.Runtime.ReactApp");
        public static ImALogger GetLogger<T>() => appWireup.DependencyRegistry.GetLogger<T>("H.Necessaire.BridgeDotNet.Runtime.ReactApp");

        public static async Task<Version> GetAppVersion() => (await appWireup?.DependencyRegistry?.Get<ImAVersionProvider>()?.GetCurrentVersion());

        public static ImALogger AppLogger
        {
            get
            {
                if (appLogger == null)
                    appLogger = AppBase.GetLoggerFor("App");
                return appLogger;
            }
        }

        public static BrandingStyle Branding => branding;

        public static readonly string[] BaseHostPathParts = Window.Document["isWebWorkerContext"] == null ? Window.Location.PathName.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries) : new string[0];

        public static readonly string BaseHostPath = "/" + string.Join("/", BaseHostPathParts);

        public static string BaseUrl => Config.Get("BaseUrl")?.ToString() ?? ((BaseHostPathParts?.Length ?? 0) > 1 ? Window.Location.Origin + "/" + string.Join("/", BaseHostPathParts) : Window.Location.Origin);

        public static string BaseApiUrl => Config.Get("BaseApiUrl")?.ToString() ?? ((BaseHostPathParts?.Length ?? 0) > 1 ? Window.Location.Origin + "/" + string.Join("/", BaseHostPathParts.Take(BaseHostPathParts.Length - 1)) : Window.Location.Origin);

        public static bool IsWebWorker => Window.Document["isWebWorkerContext"] != null;

        public static string WebWorkerID => Window.Document["webWorkerId"]?.ToString();

        public static bool IsOnline => Window.Navigator.OnLine;

        public static void Initialize(
            ImAnAppWireup appWireup,
            Func<IDispatcher, AppNavigationRegistryBase> navigationRegistryFactory,
            Func<Task> appInitializer,
            BrandingStyle branding = null,
            string[] extraLibs = null,
            bool isSecurityContextRestoreDisabled = false
        )
        {
            AppBase.branding = branding ?? BrandingStyle.Default;
            AppBase.appInitializer = appInitializer;
            AppBase.appWireup = appWireup;
            AppBase.navigationRegistryFactory = navigationRegistryFactory;
            AppBase.extraLibs = extraLibs;
            AppBase.isSecurityContextRestoreDisabled = isSecurityContextRestoreDisabled;
        }

        public static async void MainAsWebWorker(ImAnAppWireup webWorkerWireup)
        {
            if (!IsWebWorker)
                return;

            appWireup = webWorkerWireup ?? new ConcreteCoreAppWireup();
            await WireupDependencies();
        }

        protected static async void MainAsync()
        {
            CallContext<Guid?>.SetData(CallContextKey.LoggingScopeID, Guid.NewGuid());

            if (IsWebWorker)
                return;

            using (new TimeMeasurement(x => Console.WriteLine($"App initialized in {x}")))
            {
                using (new AppLoadIndicator())
                {
                    await ReferenceLibs();
                }

                await WireupDependencies();

                SetGlobalStyling();

                WireupNavigation();

                CurtainContainer = CreateCurtainContainer();

                if (appInitializer != null)
                {
                    await appInitializer();
                }

                if (isSecurityContextRestoreDisabled != true)
                {
                    using (new TimeMeasurement(x => AppLogger.LogInfo($"DONE restoring security context in {x}")))
                    {
                        await RestoreSecurityContextFromSessionIfAny();
                    }
                }

                await StartDaemons();
            }
        }

        private static async Task StartDaemons()
        {
            ImADaemon[] daemons = (Get<ImADaemon>()?.AsArray() ?? new ImADaemon[0]).Concat(Get<ImADaemon[]>() ?? new ImADaemon[0]).ToArray();

            if (!daemons.Any())
                return;

            foreach (ImADaemon daemon in daemons)
            {
                await daemon.Start();
            }
        }

        private static async Task WireupDependencies()
        {
            using (new TimeMeasurement(x => Console.WriteLine($"Done Wireup Dependencies in {x}")))
            {
                appWireup.WithEverything();

                if (!IsWebWorker)
                    await appWireup.Boot();
            }
        }

        private static void WireupNavigation()
        {
            using (new TimeMeasurement(async x => await AppLogger.LogTrace($"Done Wireup App Navigation in {x}")))
            {
                new AppNavigationBootstrapper(navigationRegistryFactory).Wireup(CreateAppContainer());
            }
        }

        private static Element CreateAppContainer()
        {
            HTMLDivElement appContainer = new HTMLDivElement();
            appContainer.Id = "AppContainer";
            appContainer.Style.Height = "100%";
            appContainer.Style.Width = "100%";
            Document.Body.AppendChild(appContainer);
            return appContainer;
        }

        private static Element CreateCurtainContainer()
        {
            HTMLDivElement curtainContainer = new HTMLDivElement();
            curtainContainer.Id = "CurtainContainer";
            curtainContainer.Style.Position = Position.Fixed;
            curtainContainer.Style.Width = "1px";
            curtainContainer.Style.Height = "1px";
            curtainContainer.Style.Top = "0px";
            curtainContainer.Style.Left = "0px";
            curtainContainer.Style.ZIndex = int.MaxValue.ToString();
            Document.Body.AppendChild(curtainContainer);
            return curtainContainer;
        }

        private static void SetGlobalStyling()
        {
            DisableMobileResize();

            SetAppIconLinksInHeader();

            ReferenceAndSetGlobalCssAndStyles();

            ReferenceFonts();

            SetGlobalFontsStyle();
        }

        private static void SetGlobalFontsStyle()
        {
            jQuery.Select("body").Css(new
            {
                color = branding.TextColor.ToCssRGBA(),
                fontFamily = branding.Typography.FontFamily,
                fontSize = branding.Typography.FontSize.PointsCss,
            });
        }

        private static void SetAppIconLinksInHeader()
        {
            Document
                .Head
                .AppendChild(
                    new HTMLLinkElement
                    {
                        Rel = "apple-touch-icon",
                        Href = "/apple-touch-icon.png",
                    }
                    .And(x => x.Sizes.Add("180x180"))
                );

            Document
                .Head
                .AppendChild(
                    new HTMLLinkElement
                    {
                        Rel = "icon",
                        Type = "image/png",
                        Href = "/favicon-32x32.png",
                    }
                    .And(x => x.Sizes.Add("32x32"))
                );

            Document
                .Head
                .AppendChild(
                    new HTMLLinkElement
                    {
                        Rel = "icon",
                        Type = "image/png",
                        Href = "/favicon-16x16.png",
                    }
                    .And(x => x.Sizes.Add("16x16"))
                );

            Document
                .Head
                .AppendChild(
                    new HTMLLinkElement
                    {
                        Rel = "manifest",
                        Href = "/site.webmanifest",
                    }
                );
        }

        private static void ReferenceFonts()
        {
            Document.Head.AppendChild(new HTMLLinkElement
            {
                Href = "https://fonts.googleapis.com",
                Rel = "preconnect",
            });

            Document.Head.AppendChild(new HTMLLinkElement
            {
                Href = "https://fonts.gstatic.com",
                Rel = "preconnect",
            }).And(x => x["crossorigin"] = "crossorigin");

            foreach (string fontFamilyUrl in branding?.Typography?.FontFamilyUrls ?? new string[0])
            {
                Document.Head.AppendChild(new HTMLLinkElement
                {
                    Href = fontFamilyUrl,
                    Rel = "stylesheet",
                });
            }
        }

        private static void ReferenceAndSetGlobalCssAndStyles()
        {
            string animationDuration = "0.4s";
            string blurAmount = "2px";

            Document.Head.AppendChild(new HTMLLinkElement
            {
                Href = "/fabric.min.css",
                Rel = "stylesheet",
            });

            Document.Head.AppendChild(new HTMLLinkElement
            {
                Href = "css/fa.all.min.css",
                Rel = "stylesheet",
            });

            jQuery.Select("html, body").Css(new
            {
                width = "100%",
                height = "100%",
                margin = 0,
                padding = 0,
                backgroundColor = branding.BackgroundColorTranslucent.ToCssRGBA(),
            });

            Document.Head.AppendChild(new HTMLStyleElement
            {
                InnerHTML = @"
a:link, a:visited, a:hover, a:active, a:focus {
    color: inherit;
    cursor: pointer;
    text-decoration: underline;
}

a:focus, a:hover {
    opacity: 0.8;
}

input, select, textarea, button {
    font-family: " + branding.Typography.FontFamily + @";
    padding: " + branding.SizingUnitInPixels / 2 + @"px;
    font-size: " + branding.Typography.FontSize.PointsCss + @";
    border-radius: " + branding.SizingUnitInPixels / 5 + @"px;
    border: solid 1px rgba(0, 0, 0, 0.45);
    background-color: rgba(255, 255, 255, 0.83);
    margin: " + branding.SizingUnitInPixels / 2 + @"px;
}

button {
    cursor: pointer;
}

.mouse-highlight:hover {
    background-color: " + branding.SecondaryColorTranslucent.ToCssRGBA() + @"!important;
}

table {
    border-collapse: collapse;
    border-spacing: 0;
}

table th, table td {
    padding: " + branding.SizingUnitInPixels / 2 + @"px;
}

table thead th {
    background-color: " + branding.PrimaryColor.ToCssRGBA() + @";
}

table tbody tr:nth-child(even) td {
    background-color: " + branding.PrimaryColorFaded.ToCssRGBA() + @";
}

table tbody tr:hover td {
    background-color: " + branding.PrimaryColorTranslucent.ToCssRGBA() + @";
}

.underline {
    text-decoration: underline;
    text-underline-offset: 10px;
    text-decoration-style: dotted;
    text-decoration-thickness: 1px;
}

.underline-on-hover {
    transition: " + animationDuration + @";
    text-decoration: underline;
    text-decoration-color: rgba(0, 0, 0, 0);
    text-underline-offset: 10px;
    text-decoration-style: dotted;
    text-decoration-thickness: 1px;
}

.underline-on-hover:hover {
    text-decoration-color: inherit;
}

.opaque-on-hover {
    opacity: 0.63;
    transition: " + animationDuration + @";
}

.opaque-on-hover:hover { 
    opacity: 1;
}

.noselect {
  -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Old versions of Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome, Edge, Opera and Firefox */
}

h1, h2, h3, h4, h5, h6
{
    color: " + Branding.Colors.Primary.Darker(3).ToCssRGBA() + @"
}

p {
    margin-top: " + Branding.SizingUnitInPixels + @"px;
    margin-bottom: " + Branding.SizingUnitInPixels + @"px;
}

.markdown-view pre {
    overflow-x: auto;
}


.markdown-view pre code 
{
    width: 0;
    display: inline-block;
}

.markdown-view pre {
    background-color: " + Branding.Colors.Complementary.Lighter(1).Clone().And(x => x.Opacity = .83f).ToCssRGBA() + @";
    padding: " + Branding.SizingUnitInPixels / 2 + @"px;
    color: " + Branding.TextColor.ToCssRGBA() + @";
}

.markdown-view code {
    font-family: 'Fira Code', monospace;
}

.markdown-view strong {
    color: " + Branding.Colors.Primary.Color.ToCssRGBA() + @"
}

@keyframes morph-in {
    from {
        scale: 1.05; 
        opacity: .1;
        -webkit-filter: blur(" + blurAmount + @");
        -moz-filter: blur(" + blurAmount + @");
        -o-filter: blur(" + blurAmount + @");
        -ms-filter: blur(" + blurAmount + @");
        filter: blur(" + blurAmount + @");
    }
}

.animate {
    animation-duration: " + animationDuration + @";
    animation-timing-function: ease;
}

.animate.morph-in {
    animation-name: morph-in;
}

.blur-underneath {
    backdrop-filter: blur(" + blurAmount + @");
}

",
            });

            Document.Head.AppendChild(new HTMLLinkElement { Rel = "stylesheet", Type = "text/css", Href = "/highlight.min.css" });
            Document.Head.AppendChild(new HTMLLinkElement { Rel = "stylesheet", Type = "text/css", Href = "/stackoverflow-light.min.css" });
            Document.Head.AppendChild(new HTMLLinkElement { Rel = "stylesheet", Type = "text/css", Href = "/fira_code.css" });
        }

        private static void DisableMobileResize()
        {
            Document.Head.AppendChild(new HTMLMetaElement
            {
                Name = "viewport",
                Content = "width=device-width, user-scalable=no",
            });
        }

        private static async Task ReferenceLibs()
        {
            using (new TimeMeasurement(x => Console.WriteLine($"Done referencing necessaire libraries in {x}")))
            {
                jQuery.AjaxSetup(new AjaxOptions
                {
                    Cache = true,
                });

                await ReferenceLib("/dexie.js");
                Script.Write("window.dexie = Dexie;");

                await ReferenceLib("/react.production.min.js");
                await ReferenceLib("/react-dom.production.min.js");
                await ReferenceLib("/marked.min.js");
                await ReferenceLib("/highlight.min.js");

                if (extraLibs?.Any() == true)
                {
                    foreach (string lib in extraLibs)
                    {
                        await ReferenceLib(lib);
                    }
                }
            }
        }

        protected static Task ReferenceLib(string lib)
        {
            return Task.FromPromise<object>(
                jQuery.GetScript(lib),
                new Action(() => Console.WriteLine($"Loaded {lib}")),
                new Action(() => Console.WriteLine($"Error loading {lib}"))
            );
        }

        protected static async Task RestoreSecurityContextFromSessionIfAny()
        {
            jQuery appContainer = jQuery.Select("#AppContainer");

            using (new ScopedRunner(
                onStart: () => appContainer.Hide(),
                onStop: () => appContainer.Show()
                ))
            {
                await Get<SecurityManager>().RestoreSecurityContextIfPossible();
                Navi.GoHome();
            }
        }
    }
}
