using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HSwitch : HMauiComponentBase
    {
        Switch @switch;
        HLabel label;
        protected override View ConstructContent()
        {
            return new Grid
            {
                ColumnDefinitions = [
                    new ColumnDefinition { Width = new GridLength(SizingUnit * 4, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                ],
            }.And(grid =>
            {

                grid.Add(
                    new Switch().And(x => @switch = x).And(@switch =>
                    {
                        @switch.OnColor = Branding.Colors.Primary.Lighter().ToMaui();
                        @switch.HorizontalOptions = LayoutOptions.Start;
                        @switch.VerticalOptions = LayoutOptions.Center;
                        @switch.Toggled += (sender, args) => {
                            bool isOn = args.Value;
                            bool isOff = !isOn;
                        };
                    })
                );

                grid.Add(
                    new HLabel().And(x => label = x).And(label =>
                    {
                        label.HorizontalOptions = LayoutOptions.Start;
                        label.VerticalOptions = LayoutOptions.Center;
                    }),
                    column: 1
                );

            });
        }

        public string Label
        {
            get => label.Text;
            set => label.Text = value;
        }

        public bool IsOn
        {
            get => @switch.IsToggled;
            set => @switch.IsToggled = value;
        }

        public bool IsOff
        {
            get => !@switch.IsToggled;
            set => @switch.IsToggled = !value;
        }
    }
}
