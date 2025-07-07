using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace H.Necessaire.Runtime.UI
{
    public static class ResponsiveExtensions
    {
        public static HResponsiveDeclaration ActResponsive<T>(this T control, Func<T, Action[]> clearActions, Func<T, HHeightCategoryAction[]> heightCategoryActions, Func<T, HWidthCategoryAction[]> widthCategoryActions = null)
        {
            if (control == null)
                return null;

            return new HResponsiveDeclaration
            {
                ClearActions = clearActions?.Invoke(control),
                HeightCategoryActions = heightCategoryActions?.Invoke(control),
                WidthCategoryActions = widthCategoryActions?.Invoke(control),
            };
        }

        public static void RegisterTo(this HResponsiveDeclaration responsiveDeclaration, IList<HResponsiveDeclaration> registry)
        {
            if (registry is null)
                return;

            if (responsiveDeclaration.IsEmpty())
                return;

            registry.Add(responsiveDeclaration);
        }

        public static bool IsEmpty(this HResponsiveDeclaration responsiveDeclaration)
        {
            return
                (responsiveDeclaration?.ClearActions).IsEmpty()
                && (responsiveDeclaration?.HeightCategoryActions).IsEmpty()
                && (responsiveDeclaration?.WidthCategoryActions).IsEmpty()
                ;
        }
    }
}
