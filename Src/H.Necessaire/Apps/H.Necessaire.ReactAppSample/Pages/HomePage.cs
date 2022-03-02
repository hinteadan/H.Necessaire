using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core;

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
                                ConsumerPlatformInfo platformDetails = (await UserAgentData.GetPlatformDetails()).Payload;

                                await Logger.LogDebug("UserAgentData.platformDetails", platformDetails.ToJson());
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
