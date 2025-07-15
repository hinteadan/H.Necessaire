using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiLabelAndDescriptionComponentBase : HMauiComponentBase
    {
        ContentView contentView;
        VerticalStackLayout layout;
        protected HLabel label;
        protected HLabel description;

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
                        TextColor = Branding.Theme == AppTheme.Dark ? Branding.Colors.Primary.Lighter(2).ToMaui() : Branding.PrimaryColor.ToMaui(),
                    };

                    description = new HLabel
                    {
                        FontSize = Branding.Typography.FontSizeSmaller,
                        TextColor = Branding.Theme == AppTheme.Dark ? Branding.Colors.PrimaryIsh().Lighter(2).ToMaui() : Branding.SecondaryColor.ToMaui(),
                    };

                    layout.Add(new ContentView
                    {
                        Content = ConstructLabeledContent()
                    }.RefTo(out contentView));

                });

            base.Construct();
        }

        protected override View WrapReceivedContent(View content)
        {
            contentView.Content = content;
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
                else if(!layout.Contains(label))
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
                else if (!layout.Contains(description))
                    layout.Add(description);
            }
        }
    }
}
