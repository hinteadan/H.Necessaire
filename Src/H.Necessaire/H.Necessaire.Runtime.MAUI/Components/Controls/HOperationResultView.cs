using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Elements;
using H.Necessaire.Runtime.MAUI.Components.Layouts;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HOperationResultView : HMauiComponentBase
    {
        public TaggedValue<OperationResult> Data
        {
            get => ViewData as TaggedValue<OperationResult>;
            set => ViewData = value;
        }
        string OperationName => Data?.Name;
        OperationResult Result => Data?.Value;
        public string SuccessMessage { get; set; }
        public string SuccessWithWarningMessage { get; set; }
        public string FailureMessage { get; set; }

        protected override View ConstructContent()
        {
            double cornerRadius = Branding.SizingUnitInPixels / 4;

            return new Grid().And(lay =>
            {
                lay.Add(
                    new Border
                    {
                        Stroke = GetColor(),
                        StrokeShape = new RoundRectangle { CornerRadius = cornerRadius },
                        StrokeThickness = 4,
                        Content = BuildDataView(),
                    }
                    .Bind(this, null, x => x.Stroke = GetColor())
                );
            });
        }

        View BuildDataView()
        {
            return new VerticalStackLayout().And(lay =>
            {

                lay.Add(BuildHeader());

                lay.Add(BuildDetailsIfNecessary());

                lay.Bind(this, x => x.AndIf(x.Children.Count > 1, x => x.RemoveAt(1)), x => x.Add(BuildDetailsIfNecessary()));

            });
        }

        View BuildHeader()
        {
            return new Grid
            {
                BackgroundColor = GetColor(),
                Padding = SizingUnit / 2,
            }
            .Bind(this, null, x => x.Background = GetColor())
            .And(lay =>
            {
                lay.Add(new HFontIconLabel
                {
                    HorizontalOptions = LayoutOptions.Start,
                    Glyph = GetIconGlyph(),
                    Color = Branding.Colors.Primary.Darker(3).ToMaui(),
                    Text = GetMainMessage(),
                }
                .Bind(this,
                    null,
                    x => { x.Glyph = GetIconGlyph(); x.Text = GetMainMessage(); }
                ));
            });
        }

        View BuildDetailsIfNecessary()
        {
            if (Result is null)
                return null;

            string[] additionalReasons = null;
            if (Result.HasReasonsToDisplay() && Result.ReasonsToDisplay.Length > 1)
                additionalReasons = Result.ReasonsToDisplay.Jump(1).ToNonEmptyArray();

            bool hasAnyAdditionalDetails = !additionalReasons.IsEmpty() || !Result.Warnings.IsEmpty();
            if (!hasAnyAdditionalDetails)
                return null;

            return new VerticalStackLayout
            {
                
            }
            .AndIf(!additionalReasons.IsEmpty(), lay =>
            {
                lay.Add(BuildAdditionalReasons(additionalReasons));
            })
            .AndIf(!Result.Warnings.IsEmpty(), lay =>
            {
                lay.Add(BuildWarnings(Result.Warnings));
            });
        }

        View BuildAdditionalReasons(string[] additionalReasons)
        {
            return new HVerticalStack
            {
                Padding = SizingUnit,
                BackgroundColor = GetColor(),
                Views = additionalReasons.Select(BuildAdditionalReason).ToArray(),
            };
        }



        View BuildWarnings(string[] warnings)
        {
            return new HVerticalStack
            {
                Padding = SizingUnit,
                BackgroundColor = Branding.WarningColor.ToMaui(),
                Views = [new HFontIconLabel
                {
                    Padding = new Thickness(0, SizingUnit / 2),
                    FontSize = Branding.Typography.FontSizeSmall,
                    HorizontalOptions = LayoutOptions.Start,
                    Glyph = "ic_fluent_warning_16_filled",
                    Color = Branding.Colors.Primary.Darker(3).ToMaui(),
                    Text = warnings.Length == 1 ? "Warning" : "Warnings",
                },.. warnings.Select(BuildWarning)],
            };
        }

        View BuildAdditionalReason(string reason, int index) => BuildAdditionalEntry(reason, index);
        View BuildWarning(string warning, int index) => BuildAdditionalEntry(warning, index);

        View BuildAdditionalEntry(string entry, int index)
        {
            return new HLabel
            {
                Padding = new Thickness(0, SizingUnit / 2),
                FontSize = Branding.Typography.FontSizeSmall,
                TextColor = Branding.TextColor.ToMaui(),
                LineBreakMode = LineBreakMode.WordWrap,
                Text = entry,
            };
        }

        Color GetColor()
        {
            if (Result is null)
                return Branding.MuteColor.ToMaui();

            if (Result.IsSuccessful)
                return Branding.SuccessColor.ToMaui();

            return Branding.DangerColor.ToMaui();
        }

        string GetIconGlyph()
        {
            if (Result is null)
                return "ic_fluent_border_none_16_filled";

            if (Result.IsSuccessful)
                return Result.HasWarnings() ? "ic_fluent_checkmark_circle_warning_16_filled" : "ic_fluent_checkmark_circle_16_filled";

            return "ic_fluent_error_circle_16_filled";
        }

        string GetMainMessage()
        {
            if (Result is null)
                return "No result";

            return Result.IsSuccessful ? GetSuccessfulMessage() : GetFailureMessage();
        }

        string GetSuccessfulMessage()
        {
            if (Result is null)
                return null;

            if (!Result.IsSuccessful)
                return null;

            if (Result.HasWarnings() && !SuccessWithWarningMessage.IsEmpty())
                return SuccessWithWarningMessage;

            if (!Result.HasWarnings() && !SuccessMessage.IsEmpty())
                return SuccessMessage;

            if (Result.HasReasonsToDisplay())
                return Result.ReasonsToDisplay.First();

            if (OperationName.IsEmpty())
                return !Result.HasWarnings() ? "Successfully completed" : "Successfully completed with some warnings";

            return !Result.HasWarnings() ? $"Successfully completed {OperationName}" : $"Successfully completed {OperationName} with some warnings";
        }

        string GetFailureMessage()
        {
            if (Data?.Value is null)
                return null;

            if (Data.Value.IsSuccessful)
                return null;

            if (!FailureMessage.IsEmpty())
                return FailureMessage;

            if (Data.Value.HasReasonsToDisplay())
                return Data.Value.ReasonsToDisplay.First();

            if (Data.Name.IsEmpty())
                return "Failed to complete";

            return $"Failed to complete {Data.Name}";
        }
    }
}
