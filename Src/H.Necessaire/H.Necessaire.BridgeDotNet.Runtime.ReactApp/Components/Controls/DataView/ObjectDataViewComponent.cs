using Bridge;
using Bridge.React;
using Retyped.Primitive;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ObjectDataViewComponent<TData> : DataViewComponentBase<TData, DataViewComponentProps<TData>, ObjectDataViewState<TData>>
    {
        public ObjectDataViewComponent(DataViewComponentProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public override ReactElement New(TData data, DataViewConfig config)
        => new ObjectDataViewComponent<TData>(new DataViewComponentProps<TData> { Data = data, DataViewConfig = config });
    }

    public class ObjectDataViewState<TData> : DataViewComponentState<TData>
    {
        public string[] PropertyNamesToIgnore { get; set; }
    }
}
