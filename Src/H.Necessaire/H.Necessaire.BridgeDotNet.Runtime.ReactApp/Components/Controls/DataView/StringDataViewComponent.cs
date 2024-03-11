using Bridge.React;
using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class StringDataViewComponent : DataViewComponentBase<string, DataViewComponentProps<string>, StringDataViewComponent.State>
    {
        public StringDataViewComponent(DataViewComponentProps<string> props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public class State : DataViewComponentState<string> { }
    }
}
