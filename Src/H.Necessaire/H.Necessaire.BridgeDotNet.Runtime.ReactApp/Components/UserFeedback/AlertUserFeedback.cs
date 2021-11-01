using Bridge.Html5;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AlertUserFeedback
    {
        public Task Go(params string[] messages)
        {
            Window.Alert(string.Join("\r\n", messages ?? new string[0]));

            return true.AsTask();
        }
    }
}
