using Bridge;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using System;

namespace H.Necessaire.ReactAppSample.Components
{
    public class CustomDataView : DataViewComponentBase<TimeSpan, DataViewComponentProps<TimeSpan>, CustomDataView.State>
    {
        public CustomDataView(DataViewComponentProps<TimeSpan> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<TimeSpan> props) => new CustomDataView(props);

        protected override Union<ReactElement, string> RenderData() => $"RenderData from CustomDataView for Timespan: {state.Data}";

        public class State : DataViewComponentState<TimeSpan>
        {

        }
    }
}
