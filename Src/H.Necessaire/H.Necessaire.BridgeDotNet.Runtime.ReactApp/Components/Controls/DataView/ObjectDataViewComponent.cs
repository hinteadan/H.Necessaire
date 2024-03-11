using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ObjectDataViewComponent<TData> : DataViewComponentBase<TData, DataViewComponentProps<TData>, ObjectDataViewState<TData>>
    {
        public ObjectDataViewComponent(DataViewComponentProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<TData> props) => new ObjectDataViewComponent<TData>(props);
    }

    public class ObjectDataViewState<TData> : DataViewComponentState<TData>
    {
        public string[] PropertyNamesToIgnore { get; set; }
    }
}
