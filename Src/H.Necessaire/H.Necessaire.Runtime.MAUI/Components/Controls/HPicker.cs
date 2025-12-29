using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using System.Linq.Expressions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HPicker : HMauiLabelAndDescriptionComponentBase
    {
        Picker editor;
        public event EventHandler SelectedIndexChanged;
        protected override View ConstructLabeledContent()
        {
            return
                new Grid { }.And(layout =>
                {

                    layout.Add(
                        ConstructEditor()
                    );

                })
                .Bordered()
                ;
        }

        double fontSize = HUiToolkit.Current.Branding.Typography.FontSize;
        public double FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                if (editor is not null)
                    editor.FontSize = fontSize;
            }
        }

        public Picker Picker => editor;

        public int SelectedIndex { get => editor.SelectedIndex; set => editor.SelectedIndex = value; }
        public object SelectedItem
        {
            get
            {
                if (dataSource.IsEmpty())
                    return null;

                if (SelectedIndex < 0 || SelectedIndex >= dataSource.Length)
                    return null;

                return dataSource[editor.SelectedIndex];
            }
            set
            {
                if (dataSource.IsEmpty())
                    return;

                SelectedIndex = Array.IndexOf(dataSource, value);

            }
        }

        object[] dataSource;
        Delegate displayPropertySelector = null;
        public HPicker SetDataSource<TDataSource, TProperty>(IEnumerable<TDataSource> dataSource, Expression<Func<TDataSource, TProperty>> selector)
        {
            displayPropertySelector = selector?.Compile();
            this.dataSource = dataSource?.Select(x => x as object).ToArray();
            RefreshEditorItems();
            return this;
        }
        public HPicker SetDataSource<TDataSource>(IEnumerable<TDataSource> dataSource)
            => SetDataSource<TDataSource, object>(dataSource, null);
        public string[] ItemsDisplayed
        {
            get
            {
                if (dataSource.IsEmpty())
                    return null;

                if (displayPropertySelector is null)
                    return dataSource.Select(x => x?.ToString()).ToArray();

                return dataSource.Select(x => displayPropertySelector.DynamicInvoke(x)?.ToString()).ToArray();
            }
        }

        BindingBase itemDisplayBinding = null;
        public BindingBase ItemDisplayBinding
        {
            get => itemDisplayBinding;
            set
            {
                itemDisplayBinding = value;
                if (editor is not null)
                    editor.ItemDisplayBinding = itemDisplayBinding;
            }
        }

        View ConstructEditor()
        {
            return new Picker
            {
                FontFamily = Branding.Typography.FontFamily,
                FontSize = fontSize,
                TextColor = Branding.TextColor.ToMaui(),
                BackgroundColor = Branding.BackgroundColorTranslucent.ToMaui(),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            }
            .And(x => x.ItemDisplayBinding = itemDisplayBinding)
            .And(x => editor = x)
            ;
        }

        protected override async Task Initialize()
        {
            await base.Initialize();
            HSafe.Run(() => editor.SelectedIndexChanged -= OnPickerSelectedIndexChanged);
            editor.SelectedIndexChanged += OnPickerSelectedIndexChanged;
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            IfNotBinding(_ => SelectedIndexChanged?.Invoke(sender, e));
        }

        void RefreshEditorItems()
        {
            if (editor is null)
                return;

            editor.ItemsSource = ItemsDisplayed?.ToList();
        }
    }
}
