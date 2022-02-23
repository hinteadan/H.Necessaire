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

                        , new Button(new Button.Props
                        {

                            OnClick = async () =>
                            {
                                Navi.ChangeDisplayedHash("test", "hash");

                                //await Logger.LogDebug("Confirm with Title", (await ConfirmWithTitle("Debug Confirm", "You sure?")).ObjectToJson());
                                //await Logger.LogDebug("Confirm", (await Confirm("You sure?")).ObjectToJson());
                                //await AlertWithTitle("Debug Alert", "Alert !!!!");
                                //await Alert("Debug Alert");
                                //await
                                //    Logger.LogTrace("Test Log TRACE")
                                //    .ContinueWith(x => Logger.LogDebug("Test Log DEBUG"))
                                //    .ContinueWith(x => Logger.LogInfo("Test Log INFO"))
                                //    .ContinueWith(x => Logger.LogWarn("Test Log WARN"))
                                //    .ContinueWith(x => Logger.LogError("Test Log ERROR"))
                                //    .ContinueWith(x => Logger.LogCritical("Test Log CRITICAL"))
                                //    .ContinueWith(x => Logger.LogDebug("Test Log DEBUG with payload", new { A = "B" }))
                                //    ;
                            },

                        }, "Debug")
                        ,
                        new FontIcon(new FontIcon.Props { Provider = FontIcon.Provider.FontAwesome, IconName = "user" })

                    )

                );
        }


        public class State : PageStateBase { }

        public class Props : PagePropsBase { }
    }
}
