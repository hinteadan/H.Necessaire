using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class BusyCurtain : ComponentBase<BusyCurtain.Props, BusyCurtain.State>
    {
        public BusyCurtain(Props props) : base(props, null) { }
        public BusyCurtain() : this(new Props { }) { }

        public override ReactElement Render()
        {
            return
                new Curtain(new Curtain.Props { StyleDecorator = x => { x.Cursor = Bridge.Html5.Cursor.Wait; return x; } },
                    new CenteredContent(

                        new FormLayout(
                            new FormLayout.Props { LayoutMode = FormLayoutMode.OnePerRowSmall, RowSpacing = Branding.SizingUnitInPixels * 2 },

                            DOM.Div(
                                new Attributes
                                {
                                    Style = new ReactStyle { JustifyContent = Bridge.Html5.JustifyContent.Center, }.FlexNode()
                                },

                                RenderAppIcon()
                            )
                            ,
                            DOM.Div(
                                new Attributes
                                {
                                    Style = new ReactStyle { JustifyContent = Bridge.Html5.JustifyContent.Center, }.FlexNode()
                                },

                                "Pondering, please wait..."
                            )

                        )



                    )
                );
        }

        private ReactElement RenderAppIcon()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Width = 192,
                            Height = 192,
                            BackgroundImage = $"url('{BaseUrl}/android-chrome-192x192.png')"
                        }
                    }
                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase { }
    }
}
