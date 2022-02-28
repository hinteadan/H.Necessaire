using Bridge;
using Bridge.Html5;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DeviceInfoResource
    {
        public Task<Note[]> GetRawProperties()
        {
            List<Note> result = new List<Note>();

            result.Add(new Note(WellKnownConsumerIdentityNote.Platform, WellKnownConsumerPlatformType.WebBrowser));
            result.Add(new Note(WellKnownConsumerIdentityNote.Timezone, new Date().GetTimezoneOffset().ToString()));
            result.Add(new Note(WellKnownConsumerIdentityNote.AsOfUtc, DateTime.UtcNow.ToString()));
            result.Add(new Note(WellKnownConsumerIdentityNote.AsOfTicks, DateTime.UtcNow.Ticks.ToString()));
            result.Add(new Note(WellKnownConsumerIdentityNote.IpAddress, string.Empty));
            result.Add(new Note(WellKnownConsumerIdentityNote.Port, string.Empty));

            try
            {
                result.Add(new Note(nameof(Window.Navigator.AppName), Window.Navigator.AppName));
                result.Add(new Note(nameof(Window.Navigator.AppVersion), Window.Navigator.AppVersion));
                result.Add(new Note(nameof(Window.Navigator.Geolocation), (Window.Navigator.Geolocation != null).ToString()));
                result.Add(new Note(nameof(Window.Navigator.Language), Window.Navigator.Language));
                result.Add(new Note("IsOnline", Window.Navigator.OnLine.ToString()));
                result.Add(new Note("CPU", Window.Navigator.Oscpu));
                result.Add(new Note(nameof(Window.Navigator.Platform), Window.Navigator.Platform));
                result.Add(new Note(nameof(Window.Navigator.Product), Window.Navigator.Product));
                result.Add(new Note(nameof(Window.Navigator.UserAgent), Window.Navigator.UserAgent));
            }
            catch (Exception) { }

            try
            {
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayWidth, Window.Screen.Width.ToString()));
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayWindowWidth, Window.InnerWidth.ToString()));
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayHeight, Window.Screen.Height.ToString()));
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayWindowHeight, Window.InnerHeight.ToString()));
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayPixelDepth, Window.Screen.PixelDepth.ToString()));
                result.Add(new Note(WellKnownConsumerIdentityNote.DisplayColorDepth, Script.Eval<string>("window.screen.colorDepth.toString()")));
            }
            catch (Exception) { }

            try
            {
                result.Add(new Note("Performance-NavigationType", PrintNavigationType()));
                result.Add(new Note("Performance-NavigationRedirectCount", Window.Performance.Navigation.RedirectCount.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.ConnectEnd)}", Window.Performance.Timing.ConnectEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.ConnectStart)}", Window.Performance.Timing.ConnectStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomainLookupEnd)}", Window.Performance.Timing.DomainLookupEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomainLookupStart)}", Window.Performance.Timing.DomainLookupStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomComplete)}", Window.Performance.Timing.DomComplete.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomContentLoadedEventEnd)}", Window.Performance.Timing.DomContentLoadedEventEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomContentLoadedEventStart)}", Window.Performance.Timing.DomContentLoadedEventStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomInteractive)}", Window.Performance.Timing.DomInteractive.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.DomLoading)}", Window.Performance.Timing.DomLoading.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.FetchStart)}", Window.Performance.Timing.FetchStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.LoadEventEnd)}", Window.Performance.Timing.LoadEventEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.LoadEventStart)}", Window.Performance.Timing.LoadEventStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.NavigationStart)}", Window.Performance.Timing.NavigationStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.RedirectEnd)}", Window.Performance.Timing.RedirectEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.RedirectStart)}", Window.Performance.Timing.RedirectStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.RequestStart)}", Window.Performance.Timing.RequestStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.ResponseEnd)}", Window.Performance.Timing.ResponseEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.ResponseStart)}", Window.Performance.Timing.ResponseStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.SecureConnectionStart)}", Window.Performance.Timing.SecureConnectionStart.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.UnloadEventEnd)}", Window.Performance.Timing.UnloadEventEnd.ToString()));
                result.Add(new Note($"Performance-Timing-{nameof(Window.Performance.Timing.UnloadEventStart)}", Window.Performance.Timing.UnloadEventStart.ToString()));
            }
            catch (Exception) { }

            return Task.FromResult(result.ToArray());
        }

        private static string PrintNavigationType()
        {
            switch ((int)Window.Performance.Navigation.Type.ValueOf())
            {
                case 0: return "Direct";
                case 1: return "Reload";
                case 2: return "BackOrForward";
                default: return "OtherUnknown";
            }
        }
    }
}
