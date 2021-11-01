using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;

namespace H.Necessaire.ReactAppSample.Pages
{
    public class HomePage : PageBase<HomePage.Props, HomePage.State>
    {
        public HomePage() : base(new Props(), null) { }

        public override ReactElement Render()
        {
            return
                new DefaultChrome(

                    new CenteredContent(

                        DOM.H1("Hello there !")

                    )

                );
        }


        public class State : PageStateBase { }

        public class Props : PagePropsBase { }
    }
}
