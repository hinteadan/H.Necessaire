using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class StringDataViewComponent : DataViewComponentBase<string, DataViewComponentProps<string>, StringDataViewComponent.State>
    {
        public StringDataViewComponent(DataViewComponentProps<string> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public override ReactElement New(string data, DataViewConfig config)
            => new StringDataViewComponent(new DataViewComponentProps<string> { Data = data, DataViewConfig = config });

        public class State : DataViewComponentState<string> { }
    }
}
