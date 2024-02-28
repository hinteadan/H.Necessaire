using Bridge;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;

namespace H.Necessaire.ReactAppSample.Components
{
    internal class DataEditComponent : DataEditComponentBase<string, DataEditComponent.Props, DataEditComponent.State>
    {
        public DataEditComponent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase { }
    }
}
