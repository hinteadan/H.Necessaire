using Bridge.Html5;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ConfirmUserFeedback
    {
        public Task<bool> Go(params string[] messages)
        {
            bool result = Window.Confirm(string.Join("\r\n", messages ?? new string[0]));

            return result.AsTask();
        }
    }
}
