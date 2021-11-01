using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class AppNavigationRegistryBase : Navigator
    {
        #region Construct
        public AppNavigationRegistryBase(IDispatcher dispatcher) : base(dispatcher)
        {

            this.NavigationActionMatcher = RegisterPageRoutes();
        }
        #endregion

        public NavigateActionMatcher NavigationActionMatcher { get; }

        protected virtual NavigateActionMatcher RegisterPageRoutes()
        {
            NavigateActionMatcher result = NavigateActionMatcher.Empty.AddFor<InvalidRoute>(new NotFoundPage());

            result = result.AddFor<AppPageDispatcherAction<IndexPage>>(x => new IndexPage());
            AddRelativeRoute(
                segments: new NonNullList<string>(AppBase.BaseHostPathParts),
                routeAction: new AppPageDispatcherAction<IndexPage>(),
                urlGenerator: () => GetPath()
            );

            result = result.AddFor<AppPageDispatcherAction<LoginPage>>(x => new LoginPage(new LoginPage.Props { NavigationParams = x.NavigationParams }));
            AddRelativeRoute(
                segments: new NonNullList<string>(AppBase.BaseHostPathParts).Add("login"),
                routeActionGenerator: queryString => new AppPageDispatcherAction<LoginPage>(
                    new UiNavigationParams(
                        Bridge.Html5.Window.DecodeURIComponent(
                            queryString.String("returnTo").ToString()
                        )
                    )
                ),
                urlGenerator: () => GetPath("login")
            );

            result = result.AddFor<AppPageDispatcherAction<UnauthorizedPage>>(x => new UnauthorizedPage(new UnauthorizedPage.Props { NavigationParams = x.NavigationParams }));
            AddRelativeRoute(
                segments: new NonNullList<string>(AppBase.BaseHostPathParts).Add("nogo"),
                routeActionGenerator: queryString => new AppPageDispatcherAction<UnauthorizedPage>(
                    new UiNavigationParams(
                        Bridge.Html5.Window.DecodeURIComponent(
                            queryString.String("ref").ToString()
                        )
                    )
                ),
                urlGenerator: () => GetPath("nogo")
            );


            return result;
        }
    }
}
