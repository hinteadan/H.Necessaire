using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HMauiLabelAndDescriptionContent : HMauiLabelAndDescriptionComponentBase
    {
        private ContentView contentView;
        public View LabeledContent
        {
            get => contentView.Content;
            set => contentView.Content = value;
        }

        protected override View ConstructLabeledContent()
        {
            return new ContentView
            {

            }.RefTo(out contentView);
        }
    }
}
