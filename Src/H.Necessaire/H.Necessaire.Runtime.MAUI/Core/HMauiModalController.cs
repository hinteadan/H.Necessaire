using H.Necessaire.UI;

namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class HMauiModalController : ImAModalController
    {
        static Page CurrentPage => Application.Current?.Windows?.FirstOrDefault()?.Page;
        static INavigation Navigation => CurrentPage?.Navigation;

        readonly TaskCompletionSource<OperationResult<UserOption[]>> modalCompletionSource = new();

        public async Task CancelModal()
        {
            INavigation navigation = Navigation;
            if (navigation is null)
                return;

            modalCompletionSource.SetResult("User Cancelled");
            await navigation.PopModalAsync();
        }

        public async Task SubmitModal(params UserOption[] userSelection)
        {
            INavigation navigation = Navigation;
            if (navigation is null)
                return;

            modalCompletionSource.SetResult(userSelection);
            await navigation.PopModalAsync();
        }

        public Task<OperationResult<UserOption[]>> GetModalResult()
        {
            return modalCompletionSource.Task;
        }
    }
}
