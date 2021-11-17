using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.UI
{
    public class UserOptionsContext
    {
        #region Construct
        readonly TaskCompletionSource<UserOptionSelectionResult> userOptionsTaskCompletionSource = new TaskCompletionSource<UserOptionSelectionResult>();
        readonly List<UserOption> selection = new List<UserOption>();

        public UserOptionsContext(string title, string descriptionHtml, bool isMultipleSelectionAllowed = false, params UserOption[] options)
        {
            Title = title;
            DescriptionHtml = descriptionHtml;
            Options = options?.Where(x => x != null).ToArray() ?? new UserOption[0];
            IsMultipleSelectionAllowed = isMultipleSelectionAllowed;
        }
        #endregion

        #region Properties
        public string Title { get; }
        public string DescriptionHtml { get; }
        public UserOption[] Options { get; }
        public bool IsMultipleSelectionAllowed { get; } = false;
        public Task<UserOptionSelectionResult> Task => userOptionsTaskCompletionSource.Task;
        #endregion

        #region Operations
        public void Select(UserOption option)
        {
            if (!selection.Contains(option)) selection.Add(option);

            if (!IsMultipleSelectionAllowed)
                ConfirmSelection();
        }

        public void ConfirmSelection()
        {
            userOptionsTaskCompletionSource.SetResult(UserOptionSelectionResult.Win(selection.ToArray()));
        }

        public void Cancel()
        {
            userOptionsTaskCompletionSource.SetResult(UserOptionSelectionResult.Fail());
        }
        #endregion
    }
}
