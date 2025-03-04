using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HThemeSelector : HMauiComponentBase
    {
        static readonly KeyValuePair<string, AppTheme>[] options = new KeyValuePair<string, AppTheme>[] {
            new KeyValuePair<string, AppTheme>("System", AppTheme.Unspecified),
            new KeyValuePair<string, AppTheme>("Light", AppTheme.Light),
            new KeyValuePair<string, AppTheme>("Dark", AppTheme.Dark),
        };

        protected override View ConstructContent()
        {
            AppTheme currentTheme = GetCurrentTheme();

            return new Grid { }.And(layout =>
            {

                layout.Add(new HPicker
                {
                    ItemSource = options,
                    ItemDisplayBinding = Binding.Create(static (KeyValuePair<string, AppTheme> entry) => entry.Key),
                    SelectedIndex = Array.IndexOf(options, options.Single(x => x.Value == currentTheme)),

                }.And(picker =>
                {
                    picker.SelectedIndexChanged += (s, e) =>
                    {
                        AppTheme selectedTheme = ((KeyValuePair<string, AppTheme>)picker.SelectedItem).Value;

                        Application.Current.UserAppTheme = selectedTheme;

                    };
                }));

            });
        }

        static AppTheme GetCurrentTheme()
        {
            AppTheme? theme = Application.Current?.UserAppTheme;
            if (theme is not null && theme != AppTheme.Unspecified)
                return theme.Value;

            return Application.Current?.RequestedTheme ?? AppTheme.Unspecified;
        }
    }
}
