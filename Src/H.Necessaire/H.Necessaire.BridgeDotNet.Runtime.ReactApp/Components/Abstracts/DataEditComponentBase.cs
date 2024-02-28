using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataEditComponentBase<TData, TProps, TState>
        : ComponentBase<TProps, TState>
        where TState : ImAUiComponentState, new()
    {
        protected DataEditComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return null;
        }
    }
}
