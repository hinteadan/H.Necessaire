using Bridge;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using System;

namespace H.Necessaire.ReactAppSample.Components
{
    internal class DataViewComponent : DataViewComponentBase<string, DataViewComponent.Props, DataViewComponent.State>
    {
        public DataViewComponent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }


        public class State : DataViewComponentState<string> { }
        public class Props : DataViewComponentProps<string>
        {
            
        }
    }
}
