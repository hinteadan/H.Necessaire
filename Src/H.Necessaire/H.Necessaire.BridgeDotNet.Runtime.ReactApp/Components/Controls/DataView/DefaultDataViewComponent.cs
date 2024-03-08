using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DefaultDataViewComponent<TData> : DataViewComponentBase<TData, DefaultDataViewComponentProps<TData>, DefaultDataViewComponentState<TData>>
    {
        public DefaultDataViewComponent(DefaultDataViewComponentProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }

    public class DefaultDataViewComponentState<TData> : DataViewComponentState<TData>
    {

    }

    public class DefaultDataViewComponentProps<TData> : DataViewComponentProps<TData>
    {

    }
}
