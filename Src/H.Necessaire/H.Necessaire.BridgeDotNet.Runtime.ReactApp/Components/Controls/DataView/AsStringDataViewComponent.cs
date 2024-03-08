using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AsStringDataViewComponent : DataViewComponentBase<string, AsStringDataViewComponent.Props, AsStringDataViewComponent.State>
    {
        public AsStringDataViewComponent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public class State : DataViewComponentState<string>
        { 

        }

        public class Props : DataViewComponentProps<string>
        {

        }
    }
}
