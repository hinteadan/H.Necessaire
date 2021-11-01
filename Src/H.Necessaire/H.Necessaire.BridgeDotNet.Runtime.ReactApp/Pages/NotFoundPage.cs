using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class NotFoundPage : PageBase<NotFoundPage.Props, NotFoundPage.State>
    {
        #region Construct
        public NotFoundPage() : base(new Props(), null) { }
        #endregion

        public override ReactElement Render()
        {
            return
                new DefaultChrome(

                    new PaddedContent(

                        new Spacer(),

                        DOM.Div(
                            "Oops... requested page doesn't exist!"
                        )

                    )

                );
        }

        public class State : PageStateBase { }
        public class Props : PagePropsBase { }
    }
}
