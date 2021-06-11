using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire.Notification
{
    public class HttpNotifier : INotify
    {
        #region Construct
        readonly HttpClient http = new HttpClient();

        public HttpNotifier(HttpNotifierConfiguration configuration)
        {
            if (configuration.RequestHeaders?.Any() ?? false)
            {
                foreach (IGrouping<string, Note> header in configuration.RequestHeaders.GroupBy(x => x.Id))
                {
                    http.DefaultRequestHeaders.Add(header.Key, header.Select(x => x.Value).ToArray());
                }
            }
            http.Timeout = configuration.Timeout;
        }
        #endregion

        public async Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            if (!to?.Any() ?? true)
                return OperationResult.Win("No destinations were specified");

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    using (StringContent httpBody = new StringContent(message.Content, message.Encoding, MapMimeType(message.ContentType)))
                    {
                        result = await DoHttpPost(httpBody, to);
                    }
                })
                .TryOrFailWithGrace(
                    numberOfTimes: 1,
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        private async Task<OperationResult> DoHttpPost(StringContent httpBody, NotificationEndpoint[] to)
        {
            OperationResult[] results
                = await
                    Task.WhenAll(
                        to.Select(x => DoHttpPost(httpBody, x)).ToArray()
                    );

            bool isSuccess = results.All(x => x.IsSuccessful);
            string[] reasons = results.SelectMany(x => x.FlattenReasons()).ToArray();

            return
                isSuccess
                ? OperationResult.Win(reason: null, reasons)
                : OperationResult.Fail(reason: "See comments for failure details", reasons);
        }

        private async Task<OperationResult> DoHttpPost(StringContent httpBody, NotificationEndpoint to)
        {
            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    using (HttpResponseMessage response = await http.PostAsync(to.Address.Address, httpBody))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            string reason = $"{response.StatusCode}: {response.ReasonPhrase ?? "[NoStatusReason]"}";
                            string responseContent = await response.Content.ReadAsStringAsync();

                            result
                                = to.IsOptional
                                ? OperationResult.Win($"{to.Address} is optional", reason, responseContent)
                                : OperationResult.Fail(reason, responseContent);

                            return;
                        }
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    numberOfTimes: 3,
                    onFail: ex => result = to.IsOptional ? OperationResult.Win($"{to.Address} is optional", ex.Flatten().Select(x => x.ToString()).ToArray()) : OperationResult.Fail(ex)
                );

            return result;
        }

        private string MapMimeType(NotificationMessageContentType contentType)
        {
            switch (contentType)
            {
                case NotificationMessageContentType.Html: return "text/html";
                case NotificationMessageContentType.JSON: return "application/json";
                case NotificationMessageContentType.LaTeX: return "application/x-latex";
                case NotificationMessageContentType.OpenXML: return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case NotificationMessageContentType.XML: return "application/xml";
                case NotificationMessageContentType.Plain:
                case NotificationMessageContentType.Markdown:
                default:
                    return "text/plain";
            }
        }
    }
}
