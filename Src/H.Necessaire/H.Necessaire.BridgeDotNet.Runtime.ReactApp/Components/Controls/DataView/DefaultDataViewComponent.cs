using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DefaultDataViewComponent<TData> : DataViewComponentBase<TData, DataViewComponentProps<TData>, DefaultDataViewComponentState<TData>>
    {
        public DefaultDataViewComponent(DataViewComponentProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }

    public class DefaultDataViewComponentState<TData> : DataViewComponentState<TData>
    {

    }
}
