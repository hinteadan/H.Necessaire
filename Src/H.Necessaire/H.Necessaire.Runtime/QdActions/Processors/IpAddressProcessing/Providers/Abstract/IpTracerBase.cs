using H.Necessaire.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Abstract
{
    internal abstract class IpTracerBase<TProviderModel> : ImANetworkTraceProvider
    {
        #region Config
        public abstract InternalIdentity About { get; }
        protected virtual TimeSpan RequiredCooldown => TimeSpan.Zero;
        protected abstract string UrlProvider(string networkAddress);
        protected abstract bool IsProviderModelSuccessful(TProviderModel model, out string message);
        protected abstract NetworkTrace Map(TProviderModel model);
        #endregion

        #region Construct
        DateTime lastRequestDoneAt = DateTime.UtcNow;

        private static readonly HttpClient httpClient = new HttpClient();
        #endregion

        #region Operations
        public async Task<OperationResult<NetworkTrace>> Trace(string networkAddress)
        {
            await Cooldown();

            string url = UrlProvider(networkAddress);

            string rawProviderPayload = await httpClient.GetStringAsync(url);

            OperationResult<TProviderModel> parseResult = rawProviderPayload.TryJsonToObject<TProviderModel>();

            if (!parseResult.IsSuccessful)
                return parseResult.WithoutPayload<NetworkTrace>();

            TProviderModel providerModel = parseResult.Payload;

            lastRequestDoneAt = DateTime.UtcNow;

            if (!IsProviderModelSuccessful(providerModel, out string providerMessage))
                return OperationResult.Fail($"Error tracing network address {networkAddress} via {About.DisplayName}; Provider message: {providerMessage ?? "<<none>>"}").WithoutPayload<NetworkTrace>();

            OperationResult<NetworkTrace> result = OperationResult.Fail("Not yet mapped").WithoutPayload<NetworkTrace>();

            new Action(() =>
            {
                result
                    = OperationResult.Win().WithPayload(
                        Map(providerModel)
                        .And(x =>
                        {
                            x.NetworkTraceProvider = About;
                            x.IpAddress = networkAddress;
                        })
                    );

            })
            .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<NetworkTrace>());

            return result;
        }

        private async Task Cooldown()
        {
            if (RequiredCooldown == TimeSpan.Zero)
                return;

            if (DateTime.UtcNow - lastRequestDoneAt > RequiredCooldown)
                return;

            await Task.Delay(RequiredCooldown);
        }
        #endregion
    }
}
