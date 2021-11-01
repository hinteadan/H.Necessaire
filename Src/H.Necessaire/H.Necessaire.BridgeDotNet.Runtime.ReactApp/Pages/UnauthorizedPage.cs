using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class UnauthorizedPage : PageBase<UnauthorizedPage.Props, UnauthorizedPage.State>
    {
        #region Construct
        public UnauthorizedPage() : base(new Props(), null) { }
        public UnauthorizedPage(Props props) : base(props, null) { }

        public override async Task Initialize()
        {
            await base.Initialize();

            state.PagePath = props.NavigationParams.GetValue<string>();
        }
        #endregion

        public override ReactElement Render()
        {
            return
                new DefaultChrome(

                    new PaddedContent(

                        new Spacer(),

                        DOM.Div(
                            "Oops... you are not authorized to view this page!"
                        )

                    )

                );
        }

        public class State : PageStateBase
        {
            public string PagePath { get; set; }
        }
        public class Props : PagePropsBase { }
    }
}
