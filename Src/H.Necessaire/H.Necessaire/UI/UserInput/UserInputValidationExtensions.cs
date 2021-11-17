using System.Threading.Tasks;

namespace H.Necessaire.UI
{
    public static class UserInputValidationExtensions
    {
        public static Task<OperationResult> Required(this string value, string message = "This field is required")
        {
            if (string.IsNullOrWhiteSpace(value))
                return OperationResult.Fail(message).AsTask();
            return OperationResult.Win().AsTask();
        }
    }
}
