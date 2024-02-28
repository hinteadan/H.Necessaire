using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataViewComponentState<TData>, new()
        where TProps : DataViewComponentProps<TData>
    {
        protected DataViewComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return null;
        }
    }

    public abstract class DataViewComponentState<TData> : ComponentStateBase { }
    public abstract class DataViewComponentProps<TData> : ComponentPropsBase { }
}
