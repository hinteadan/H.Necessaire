using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Notification
{
    public class HttpNotifier : INotify
    {
        #region Construct
        readonly HttpClient http = new HttpClient();
        #endregion

        public async Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            OperationResult result = OperationResult.Win();

            //await
            //    new Func<Task>(async () =>
            //    {
            //        using (StringContent httpBody = new StringContent(message.Content, Encoding.UTF8, "application/json"))
            //        using (HttpResponseMessage response = await http.PostAsync(config.PostUrl, httpBody))
            //        {
            //            if (!response.IsSuccessStatusCode)
            //            {
            //                result = OperationResult.Fail($"{response.StatusCode}: {response.ReasonPhrase ?? "[NoStatusReason]"}", await response.Content.ReadAsStringAsync());
            //                return;
            //            }
            //        }

            //        result = OperationResult.Win();

            //    })
            //    .TryOrFailWithGrace(
            //        numberOfTimes: 3,
            //        onFail: ex => result = OperationResult.Fail(ex)
            //    );

            return result;
        }
    }
}
