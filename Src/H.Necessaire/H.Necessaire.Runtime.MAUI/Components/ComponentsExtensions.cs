using H.Necessaire.Runtime.MAUI.Components.Controls;
using H.Necessaire.Runtime.MAUI.Components.Elements;

namespace H.Necessaire.Runtime.MAUI.Components
{
    public static class ComponentsExtensions
    {
        public static View Bordered(this View view)
        {
            return new HBorderedContent(view);
        }

        public static HNullableControl Nullable(this View view, string nullText = "Any")
        {
            return new HNullableControl
            {
                Content = view,
                NullText = nullText,
            };
        }

        public static HMauiLabelAndDescriptionContent WithLabelAndDescription(this View view, string label = null, string description = null)
        {
            return new HMauiLabelAndDescriptionContent
            {
                Content = view,
                Label = label,
                Description = description,
            };
        }
    }
}
