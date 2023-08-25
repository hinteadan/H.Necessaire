using Bridge.React;
using H.Necessaire.UI;
using System;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ActionSheet : ComponentBase<ActionSheet.Props, ActionSheet.State>
    {
        public ActionSheet(Props props) : base(props, null) { }

        public override ReactElement Render()
        {
            return
                new FormLayout
                (
                    new FormLayout.Props { LayoutMode = FormLayoutMode.OnePerRow },

                    new Elevation(new Elevation.Props { ClassName = "animate morph-in", Depth = ElevationDepthLevel.Hihghest, StyleDecorator = x => x.And(s => s.MaxHeight = "70vh").FlexNode(isVerticalFlow: true) },

                        DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { MarginBottom = Branding.SizingUnitInPixels }.FlexNode(isVerticalFlow: true),
                            },

                            props.Header != null ? props.Header : new CenteredContent(new Title(props.UserOptionsContext.Title))

                        ),

                        string.IsNullOrWhiteSpace(props.UserOptionsContext.DescriptionHtml) ? null : DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { }.FlexNode(isVerticalFlow: true),
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
                                Style = new ReactStyle { }.FlexNode(isVerticalFlow: true).ScrollContent(),
                            },


                            props.UserOptionsContext.Options.Select(RenderOption).ToArray()

                        ),

                        DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle { MarginTop = Branding.SizingUnitInPixels }.FlexNode(isVerticalFlow: true),
                            },

                            new CenteredContent(
                                new Button(
                                    new Button.Props
                                    {
                                        OnClick = () => props.UserOptionsContext.Cancel(),
                                    },
                                    "Cancel"
                                )
                            )

                        )

                    )
                );
        }

        private ReactElement RenderOption(UserOption option)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle { Height = 80 },
                    },
                    new ListItem(new ListItem.Props
                    {
                        Width = "100%",
                        IconUrl = option.Icon,
                        Payload = option,
                        OnClick = x =>
                        {
                            props.UserOptionsContext.Select(x.As<UserOption>());
                            option.OnClick?.Invoke(x.As<UserOption>().Payload);
                        },
                    }, new CenteredContent(
                        new Subtitle(option.Label)
                    ))
                );
        }

        public class State : ComponentStateBase { }
        public class Props : UserFeedbackProps
        {
            public ReactElement Header { get; set; } = null;
        }
    }
}
