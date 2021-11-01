using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.ReactAppSample.Pages;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Helpers;

namespace H.Necessaire.ReactAppSample
{
    public class AppNavigationRegistry : AppNavigationRegistryBase
    {
        public AppNavigationRegistry(IDispatcher dispatcher) : base(dispatcher)
        {
        }

        protected override NavigateActionMatcher RegisterPageRoutes()
        {
            NavigateActionMatcher result = base.RegisterPageRoutes();

            result = result.AddFor<AppPageDispatcherAction<HomePage>>(x => new HomePage());
            AddRelativeRoute(
                segments: new NonNullList<string>(AppBase.BaseHostPathParts).Add("home"),
                routeAction: new AppPageDispatcherAction<HomePage>(),
                urlGenerator: () => GetPath("home")
            );

            return result;
        }
    }
}
