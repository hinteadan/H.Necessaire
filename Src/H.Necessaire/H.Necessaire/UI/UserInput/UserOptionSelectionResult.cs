using System.Linq;

namespace H.Necessaire.UI
{
    public class UserOptionSelectionResult
    {
        private UserOptionSelectionResult(bool hasCanceled, params UserOption[] selectedOptions)
        {
            HasCanceled = hasCanceled;
            SelectedOptions = selectedOptions ?? new UserOption[0];
        }

        public bool HasCanceled { get; }
        public UserOption[] SelectedOptions { get; }
        public UserOption SelectedOption => SelectedOptions?.FirstOrDefault();

        public static UserOptionSelectionResult Win(params UserOption[] selectedOptions)
            => new UserOptionSelectionResult(false, selectedOptions);

        public static UserOptionSelectionResult Fail()
            => new UserOptionSelectionResult(true);
    }
}
