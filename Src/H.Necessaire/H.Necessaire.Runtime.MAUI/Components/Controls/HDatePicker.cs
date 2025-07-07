using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HDatePicker : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HDatePickerValueChangedEventArgs> OnValueChanged;

        private HNullableControl nullableControl;
        private DatePicker datePicker;

        protected override View ConstructLabeledContent()
        {
            return
                new DatePicker
                {
                    FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily,
                    FontSize = HUiToolkit.Current.Branding.Typography.FontSize,
                    TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui(),
                    Format = HUiToolkit.Current.Branding.DateFormat,
                    Date = DateTime.Today,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                }
                .RefTo(out datePicker)
                .And(x =>
                {
                    x.DateSelected += (s, e) => IfNotBinding(_ =>
                    {
                        IsNull = false;
                        Date = ((PartialDateTime)e.NewDate).And(x => { x.Hour = null; x.Minute = null; x.Second = null; x.Millisecond = null; });
                    });
                })
                .Nullable()
                .And(x =>
                {
                    x.NullToggled += (s, e) => IfNotBinding(_ =>
                    {
                        bool isNull = !e.Value;
                        if (isNull)
                        {
                            Date = null;
                        }
                    });
                })
                .RefTo(out nullableControl)
                .Bordered()
                ;
        }

        public bool IsNull
        {
            get => nullableControl.IsNull;
            set => nullableControl.IsNull = value;
        }

        public string NullText
        {
            get => nullableControl.NullText;
            set => nullableControl.NullText = value;
        }

        public DatePicker DatePicker => datePicker;

        PartialDateTime date = null;
        public PartialDateTime Date
        {
            get => date;
            set
            {
                bool hasChanged = value != date;
                if (!hasChanged)
                    return;

                date = value;
                RefreshControl();
                IfNotBinding(_ => RaiseOnValueChanged());
            }
        }

        void RefreshControl()
        {
            if (date is null || date.IsWheneverDate())
            {
                IsNull = true;
                return;
            }

            IsNull = false;
            datePicker.Date = date.ToMaximumDateTime().Value;
        }

        void RaiseOnValueChanged()
        {
            OnValueChanged?.Invoke(this, new HDatePickerValueChangedEventArgs(date));
        }
    }

    public class HDatePickerValueChangedEventArgs : EventArgs
    {
        public HDatePickerValueChangedEventArgs(PartialDateTime newValue)
        {
            NewValue = newValue;
        }

        public PartialDateTime NewValue { get; }
    }
}
