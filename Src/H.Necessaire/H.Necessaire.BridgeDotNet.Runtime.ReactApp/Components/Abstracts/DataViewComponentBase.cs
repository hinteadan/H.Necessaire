using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataViewComponentState<TData>, new()
        where TProps : DataViewComponentProps<TData>
    {
        const int defaultMaxLength = 150;

        protected DataViewComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override async Task RunAtStartup()
        {
            await base.RunAtStartup();
            await OnPropsChange(props);
        }

        protected override async void ComponentWillReceiveProps(TProps nextProps)
        {
            base.ComponentWillReceiveProps(nextProps);
            await OnPropsChange(nextProps);
        }

        protected virtual async Task OnPropsChange(TProps props)
        {
            await DoAndSetStateAsync(state =>
            {
                state.Data = props == null ? GetDefaultDataValue() : props.Data;
                state.Label = props?.Label;
                state.Description = props?.Description;
            });
        }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{typeof(TData).Name}-View-Chrome",
                    }
                    ,
                    RenderLabelIfNecessary()
                    ,
                    (!HasValue() ? RenderNoDataView() : RenderDataView())
                    ,
                    RenderDescriptionIfNecessary()
                );
        }

        protected virtual ReactElement RenderDescriptionIfNecessary()
        {
            if (state.Description == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.Flex,
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = Branding.Colors.PrimaryIsh().Lighter().ToCssRGBA(),
                        },
                        ClassName = $"{typeof(TData).Name}-DescriptionChrome",
                    }
                    ,
                    state.Description
                );
        }

        protected virtual ReactElement RenderLabelIfNecessary()
        {
            if(state.Label == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.Flex,
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = Branding.Colors.Primary.Lighter().ToCssRGBA(),
                        },
                        ClassName = $"{typeof(TData).Name}-LabelChrome",
                    }
                    ,
                    state.Label
                );
        }

        protected virtual ReactElement RenderDataView()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            JustifyContent = JustifyContent.Center,
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{typeof(TData).Name}-Viewer",
                    }
                    ,
                    RenderDataValue()
                );
        }

        protected virtual ReactElement RenderNoDataView()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            JustifyContent = JustifyContent.Center,
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{typeof(TData).Name}-Viewer-NoData",
                    }
                    ,
                    RenderNoDataValue()
                );
        }

        protected virtual ReactElement RenderDataValue()
        {
            return
                DOM.Span(
                    new Attributes
                    {
                        Title = RenderTooltipText(),
                    }
                    ,
                    RenderData()
                );
        }

        protected virtual ReactElement RenderNoDataValue()
        {
            return
                DOM.Span(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.Flex,
                            Color = Branding.WarningColor.ToCssRGBA(),
                            VerticalAlign = VerticalAlign.Middle,
                            AlignItems = AlignItems.Center,
                            FontSize = Branding.Typography.FontSizeSmall.EmsCss,
                        }
                    }
                    ,
                    DOM.Span("😔")
                    ,
                    DOM.Span(new Attributes { Style = new ReactStyle { MarginRight = Branding.SizingUnitInPixels / 4 } })
                    ,
                    DOM.Em("No Data")
                );
        }

        protected virtual TData GetDefaultDataValue() => default(TData);
        protected virtual bool HasValue() => state.Data != null;
        protected virtual Union<ReactElement, string> RenderData() => state.Data.ToString().EllipsizeIfNecessary(maxLength: props?.MaxLength ?? defaultMaxLength);
        protected virtual string RenderTooltipText() => state.Data.ToString();
    }

    public abstract class DataViewComponentState<TData> : ComponentStateBase
    {
        public TData Data { get; set; }
        public Union<ReactElement, string> Label { get; set; }
        public Union<ReactElement, string> Description { get; set; }
    }

    public abstract class DataViewComponentProps<TData> : ComponentPropsBase
    {
        public int? MaxLength { get; set; }
        public TData Data { get; set; }
        public Union<ReactElement, string> Label { get; set; }
        public Union<ReactElement, string> Description { get; set; }
    }
}
