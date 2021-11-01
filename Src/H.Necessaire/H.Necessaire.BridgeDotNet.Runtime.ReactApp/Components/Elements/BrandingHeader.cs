using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class BrandingHeader : ComponentBase<BrandingHeader.Props, BrandingHeader.State>
    {
        public BrandingHeader() : base(new Props(), null) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Height = Branding.SizingUnitInPixels * 5,
                            Overflow = Bridge.Html5.Overflow.Hidden,
                            Display = Bridge.Html5.Display.Flex,
                            BackgroundColor = Branding.PrimaryColorTranslucent.ToCssRGBA(),
                            BoxShadow = "0 3.2px 7.2px 0 rgba(0,0,0,.132),0 .6px 1.8px 0 rgba(0,0,0,.108)",
                        },
                    },


                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                Height = Branding.SizingUnitInPixels * 5,
                                PaddingLeft = Branding.SizingUnitInPixels,
                                Cursor = Bridge.Html5.Cursor.Pointer,
                                JustifyContent = Bridge.Html5.JustifyContent.Center,
                            }.FlexNode(isVerticalFlow: true),
                            OnClick = x => FlySafe(props?.OnClick),
                        },
                        Children
                    ),

                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                Height = Branding.SizingUnitInPixels * 5,
                                PaddingRight = Branding.SizingUnitInPixels,
                                Cursor = Bridge.Html5.Cursor.Default,
                                JustifyContent = Bridge.Html5.JustifyContent.Center,
                            }.FlexNode(isVerticalFlow: true),
                        },

                        new CurrentUserCard()
                    )
                ); ;
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public Action OnClick { get; set; } = null;
        }
    }
}
