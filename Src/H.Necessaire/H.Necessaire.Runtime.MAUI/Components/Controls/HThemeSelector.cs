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

        HPicker themePicker = null;

        public AppTheme Theme
        {
            get => ((KeyValuePair<AppTheme, string>)themePicker.SelectedItem).Key;
            set
            {
                SelectTheme(value);
            }
        }

        protected override View ConstructContent()
        {
            return new Grid { }.And(layout =>
            {
                layout.Add(
                    new HPicker { Label = "Theme" }
                    .SetDataSource(options, x => x.Value)
                    .RefTo(out themePicker)
                );
            });
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            AppTheme currentTheme = Application.Current.UserAppTheme;
            HSafe.Run(() => themePicker.SelectedIndexChanged -= ThemePicker_SelectedIndexChanged);
            themePicker.SelectedItem = options.Single(x => x.Key == currentTheme);
            themePicker.SelectedIndexChanged += ThemePicker_SelectedIndexChanged;
        }

        protected override async Task Destroy()
        {
            HSafe.Run(() => themePicker.SelectedIndexChanged -= ThemePicker_SelectedIndexChanged);

            await base.Destroy();
        }

        void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppTheme selectedTheme = ((KeyValuePair<AppTheme, string>)themePicker.SelectedItem).Key;

            Application.Current.UserAppTheme = selectedTheme;
        }

        void SelectTheme(AppTheme theme)
        {
            AppTheme selectedTheme = ((KeyValuePair<AppTheme, string>)themePicker.SelectedItem).Key;
            if (theme == selectedTheme)
                return;

            themePicker.SelectedItem = options.Single(x => x.Key == theme);
        }
    }
}
