using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using System.Collections;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HPicker : HMauiLabelAndDescriptionComponentBase
    {
        Picker editor;

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

        public Picker Picker => editor;

        public int SelectedIndex { get => editor.SelectedIndex; set => editor.SelectedIndex = value; }
        public object SelectedItem { get => editor.SelectedItem; set => editor.SelectedItem = value; }
        public IList ItemSource { get => editor.ItemsSource; set => editor.ItemsSource = value; }
        public IList<string> Items => editor.Items;

        View ConstructEditor()
        {
            return new Picker
            {
                FontFamily = Branding.Typography.FontFamily,
                FontSize = Branding.Typography.FontSize,
                TextColor = Branding.TextColor.ToMaui(),
                BackgroundColor = Branding.BackgroundColorTranslucent.ToMaui(),
                VerticalTextAlignment = TextAlignment.Center,
            }
            .And(x => x.SelectedIndexChanged += OnPickerSelectedIndexChanged)
            .And(x => editor = x)
            ;
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
