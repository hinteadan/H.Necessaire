using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Layouts
{
    public class HVerticalStack : HMauiComponentBase
    {
        VerticalStackLayout layout;
        protected override View ConstructContent()
        {
            return new VerticalStackLayout
            {

            }
            .RefTo(out layout);
        }

        public IView[] Views
        {
            get => layout.Children?.ToArrayNullIfEmpty();
            set
            {
                layout.Clear();
                if (value.IsEmpty())
                {
                    return;
                }

                foreach (IView view in value)
                {
                    layout.Add(view);
                }
            }
        }

        public void Clear() => layout.Clear();
        public void Append(IView view)
        {
            if (view is null)
                return;

            layout.Add(view);
        }
    }
}
