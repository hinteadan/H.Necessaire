using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.ReactAppSample.Pages;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;
using System;

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

            result = result.AddFor<AppPageDispatcherAction<DataViewPage>>(x => new DataViewPage(new DataViewPage.Props { NavigationParams = x.NavigationParams }));
            AddRelativeRoute(
                routeDetails: RouteBuilder.Empty.Fixed("view").Fixed("testdata").String(x => x?.ToString()),
                routeActionGenerator: id => new AppPageDispatcherAction<DataViewPage>(new UiNavigationParams(id.ParseToGuidOrFallbackTo(Guid.Empty).Value)),
                urlGenerator: id => GetPath("view", "testdata", id)
            );

            return result;
        }
    }
}
