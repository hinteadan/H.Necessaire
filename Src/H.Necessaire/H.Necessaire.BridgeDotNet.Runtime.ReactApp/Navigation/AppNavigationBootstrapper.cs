using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;
using System;
using System.Collections.Generic;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AppNavigationBootstrapper
    {
        public static event EventHandler<PreNavigationEventArgs> OnPreNavigation;

        #region Construct
        static bool isHashNavigationDisabled = false;
        readonly AppNavigationRegistryBase appNavigationRegistry;
        static readonly ControllableDispatcher dispatcher = new ControllableDispatcher();
        static readonly ControllableRouter historyHandler = new ControllableRouter();

        public AppNavigationBootstrapper(Func<IDispatcher, AppNavigationRegistryBase> navigationRegistryFactory)
        {
            this.appNavigationRegistry = navigationRegistryFactory(dispatcher);
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

        private void HandleHashChange(string currentLocation)
        {
            PreNavigationEventArgs preNavigationEventArgs = new PreNavigationEventArgs();
            OnPreNavigation?.Invoke(this, preNavigationEventArgs);
            if (preNavigationEventArgs.IsNavigationCancelled)
                return;

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

        public static void PauseHashNavigation()
        {
            dispatcher.IsDisabled = true;
            historyHandler.IsDisabled = true;
            isHashNavigationDisabled = true;
        }
        public static void ResumeHashNavigation()
        {
            dispatcher.IsDisabled = false;
            historyHandler.IsDisabled = false;
            isHashNavigationDisabled = false;
        }


        private class ControllableRouter : IInteractWithBrowserRouting
        {
            readonly Html5HistoryRouter historyHandler;
            public ControllableRouter()
            {
                this.historyHandler = Html5HistoryRouter.Instance;
            }

            public bool IsDisabled { get; set; } = false;

            public UrlDetails CurrentLocation => historyHandler.CurrentLocation;

            public Optional<UrlDetails> LastNavigatedToUrl => historyHandler.LastNavigatedToUrl;

            public void NavigateTo(UrlDetails url)
            {
                if (IsDisabled)
                    return;

                historyHandler.NavigateTo(url);
            }

            public void RaiseNavigateToForCurrentLocation()
            {
                if (IsDisabled)
                    return;

                historyHandler.RaiseNavigateToForCurrentLocation();
            }

            public void RegisterForNavigatedCallback(Action<UrlDetails> callback)
            {
                historyHandler.RegisterForNavigatedCallback(callback);
            }
        }

        private class ControllableDispatcher : IDispatcher
        {
            readonly IDispatcher dispatcher;
            public ControllableDispatcher()
            {
                this.dispatcher = new AppDispatcher();
            }

            public bool IsDisabled { get; set; } = false;

            public void Dispatch(IDispatcherAction action)
            {
                if (IsDisabled)
                    return;

                dispatcher.Dispatch(action);
            }

            public void HandleServerAction(IDispatcherAction action) => dispatcher.HandleServerAction(action);

            public void HandleViewAction(IDispatcherAction action) => dispatcher.HandleViewAction(action);

            public DispatchToken Receive(Action<IDispatcherAction> callback) => dispatcher.Receive(callback);

            public DispatchToken Register(Action<DispatcherMessage> callback) => dispatcher.Register(callback);

            public void Unregister(DispatchToken token) => dispatcher.Unregister(token);

            public void WaitFor(IEnumerable<DispatchToken> tokens) => dispatcher.WaitFor(tokens);

            public void WaitFor(params DispatchToken[] tokens) => dispatcher.WaitFor(tokens);
        }
    }
}
