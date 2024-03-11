using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>, ImADataViewComponent<TData>
        where TState : DataViewComponentState<TData>, new()
        where TProps : DataViewComponentProps<TData>, new()
    {
        #region Factory Stuff
        protected abstract ReactElement New(TProps props);
        public virtual ReactElement New(TData data, DataViewConfig config) => New(NewProps(data, config));
        protected virtual TProps NewProps(TData data, DataViewConfig config) => new TProps { Data = data, DataViewConfig = config };
        #endregion

        protected const int defaultMaxLength = 150;

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
                state.DataType = props == null ? typeof(TData) : props.DataType;
                if (state.DataType == typeof(object))
                    state.DataType = state.Data?.GetType();
                state.DataViewConfig = props?.DataViewConfig == null ? new DataViewConfig() : props.DataViewConfig;
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
                        ClassName = $"{GetDataTypeName()}-View-Chrome",
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
            if (state.DataViewConfig?.Description == null)
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
                        ClassName = $"{GetDataTypeName()}-DescriptionChrome",
                    }
                    ,
                    state.DataViewConfig.Description
                );
        }

        protected virtual ReactElement RenderLabelIfNecessary()
        {
            if(state.DataViewConfig?.Label == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.Flex,
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = Branding.Colors.Primary.Lighter(2).ToCssRGBA(),
                        },
                        ClassName = $"{GetDataTypeName()}-LabelChrome",
                    }
                    ,
                    state.DataViewConfig.Label
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
                        ClassName = $"{GetDataTypeName()}-Viewer",
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
                        ClassName = $"{GetDataTypeName()}-Viewer-NoData",
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
        protected virtual Union<ReactElement, string> RenderData() => state.Data.ToString().EllipsizeIfNecessary(maxLength: state.DataViewConfig?.MaxValueDisplayLength ?? defaultMaxLength);
        protected virtual string RenderTooltipText() => state.Data.ToString();
        protected virtual Type GetDataType() => state.DataType;
        protected virtual string GetDataTypeName() => GetDataType()?.Name ?? "Object";
    }

    public abstract class DataViewComponentState<TData> : ComponentStateBase
    {
        public TData Data { get; set; }
        public Type DataType { get; set; } = typeof(TData);
        public DataViewConfig DataViewConfig { get; set; } = new DataViewConfig();
    }

    public class DataViewComponentProps<TData> : ComponentPropsBase
    {
        public TData Data { get; set; }
        public Type DataType { get; set; } = typeof(TData);
        public DataViewConfig DataViewConfig { get; set; } = new DataViewConfig();
    }
}
