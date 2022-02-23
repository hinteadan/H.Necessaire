using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.React;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AppNavigationBootstrapper
    {
        #region Construct
        static bool isHashNavigationDisabled = false;
        readonly AppNavigationRegistryBase appNavigationRegistry;
        readonly IDispatcher dispatcher;
        readonly Html5HistoryRouter historyHandler;

        public AppNavigationBootstrapper(Func<IDispatcher, AppNavigationRegistryBase> navigationRegistryFactory)
        {
            this.dispatcher = new AppDispatcher();
            this.appNavigationRegistry = navigationRegistryFactory(this.dispatcher);
            this.historyHandler = Html5HistoryRouter.Instance;
        }
        #endregion

        public void Wireup(Element pageContainer)
        {
            React.Render(
                new RoutingStoreActivatorContainer(dispatcher, appNavigationRegistry.NavigationActionMatcher),
                pageContainer
            );

            RouteCombiner.StartListening(historyHandler, appNavigationRegistry.Routes, dispatcher);

            jQuery.Window.Bind("hashchange", () => HandleHashChange(historyHandler.CurrentLocation?.ToString() ?? AppBase.BaseHostPath));

            if (Window.Location.Hash.IsIndexPath())
                Html5HistoryRouter.Instance.RaiseNavigateToForCurrentLocation();
            else
                HandleHashChange(historyHandler.CurrentLocation?.ToString());
        }

        private static void HandleHashChange(string currentLocation)
        {
            if (isHashNavigationDisabled)
                return;

            string requestedHash = Window.Location.Hash;
            currentLocation = currentLocation == $"{AppBase.BaseHostPath}/" ? AppBase.BaseHostPath : (currentLocation ?? AppBase.BaseHostPath);

            if (currentLocation.IsIndexPath() && requestedHash.IsIndexPath())
                return;

            if ((requestedHash ?? AppBase.BaseHostPath).Equals(currentLocation ?? AppBase.BaseHostPath, StringComparison.InvariantCultureIgnoreCase))
                return;

            if (requestedHash.IsIndexPath())
            {
                Window.History.ReplaceState(null, null, $"{AppBase.BaseUrl}/");
                Html5HistoryRouter.Instance.RaiseNavigateToForCurrentLocation();
                Window.History.ReplaceState(null, null, $"{AppBase.BaseUrl}/#/");
                return;
            }

            Window.History.ReplaceState(null, null, $"{AppBase.BaseUrl}{requestedHash.Substring(1)}");
            Html5HistoryRouter.Instance.RaiseNavigateToForCurrentLocation();
            Window.History.ReplaceState(null, null, $"{AppBase.BaseUrl}/{requestedHash}");
        }

        public static void PauseHashNavigation() => isHashNavigationDisabled = true;
        public static void ResumeHashNavigation() => isHashNavigationDisabled = false;
    }
}
