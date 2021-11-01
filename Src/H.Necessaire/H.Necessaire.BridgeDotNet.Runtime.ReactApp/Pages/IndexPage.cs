using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class IndexPage : PageBase<IndexPage.Props, IndexPage.State>
    {
        #region Construct
        public IndexPage() : base(new Props(), null) { }

        public override Task RunAtStartup()
        {
            if (props.IsHomePageSecured && SecurityContext == null)
                FlySafe(() => Navi.GoToLogin(returnTo: "/"));
            else
                FlySafe(() => Navi.Go(props.HomePagePath));

            return true.AsTask();
        }
        #endregion

        public override ReactElement Render()
        {
            return
                new DefaultChrome();
        }


        public class State : PageStateBase { }
        public class Props : PagePropsBase
        {
            public string[] HomePagePath { get; set; } = (Config.Get("HomePagePath")?.ToString() ?? "home").AsArray();
            public bool IsHomePageSecured { get; set; } = Config.Get("IsHomePageSecured")?.ToString()?.ParseToBoolOrFallbackTo(false) ?? false;
        }
    }
}
