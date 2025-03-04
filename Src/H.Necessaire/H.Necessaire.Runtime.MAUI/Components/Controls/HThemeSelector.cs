using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HThemeSelector : HMauiComponentBase
    {
        static readonly IReadOnlyDictionary<AppTheme, string> options = new Dictionary<AppTheme, string> {
            { AppTheme.Unspecified, "System" },
            { AppTheme.Light, "Light" },
            { AppTheme.Dark, "Dark" },
        }.AsReadOnly();

        protected override View ConstructContent()
        {
            AppTheme currentTheme = Application.Current.UserAppTheme;

            return new Grid { }.And(layout =>
            {

                layout.Add(
                    new HPicker { FontSize = Branding.Typography.FontSizeSmaller }
                    .SetDataSource(options, x => x.Value)
                    .And(picker =>
                    {
                        picker.SelectedItem = options.Single(x => x.Key == currentTheme);
                        picker.SelectedIndexChanged += (s, e) =>
                        {
                            AppTheme selectedTheme = ((KeyValuePair<AppTheme, string>)picker.SelectedItem).Key;

                            Application.Current.UserAppTheme = selectedTheme;

                        };
                    })
                );

            });
        }
    }
}
