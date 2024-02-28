using Bridge;
using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataViewComponentState<TData>, new()
        where TProps : DataViewComponentProps<TData>
    {
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
                    !HasValue() ? RenderNoDataView() : RenderDataView()
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
                    state.Data.ToString()
                );
        }

        protected virtual ReactElement RenderNoDataValue()
        {
            return
                DOM.Em(
                    "😔 No Data..."
                );
        }

        protected virtual TData GetDefaultDataValue() => default(TData);
        protected virtual bool HasValue() => state.Data != null;
    }

    public abstract class DataViewComponentState<TData> : ComponentStateBase
    {
        public TData Data { get; set; }
    }

    public abstract class DataViewComponentProps<TData> : ComponentPropsBase
    {
        public TData Data { get; set; }
    }
}
