using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CurrentUserCard : ComponentBase<CurrentUserCard.Props, CurrentUserCard.State>
    {
        #region Construct
        public CurrentUserCard(Props props) : base(props, null) { }
        public CurrentUserCard() : base(new Props(), null) { }
        #endregion

        public override ReactElement Render()
        {
            if (SecurityContext == null)
                return null;

            return
                DOM.Div(

                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            JustifyContent = Bridge.Html5.JustifyContent.FlexEnd,
                        }
                        .FlexNode(),
                    },

                    RenderUserAvatar(),

                    new Spacer(new Spacer.Props { IsVertical = true }),

                    RenderUserLabel()

                );
        }

        private ReactElement RenderUserAvatar()
        {
            return
                DOM.Div(

                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Width = Branding.SizingUnitInPixels * 4,
                            BackgroundImage = $"url('{BaseUrl}/android-chrome-192x192.png')",
                            BackgroundRepeat = Bridge.Html5.BackgroundRepeat.NoRepeat,
                            BackgroundPosition = "center center",
                            BackgroundSize = "contain",
                            Display = Bridge.Html5.Display.InlineBlock,
                        },
                    }

                );
        }

        private ReactElement RenderUserLabel()
        {
            return
                DOM.Div(

                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmall.EmsCss,
                            JustifyContent = Bridge.Html5.JustifyContent.Center,
                            Display = Bridge.Html5.Display.Flex,
                            FlexDirection = Bridge.Html5.FlexDirection.Column,
                        },
                    }
                    ,
                    SecurityContext.User?.ToString()
                    ?? "[No User Info]"

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase { }
    }
}
