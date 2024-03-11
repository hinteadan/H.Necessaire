using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class StringDataViewComponent : DataViewComponentBase<string, DataViewComponentProps<string>, StringDataViewComponent.State>
    {
        public StringDataViewComponent(DataViewComponentProps<string> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<string> props) => new StringDataViewComponent(props);

        public class State : DataViewComponentState<string> { }
    }
}
