using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.HUI.Components;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Components.Builders.Abstracts
{
    public abstract class HMauiHUIPropertyComponentBuilderBase : ImAHMauiHUIPropertyComponentBuilder
    {
        public virtual View BuildComponentForProperty(HViewModelProperty viewModelProperty)
        {
            switch (viewModelProperty.PresentationInfo.Type)
            {
                case HUIPresentationType.Boolean:
                    return BuildBooleanPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Selection:
                    return BuildSelectionPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Text:
                    return BuildTextPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Note:
                    return BuildNotePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Number:
                    return BuildNumberPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.NumberInterval:
                    return BuildNumberIntervalPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.TimeSpan:
                    return BuildTimeSpanPropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Date:
                    return BuildDatePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.Time:
                    return BuildTimePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.DateAndTime:
                    return BuildDateTimePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.PeriodOfTime:
                    return BuildPeriodOfTimePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.PartialDateTime:
                    return BuildPartialDateTimePropertyComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));

                case HUIPresentationType.PartialPeriodOfTime:
                case HUIPresentationType.ApproximatePeriodOfTime:
                case HUIPresentationType.Collection:
                case HUIPresentationType.SubViewModel:
                case HUIPresentationType.Custom:
                default:
                    return new HUINotYetSupportedComponent(viewModelProperty).And(x => viewModelProperty.ReferenceNativeUIControl(x));
            }
        }

        static View BuildPartialDateTimePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HPartialDateTimeEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildPeriodOfTimePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HPeriodOfTimeEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildDateTimePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HDateTimePicker().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildTimePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HTimePicker().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildDatePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HDatePicker().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildTimeSpanPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HTimeSpanEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildNumberIntervalPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HNumberIntervalEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildNumberPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HNumberEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildNotePropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HNoteEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
            });
        }

        static View BuildTextPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HTextEditor().And(editor =>
            {
                editor.Label = viewModelProperty.Label;
                editor.Text = viewModelProperty.ValueAs<string>();
            });
        }

        static View BuildSelectionPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HPicker().And(picker =>
            {
                picker.Label = viewModelProperty.Label;
                picker.SetDataSource(viewModelProperty.PresentationInfo.Selection.Options);
            });
        }

        static View BuildBooleanPropertyComponent(HViewModelProperty viewModelProperty)
        {
            return new HSwitch().And(@switch =>
            {
                @switch.Label = viewModelProperty.Label;
                @switch.IsOn = viewModelProperty.ValueAs<bool>();
            });
        }
    }
}
