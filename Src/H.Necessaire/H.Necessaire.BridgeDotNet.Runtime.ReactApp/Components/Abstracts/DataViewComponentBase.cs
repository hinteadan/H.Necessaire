using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataViewComponentState, new()
        where TProps : DataViewComponentProps
    {
        protected DataViewComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return null;
        }
    }

    public abstract class DataViewComponentState : ComponentStateBase { }
    public abstract class DataViewComponentProps : ComponentPropsBase { }
}
