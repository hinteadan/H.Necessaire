using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class AppNavigationRegistryBase : Navigator
    {
        #region Construct
        public AppNavigationRegistryBase(IDispatcher dispatcher) : base(dispatcher)
        {

            this.NavigationActionMatcher = RegisterPageRoutes();
        }

        protected virtual bool IsDefaultIndexRouteEnabled => true;
        protected virtual bool IsDefaultInvalidRouteEnabled => true;
        protected virtual bool IsDefaultLoginRouteEnabled => true;
        protected virtual bool IsDefaultUnauthorizedRouteEnabled => true;
        #endregion

        public NavigateActionMatcher NavigationActionMatcher { get; }

        protected virtual NavigateActionMatcher RegisterPageRoutes()
        {
            NavigateActionMatcher result = NavigateActionMatcher.Empty;

            if (IsDefaultInvalidRouteEnabled)
            {
                result = result.AddFor<InvalidRoute>(new NotFoundPage());
            }

            if (IsDefaultIndexRouteEnabled)
            {
                result = result.AddFor<AppPageDispatcherAction<IndexPage>>(x => new IndexPage(new IndexPage.Props { NavigationParams = x.NavigationParams }));
                AddRelativeRoute(
                    segments: new NonNullList<string>(AppBase.BaseHostPathParts),
                    routeActionGenerator: _ => new AppPageDispatcherAction<IndexPage>(new UiNavigationParams(ParseReturnTo())),
                    urlGenerator: () => GetPath()
                );
            }

            if (IsDefaultLoginRouteEnabled)
            {
                result = result.AddFor<AppPageDispatcherAction<LoginPage>>(x => new LoginPage(new LoginPage.Props { NavigationParams = x.NavigationParams }));
                AddRelativeRoute(
                    segments: new NonNullList<string>(AppBase.BaseHostPathParts).Add("login"),
                    routeActionGenerator: _ => new AppPageDispatcherAction<LoginPage>(new UiNavigationParams(ParseReturnTo())),
                    urlGenerator: () => GetPath("login")
                );
            }

            if (IsDefaultUnauthorizedRouteEnabled)
            {
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
            }

            return result;
        }

        protected Note[] ParseQueryString()
        {
            string hash = Bridge.Html5.Window.Location.Hash;
            if (hash.IsEmpty())
                return null;

            int indexOfQueryString = hash.IndexOf('?');
            if (indexOfQueryString < 0)
                return null;

            string rawQueryString = hash.Substring(indexOfQueryString + 1);
            string[] queryStringParts = rawQueryString.Split('&'.AsArray(), System.StringSplitOptions.RemoveEmptyEntries);
            if (!queryStringParts.Any())
                return null;

            Note[] queryParams
                = queryStringParts
                .Select(x => {
                    int equalIndex = x.IndexOf('=');
                    if (equalIndex < 0)
                        return new Note(x.Trim().NullIfEmpty(), null);
                    string[] parts = x.Split('='.AsArray(), 2);
                    return new Note(parts[0].Trim().NullIfEmpty(), parts[1].Trim().NullIfEmpty());
                })
                .ToArray();

            return queryParams;
        }

        protected string ParseReturnTo()
        {
            Note[] queryParams = ParseQueryString();
            string returnTo = queryParams?.Get("returnTo", ignoreCase: true) ?? "";
            return Bridge.Html5.Window.DecodeURIComponent(returnTo);
        }
    }
}
