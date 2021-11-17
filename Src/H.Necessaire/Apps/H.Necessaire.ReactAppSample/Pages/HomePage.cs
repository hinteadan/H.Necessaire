using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using System;

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

                        , new Button(new Button.Props
                        {

                            OnClick = async () =>
                            {
                                Console.WriteLine((await ConfirmWithTitle("Debug Confirm", "You sure?")).ObjectToJson());
                                Console.WriteLine((await Confirm("You sure?")).ObjectToJson());
                                await AlertWithTitle("Debug Alert", "Alert !!!!");
                                await Alert("Debug Alert");
                            },

                        }, "Debug")

                    )

                );
        }


        public class State : PageStateBase { }

        public class Props : PagePropsBase { }
    }
}
