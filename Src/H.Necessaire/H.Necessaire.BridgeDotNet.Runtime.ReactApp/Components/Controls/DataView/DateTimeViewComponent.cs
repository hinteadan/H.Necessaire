using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DateTimeViewComponent : DataViewComponentBase<DateTime?, DataViewComponentProps<DateTime?>, DateTimeViewComponent.State>
    {
        const string defaultFormat = "ddd, MMM dd, yyyy 'at' HH:mm";

        public DateTimeViewComponent(DataViewComponentProps<DateTime?> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<DateTime?> props) => new DateTimeViewComponent(props);


        protected override Union<ReactElement, string> RenderData()
        {
            if (state.DataViewConfig?.DateTime?.IsAsOfNow == true)
                return state.Data.Value.PrintDateTimeAsOfNow();

                DateTime dateTimeToDisplay = state.Data.Value.EnsureUtc();
            if ((state.DataViewConfig?.DateTime?.Kind).In(DateTimeKind.Unspecified, DateTimeKind.Local))
                dateTimeToDisplay = dateTimeToDisplay.ToLocalTime();

            if (state.DataViewConfig?.DateTime?.Format?.IsEmpty() == false)
                return dateTimeToDisplay.ToString(state.DataViewConfig.DateTime.Format);

            return dateTimeToDisplay.ToString(Config.Get("Formatting")?.Get("DateAndTime")?.ToString() ?? defaultFormat);
        }


        public class State : DataViewComponentState<DateTime?> { }
    }
}
