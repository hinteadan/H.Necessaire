using Bridge;
using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class SecuredPageBase<TProps, TState>
        : PageBase<TProps, TState>
        where TState : ImAUiComponentState, new()
        where TProps : ImPageProps
    {
        #region Construct
        protected SecuredPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected virtual PermissionClaim[] NecessaryPermissions { get; }
        #endregion

        public override async Task RunAtStartup()
        {
            await base.RunAtStartup();

            if (SecurityContext == null)
                FlySafe(() => Navi.GoToLogin(returnTo: CurrentLocationPath));

            else if (!SecurityContext.HasPermission(NecessaryPermissions))
                FlySafe(() => Navi.GoToUnauthorized(pageRef: CurrentLocationPath));
        }
    }
}
