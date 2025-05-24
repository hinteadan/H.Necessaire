using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiLabelAndDescriptionComponentBase : HMauiComponentBase
    {
        ContentPresenter contentPresenter;
        VerticalStackLayout layout;
        HLabel label;
        HLabel description;

        public HMauiLabelAndDescriptionComponentBase(params object[] constructionArgs) : base(constructionArgs) { }


        protected override void Construct()
        {
            layout
                = new VerticalStackLayout()
                .And(layout =>
                {
                    this.layout = layout;

                    label = new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeSmall,
                        TextColor = Branding.PrimaryColor.ToMaui(),
                    };

                    description = new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeSmaller,
                        TextColor = Branding.SecondaryColor.ToMaui(),
                    };

                    contentPresenter = new ContentPresenter();
                    contentPresenter.Content = ConstructLabeledContent();

                    layout.Add(new ContentView
                    {
                        Content = contentPresenter
                    });

                });

            base.Construct();
        }

        protected override View WrapReceivedContent(View content)
        {
            contentPresenter.Content = content;
            return base.WrapReceivedContent(content);
        }

        protected sealed override View ConstructContent() => layout;

        protected virtual View ConstructLabeledContent() => null;

        public string Label
        {
            get => label.Text;
            set
            {
                label.Text = value;
                if (value.IsEmpty())
                    layout.Remove(label);
                else
                    layout.Insert(0, label);
            }
        }

        public string Description
        {
            get => description.Text;
            set
            {
                description.Text = value;
                if (value.IsEmpty())
                    layout.Remove(description);
                else
                    layout.Add(description);
            }
        }
    }
}
