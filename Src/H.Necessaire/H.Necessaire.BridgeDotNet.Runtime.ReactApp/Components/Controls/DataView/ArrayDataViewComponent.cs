using Bridge;
using Bridge.Html5;
using Bridge.React;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ArrayDataViewComponent<TData> : DataViewComponentBase<TData[], DataViewComponentProps<TData[]>, ArrayDataViewState<TData>>
    {
        public ArrayDataViewComponent(DataViewComponentProps<TData[]> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<TData[]> props) => new ArrayDataViewComponent<TData>(props);
        protected override DataViewComponentProps<TData[]> NewProps(TData[] data, DataViewConfig config)
            => base.NewProps(data, config).And(x => { 
                x.DataType = typeof(TData);
                if (state.DataType == typeof(object))
                    state.DataType = state.Data?.FirstOrDefault()?.GetType();
            });

        protected override async Task OnPropsChange(DataViewComponentProps<TData[]> props)
        {
            await base.OnPropsChange(props);
            await DoAndSetStateAsync(state =>
            {
                state.Data = state.Data?.ToNoNullsArray();
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
                            MarginLeft = (state.DataViewConfig?.Object?.CurrentDepth ?? 0) == 0 ? 0 : (state.DataViewConfig?.SpacingSize ?? Branding.SizingUnitInPixels),
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{GetDataTypeName()}-ArrayView-Chrome",
                    }
                    ,
                    RenderLabelIfNecessary()
                    ,
                    RenderDescriptionIfNecessary()
                    ,
                    (!HasValue() ? RenderNoDataView() : RenderDataView())
                );
        }

        protected override ReactElement RenderDataValue()
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
                        ClassName = $"{GetDataTypeName()}-Elements",
                    }
                    ,
                    state.Data.Select(RenderArrayElement)
                );
        }

        protected virtual ReactElement RenderArrayElement(TData data, int index)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            MarginTop = (index == 0 && state.DataViewConfig?.Label == null && state.DataViewConfig?.Description == null) ? 0 : state.DataViewConfig?.SpacingSize ?? Branding.SizingUnitInPixels,
                            JustifyContent = JustifyContent.Center,
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{GetDataTypeName()}-Element-{index}",
                    }
                    ,
                    DataViewComponentFactory.BuildViewerFor<TData>(
                        data,
                        cfg =>
                        {
                            cfg.CopyFrom(state.DataViewConfig);

                            cfg.Label = GetElementLabel(data, index);
                            cfg.Description = null;

                            cfg.Object = (cfg.Object ?? new ObjectDataViewConfig()).And(x => {
                                x.CurrentDepth += 1;
                                x.Path = x.Path.Push($"{index + 1}");
                            });
                        }
                    )
                );
        }

        private Union<ReactElement, string> GetElementLabel(TData data, int index)
        {
            if (state.DataViewConfig?.Array?.LabelPrinter != null)
                return state.DataViewConfig.Array.LabelPrinter.Invoke(index);

            return $"{index + 1}";
        }

        protected override bool HasValue() => base.HasValue() && state.Data.Any();
    }

    public class ArrayDataViewState<TData> : DataViewComponentState<TData[]>
    {
    }
}
