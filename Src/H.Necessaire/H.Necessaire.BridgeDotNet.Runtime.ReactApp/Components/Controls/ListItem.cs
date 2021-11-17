using Bridge;
using Bridge.React;
using System;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ListItem : ComponentBase<ListItem.Props, ListItem.State>
    {
        public ListItem(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public ListItem(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                MinWidth = 200,
                Width = props.Width,
                MinHeight = 80,
                MarginTop = Branding.SizingUnitInPixels,
                BorderBottom = "solid 1px",
                BorderBottomColor = Branding.Colors.Complementary.Lighter(2).ToCssRGBA(),
            }.FlexNode();

            if (props.OnClick != null)
                reactStyle.Cursor = Bridge.Html5.Cursor.Pointer;

            ReactElement icon = RenderIcon();

            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = reactStyle,
                        OnClick = x => props.OnClick?.Invoke(props.Payload),
                    },
                    icon == null ? null : DOM.Div
                    (
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                BackgroundColor = Branding.Colors.Primary.Color.ToCssRGBA(),
                            }.FlexNode(isVerticalFlow: false, size: Branding.SizingUnitInPixels * 3)
                        },
                        new CenteredContent
                        (
                            RenderIcon()
                        )
                    ),
                    DOM.Div
                    (
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                PaddingLeft = Branding.SizingUnitInPixels / 2,
                            }.FlexNode(isVerticalFlow: true)
                        },
                        RenderContent()
                    ),

                    props.OnMoreClick == null ? null :
                    DOM.Div
                    (
                        new Attributes
                        {
                            OnClick = x => { x.StopPropagation(); props.OnMoreClick.Invoke(props.Payload); },
                            Style = new ReactStyle
                            {
                                BackgroundColor = Branding.Colors.Complementary.Color.ToCssRGBA(),
                                Cursor = Bridge.Html5.Cursor.Pointer,
                            }.FlexNode(isVerticalFlow: false, size: Branding.SizingUnitInPixels * 3)
                        },
                        new CenteredContent
                        (
                            RenderMoreOperation()
                        )
                    )
                );
        }

        private ReactElement RenderMoreOperation()
        {
            return
                DOM.Div(new Attributes
                {
                    Style = new ReactStyle
                    {
                        FontSize = Branding.Typography.FontSizeLarger.PointsCss,
                        Color = Branding.BackgroundColor.ToCssRGBA(),
                    },
                }, "...");
        }

        private ReactElement RenderContent()
        {
            bool hasChildren = Children?.Any() ?? false;
            bool hasTitle = !string.IsNullOrWhiteSpace(props.Title);

            if (!hasTitle && !hasChildren)
                return RenderEmptyContent();

            if (hasTitle && !hasChildren)
                return RenderOnlyTitleContent();

            if (!hasTitle && hasChildren)
                return RenderOnlyChildrenContent();

            return RenderTitleAndChildren();
        }

        private ReactElement RenderTitleAndChildren()
        {
            return
                DOM.Div
                (
                    new Attributes { Style = new ReactStyle { }.FlexNode(isVerticalFlow: true) },
                    DOM.Div
                    (
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                FontSize = Branding.Typography.FontSizeLarge.PointsCss,
                                JustifyContent = Bridge.Html5.JustifyContent.Center,
                            }.FlexNode(isVerticalFlow: true, 30)
                        },
                        props.Title
                    ),
                    DOM.Div
                    (
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                FontSize = Branding.Typography.FontSizeSmall.PointsCss,
                            }.FlexNode(isVerticalFlow: true)
                        },
                        new ScrollableContent(Children)
                    )
                );
        }

        private ReactElement RenderOnlyTitleContent()
        {
            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            JustifyContent = Bridge.Html5.JustifyContent.Center,
                            FontSize = Branding.Typography.FontSizeLarge.PointsCss,
                        }.FlexNode(isVerticalFlow: true)
                    },
                    props.Title
                );
        }

        private ReactElement RenderOnlyChildrenContent()
        {
            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmall.PointsCss,
                        }.FlexNode(isVerticalFlow: true)
                    },
                    new ScrollableContent(Children)
                );
        }

        private ReactElement RenderEmptyContent()
        {
            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmall.PointsCss,
                            Opacity = "0.2",
                        }.FlexNode()
                    },
                    new CenteredContent(DOM.Em("Nothing to show here, so sorry...  :("))
                );
        }

        private ReactElement RenderIcon()
        {
            if (string.IsNullOrWhiteSpace(props.Title) && string.IsNullOrWhiteSpace(props.IconUrl))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(props.IconUrl))
            {
                string iconText
                    = props.Title.Length >= 2
                    ? props.Title.Substring(0, 1).ToUpper() + props.Title.Substring(1, 1).ToLower()
                    : props.Title.ToUpper()
                    ;

                return DOM.Div(new Attributes
                {
                    Style = new ReactStyle
                    {
                        FontSize = Branding.Typography.FontSizeLarger.PointsCss,
                        Color = Branding.BackgroundColor.ToCssRGBA(),
                    }
                }, iconText);
            }

            return new Icon(new Icon.Props { Url = props.IconUrl });
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public object Payload { get; set; }
            public Union<string, int> Width { get; set; } = 400;
            public string IconUrl { get; set; } = null;
            public string Title { get; set; } = null;
            public Action<object> OnClick { get; set; } = null;
            public Action<object> OnMoreClick { get; set; } = null;
        }
    }
}
