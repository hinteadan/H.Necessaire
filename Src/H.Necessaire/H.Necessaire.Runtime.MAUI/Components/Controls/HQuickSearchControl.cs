using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HQuickSearchControl : SearchBar, IDisposable
    {
        static readonly TimeSpan defaultDebounceInterval = TimeSpan.FromSeconds(.37);
        public sbyte MinSearchKeyLength { get; set; } = 2;
        public sbyte MaxSearchKeys { get; set; } = 5;

        readonly AsyncEventRaiser<HQuickSearchEventArgs> searchEventRaiser;
        public event AsyncEventHandler<HQuickSearchEventArgs> OnSearch { add => searchEventRaiser.OnEvent += value; remove => searchEventRaiser.OnEvent -= value; }

        readonly Debouncer debouncedSearchRaiser;
        public HQuickSearchControl(TimeSpan? debounceInterval)
        {
            searchEventRaiser = new AsyncEventRaiser<HQuickSearchEventArgs>(this);

            debouncedSearchRaiser = new Debouncer(RaiseOnSearch, debounceInterval ?? defaultDebounceInterval);

            Text = " ";
            FontFamily = HUiToolkit.Current.Branding.Typography.FontFamily;
            FontSize = HUiToolkit.Current.Branding.Typography.FontSize;
            TextColor = HUiToolkit.Current.Branding.TextColor.ToMaui();
            BackgroundColor = HUiToolkit.Current.Branding.BackgroundColorTranslucent.ToMaui();
            PlaceholderColor = HUiToolkit.Current.Branding.TextColor.WithOpacity(.3f).ToMaui();
            VerticalTextAlignment = TextAlignment.Center;

            Keyboard = Keyboard.Text;

            SearchButtonPressed += HQuickSearchControl_SearchButtonPressed;
            TextChanged += HQuickSearchControl_TextChanged;
        }
        public HQuickSearchControl() : this(null) { }

        ~HQuickSearchControl()
        {
            HSafe.Run(Dispose);
        }

        public void Dispose()
        {
            SearchButtonPressed -= HQuickSearchControl_SearchButtonPressed;
            TextChanged -= HQuickSearchControl_TextChanged;
        }

        public string[] GetSearchKeys()
        {
            string srcString = Text;

            if (srcString.IsEmpty())
                return null;

            string[] keys
                = srcString
                .DiacriticLess()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .TrimToValidKeywordsOnly(minLength: MinSearchKeyLength, maxNumberOfKeywords: MaxSearchKeys)
                ;

            if (keys.IsEmpty())
                return null;

            return keys;
        }

        async void HQuickSearchControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HUiToolkit.Current.IsPageBinding)
                return;

            await debouncedSearchRaiser.Invoke();
        }

        async void HQuickSearchControl_SearchButtonPressed(object sender, EventArgs e)
        {
            if (HUiToolkit.Current.IsPageBinding)
                return;

            await this.HideSoftInputAsync(CancellationToken.None);

            await RaiseOnSearch();
        }

        async Task RaiseOnSearch()
        {
            if (HUiToolkit.Current.IsPageBinding)
                return;

            await searchEventRaiser.Raise(new HQuickSearchEventArgs(GetSearchKeys()));
        }
    }

    public class HQuickSearchEventArgs : EventArgs
    {
        public HQuickSearchEventArgs(string[] searchKeys)
        {
            SearchKeys = searchKeys;
        }

        public string[] SearchKeys { get; }
    }
}
