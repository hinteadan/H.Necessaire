using H.Necessaire.Runtime.MAUI.Components.Elements;

namespace H.Necessaire.Runtime.MAUI.Components
{
    public static class ComponentsExtensions
    {
        public static View Bordered(this View view)
        {
            return new HBorderedContent(view);
        }
    }
}
