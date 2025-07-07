using System;

namespace H.Necessaire.Runtime.UI
{
    public class HResponsiveDeclaration
    {
        public HResponsiveDeclaration(Action[] clearActions, HHeightCategoryAction[] heightCategoryActions, HWidthCategoryAction[] widthCategoryActions = null)
        {
            ClearActions = clearActions;
            HeightCategoryActions = heightCategoryActions;
            WidthCategoryActions = widthCategoryActions;
        }
        public HResponsiveDeclaration() : this(null, null) { }

        public Action[] ClearActions { get; set; }
        public HHeightCategoryAction[] HeightCategoryActions { get; set; }
        public HWidthCategoryAction[] WidthCategoryActions { get; set; }
    }
}
