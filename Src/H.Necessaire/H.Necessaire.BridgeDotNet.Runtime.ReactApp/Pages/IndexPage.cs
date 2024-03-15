using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class IndexPage : PageBase<IndexPage.Props, IndexPage.State>
    {
        #region Construct
        public IndexPage() : base(new Props(), null) { }
        public IndexPage(Props props) : base(props, null) { }

        public override async Task Initialize()
        {
            await base.Initialize();

            state.ReturnTo = props.NavigationParams?.GetValue<string>();
        }

        public override Task RunAtStartup()
        {
            if (props.IsHomePageSecured && SecurityContext == null)
            {
                FlySafe(() => Navi.GoToLogin(returnTo: state.ReturnTo.NullIfEmpty() ?? "/"));
            }
            else if (!state.ReturnTo.IsEmpty())
            {
                FlySafe(() => Navi.Go(state.ReturnTo));
            }
            else
            {
                FlySafe(() => Navi.Go(props.HomePagePath));
            }

            return true.AsTask();
        }
        #endregion

        public override ReactElement Render()
        {
            return
                new DefaultChrome();
        }


        public class State : PageStateBase 
        {
            public string ReturnTo { get; set; }
        }
        public class Props : PagePropsBase
        {
            public string[] HomePagePath { get; set; } = (Config.Get("HomePagePath")?.ToString() ?? "home").AsArray();
            public bool IsHomePageSecured { get; set; } = Config.Get("IsHomePageSecured")?.ToString()?.ParseToBoolOrFallbackTo(false) ?? false;
        }
    }
}
