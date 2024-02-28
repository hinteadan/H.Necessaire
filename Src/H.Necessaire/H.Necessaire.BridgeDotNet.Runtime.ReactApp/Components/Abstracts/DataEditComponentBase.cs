using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataEditComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataEditComponentState, new()
        where TProps : DataEditComponentProps
    {
        protected DataEditComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return null;
        }
    }

    public abstract class DataEditComponentState : ComponentStateBase { }
    public abstract class DataEditComponentProps : ComponentPropsBase { }
}
