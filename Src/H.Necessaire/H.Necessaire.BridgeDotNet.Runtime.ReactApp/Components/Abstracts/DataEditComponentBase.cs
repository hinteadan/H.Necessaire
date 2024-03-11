using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataEditComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : DataEditComponentState<TData>, new()
        where TProps : DataEditComponentProps<TData>, new()
    {
        protected DataEditComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return null;
        }
    }

    public abstract class DataEditComponentState<TData> : ComponentStateBase 
    {
        public TData DataBeingEdited { get; set; }
    }

    public abstract class DataEditComponentProps<TData> : ComponentPropsBase
    {
        public TData DataToEdit { get; set; }
    }
}
