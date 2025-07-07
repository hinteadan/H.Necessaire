using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class PageExtensions
    {
        public static TPage Responsive<TPage>(this TPage page, Func<TPage, Action[]> clearActions, Func<TPage, HHeightCategoryAction[]> heightCategoryActions, Func<TPage, HWidthCategoryAction[]> widthCategoryActions = null) where TPage : HMauiPageBase
        {
            if (page == null)
                return page;

            page.ActResponsive(clearActions, heightCategoryActions, widthCategoryActions).RegisterTo(page);

            return page;
        }

        public static TPage Bind<TPage>(this TPage ctrl, Action<TPage> clear, Action<TPage> bind) where TPage : HMauiPageBase
        {
            if (clear is not null)
                ctrl.Act(clear).RegisterTo(ctrl.ClearUIActions);
            if (bind is not null)
                ctrl.Act(bind).RegisterTo(ctrl.RefreshUIActions);
            return ctrl;
        }
    }
}
