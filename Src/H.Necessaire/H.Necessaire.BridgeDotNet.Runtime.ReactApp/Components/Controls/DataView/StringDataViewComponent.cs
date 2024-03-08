using Bridge.React;
using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class StringDataViewComponent : DataViewComponentBase<string, StringDataViewComponent.Props, StringDataViewComponent.State>
    {
        public StringDataViewComponent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public class State : DataViewComponentState<string> { }
        public class Props : DataViewComponentProps<string> { }
    }
}
