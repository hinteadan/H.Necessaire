using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Alert : ComponentBase<UserFeedbackProps, Alert.State>
    {
        public Alert(UserFeedbackProps props) : base(props, null) { }

        public override ReactElement Render()
        {
            return
                new Elevation(new Elevation.Props { Depth = ElevationDepthLevel.Hihghest, StyleDecorator = x => x.FlexNode(isVerticalFlow: true).And(s => s.Padding = Branding.SizingUnitInPixels) },

                    new FormLayout
                    (
                        new FormLayout.Props { LayoutMode = FormLayoutMode.OnePerRowFill },

                        DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { }.FlexNode(isVerticalFlow: true),
                            },

                            new CenteredContent(new Title(new Title.Props { StyleDecorator = s => s.FlexNode() }, props.UserOptionsContext.Title))

                        ),

                        string.IsNullOrWhiteSpace(props.UserOptionsContext.DescriptionHtml) ? null : DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { MarginTop = Branding.SizingUnitInPixels }.FlexNode(isVerticalFlow: true),
                            },

                            new CenteredContent(
                                DOM.Span(new Attributes
                                {
                                    DangerouslySetInnerHTML = new RawHtml { Html = props.UserOptionsContext.DescriptionHtml },
                                })
                            )

                        ),

                        DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { MarginTop = Branding.SizingUnitInPixels }
                                .FlexNode(isVerticalFlow: false)
                                .And(s => s.JustifyContent = Bridge.Html5.JustifyContent.Center)
                                .And(s => s.MarginTop = Branding.SizingUnitInPixels)
                                ,
                            },

                            new CenteredContent(
                                new Button(
                                    new Button.Props
                                    {
                                        MinWidth = Branding.SizingUnitInPixels * 7,
                                        OnClick = () => props.UserOptionsContext.ConfirmSelection(),
                                    },
                                    "OK"
                                )
                            )

                        )

                    )
                );
        }

        public class State : ComponentStateBase { }
    }
}
