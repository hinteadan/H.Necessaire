using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks.Implementations
{
    class DefaultWebHookService : IWebHookService
    {
        #region Construct
        readonly IWebHookRequestStorage webHookRequestStorage;
        readonly IWebHookProcessingResultStorage webHookProcessingResultStorage;
        readonly IWebHookProcessor[] webHookProcessors = new IWebHookProcessor[0];
        public DefaultWebHookService(
            IWebHookRequestStorage webHookRequestStorage,
            IWebHookProcessingResultStorage webHookProcessingResultStorage,
            IWebHookProcessor[] webHookProcessors
            )
        {
            this.webHookRequestStorage = webHookRequestStorage;
            this.webHookProcessingResultStorage = webHookProcessingResultStorage;
            this.webHookProcessors = webHookProcessors ?? new IWebHookProcessor[0];
        }
        #endregion

        public async Task<OperationResult> Hook(IWebHookRequest request)
        {
            await webHookRequestStorage.Append(request);

            if (!webHookProcessors?.Any() ?? true)
            {
                OperationResult x = OperationResult.Win("There are no web hook processors defined. The web hook request was simply audited.");
                await webHookProcessingResultStorage.Append(BuildWebHookProcessingResult(request, x));
                return x;
            }

            OperationResult[] results = await Task.WhenAll(webHookProcessors.Select(x => RunProcessor(x, request)).ToArray());

            bool isSuccess = results.All(x => x.IsSuccessful);
            string[] reasons = results.SelectMany(x => x.FlattenReasons()).ToArray();
            string mainReason = !reasons.Any() ? null : reasons.Length == 1 ? reasons.Single() : "There are multiple reasons. See comments for details.";
            string[] moreReasons = reasons.Length <= 1 ? null : reasons;

            OperationResult result =
                isSuccess
                ? OperationResult.Win(mainReason, moreReasons)
                : OperationResult.Fail(mainReason, moreReasons)
                ;

            await webHookProcessingResultStorage.Append(BuildWebHookProcessingResult(request, result));

            return result;

        }

        async Task<OperationResult> RunProcessor(IWebHookProcessor webHookProcessor, IWebHookRequest request)
        {
            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    bool willHandle = await webHookProcessor.WillHandle(request);

                    if (!willHandle)
                        return;

                    result = await webHookProcessor.Process(request);
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        WebHookProcessingResult BuildWebHookProcessingResult(IWebHookRequest request, OperationResult result)
        {
            return
                new WebHookProcessingResult
                {
                    WebHookRequestID = request.ID,
                    Comments = result.Comments,
                    IsSuccessful = result.IsSuccessful,
                    Reason = result.Reason,
                };
        }
    }
}
