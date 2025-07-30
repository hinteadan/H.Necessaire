using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HDateTimePicker : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HDateTimePickerValueChangedEventArgs> OnValueChanged;

        DateTime? dateTime;
        private HNullableControl nullabelControl;
        private PatchedTimePicker timePicker;
        private DatePicker datePicker;

        public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;

        public DateTime? DateAndTime
        {
            get => dateTime?.EnsureUtc();
            set
            {
                if (value?.EnsureUtc() == dateTime?.EnsureUtc())
                    return;

                dateTime = value?.EnsureUtc();

                RefreshUI();

                IfNotBinding(_ => RaiseValueChanged());
            }
        }

        public DateTime MinimumDate
        {
            get => datePicker.MinimumDate;
            set => datePicker.MinimumDate = value;
        }

        public DateTime MaximumDate
        {
            get => datePicker.MaximumDate;
            set => datePicker.MaximumDate = value;
        }

        public bool IsNullable
        {
            get => nullabelControl.IsNullable;
            set => nullabelControl.IsNullable = value;
        }

        protected override View ConstructLabeledContent()
        {
            return new Grid().And(layout =>
            {

                layout.Add(new HorizontalStackLayout { HorizontalOptions = LayoutOptions.End }.And(layout =>
                {

                    layout.Add(
                        new DatePicker
                        {
                            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily,
                            FontSize = HUiToolkit.Current.Branding.Typography.FontSize,
                            TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui(),
                            Format = HUiToolkit.Current.Branding.DateFormat,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                        }
                        .RefTo(out datePicker)
                        .Bind(this, null, x => x.AndIf(dateTime != null, x => x.Date = dateTime.Value.ToTimezone(TimeZoneInfo ?? TimeZoneInfo.Local)))
                        .And(x => x.DateSelected += (s, e) => IfNotBinding(_ =>
                        {
                            dateTime = BuildDateTimeFromUIControls();
                            RaiseValueChanged();
                        }))
                    );

                    layout.Add(
                        new PatchedTimePicker
                        {
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.Center,
                        }
                        .RefTo(out timePicker)
                        .Bind(this, null, x => x.AndIf(dateTime != null, x =>
                        {
                            DateTime local = dateTime.Value.ToTimezone(TimeZoneInfo ?? TimeZoneInfo.Local);
                            x.Time = local - local.Date;
                        }))
                        .And(x => x.TimeSelected += (s, e) => IfNotBinding(_ =>
                        {
                            dateTime = BuildDateTimeFromUIControls();
                            RaiseValueChanged();
                        }))
                    );

                }));

            })
            .Nullable("Not selected")
            .RefTo(out nullabelControl)
            .Bind(this, null, x => x.IsNull = dateTime is null)
            .And(x => x.NullToggled += (s, e) => IfNotBinding(_ =>
            {
                dateTime = BuildDateTimeFromUIControls();
                RaiseValueChanged();
            }))
            .Bordered()
            ;
        }

        DateTime? BuildDateTimeFromUIControls()
        {
            if (nullabelControl.IsNull)
                return null;

            DateTime result
                = new DateTime(datePicker.Date.Year, datePicker.Date.Month, datePicker.Date.Day, 0, 0, 0, DateTimeKind.Local)
                .Add(timePicker.Time)
                .EnsureUtc()
                ;

            return result;
        }

        protected override void RefreshUI(bool isViewDataIgnored = false)
        {
            base.RefreshUI(isViewDataIgnored: true);
        }

        void RaiseValueChanged()
        {
            OnValueChanged?.Invoke(this, new HDateTimePickerValueChangedEventArgs(dateTime));
        }
    }

    public class HDateTimePickerValueChangedEventArgs : EventArgs
    {
        public HDateTimePickerValueChangedEventArgs(DateTime? newValue)
        {
            NewValue = newValue?.EnsureUtc();
        }

        public DateTime? NewValue { get; }
    }
}
