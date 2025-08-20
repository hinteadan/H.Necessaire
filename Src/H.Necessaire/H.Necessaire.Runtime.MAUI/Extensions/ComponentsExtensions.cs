using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.UI;
using H.Necessaire.UI;

namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class ComponentsExtensions
    {
        static Page CurrentPage => Application.Current?.Windows?.FirstOrDefault()?.Page;
        static INavigation Navigation => CurrentPage?.Navigation;

        public static void RegisterTo(this HResponsiveDeclaration responsiveDeclaration, HMauiPageBase page)
        {
            if (page is null)
                return;

            if (responsiveDeclaration.IsEmpty())
                return;

            page.ResponsiveDeclarations.Add(responsiveDeclaration);
        }

        public static T Responsive<T>(this T control, HMauiPageBase page, Func<T, Action[]> clearActions, Func<T, HHeightCategoryAction[]> heightCategoryActions, Func<T, HWidthCategoryAction[]> widthCategoryActions = null)
        {
            if (control == null)
                return control;

            control.ActResponsive(clearActions, heightCategoryActions, widthCategoryActions).RegisterTo(page);

            return control;
        }

        public static TCtrl Bind<TCtrl>(this TCtrl ctrl, HMauiComponentBase dataHolder, Action<TCtrl> clear, Action<TCtrl> bind)
        {
            if (clear is not null)
                ctrl.Act(clear).RegisterTo(dataHolder.ClearUIActions);
            if (bind is not null)
                ctrl.Act(bind).RegisterTo(dataHolder.RefreshUIActions);
            return ctrl;
        }
        public static TCtrl Bind<TCtrl>(this TCtrl ctrl, HMauiPageBase page, Action<TCtrl> clear, Action<TCtrl> bind)
        {
            if (clear is not null)
                ctrl.Act(clear).RegisterTo(page.ClearUIActions);
            if (bind is not null)
                ctrl.Act(bind).RegisterTo(page.RefreshUIActions);
            return ctrl;
        }

        public static async Task<OperationResult<UserOption[]>> ShowModal<TPage>(this TPage modalPage) where TPage : Page, ImAModal
        {
            if (modalPage is null)
                return "Modal page is NULL";

            INavigation navigation = Navigation;
            if (navigation is null)
                return "Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation is NULL";

            await navigation.PushModalAsync(modalPage.RefTo(out var modal));
            OperationResult<UserOption[]> modalResult = await modal.GetModalResult();

            return modalResult;
        }
    }
}
