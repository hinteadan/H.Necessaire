using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Versioning
{
    internal class VersionResource : HttpApiResourceBase
    {
        static readonly Version defaultVersion = Version.Unknown;

        public async Task<Version> GetCurrentVersion()
        {
            OperationResult<Version> httpRequestResult =
                await SafelyRequest(() => httpClient.GetJson<Version>($"{BaseAiUrl}/Version/json"));

            if (!httpRequestResult.IsSuccessful)
                return defaultVersion;

            return httpRequestResult.Payload;
        }
        public async Task<string> GetCurrentVersionAsString()
        {
            OperationResult<string> httpRequestResult =
                await SafelyRequest(() => httpClient.GetJson<string>($"{BaseAiUrl}/Version"));

            if (!httpRequestResult.IsSuccessful)
                return defaultVersion.ToString();

            return httpRequestResult.Payload;
        }
    }
}
