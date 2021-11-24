using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Versioning
{
    internal class VersionResource : HttpApiResourceBase, ImAVersionProvider
    {
        static readonly Version defaultVersion = Version.Unknown;
        static Version currentVersion = null;
        static string currentVersionAsString = null;

        public async Task<Version> GetCurrentVersion()
        {
            if (currentVersion != null)
                return currentVersion;

            OperationResult<Version> httpRequestResult =
                await SafelyRequest(() => httpClient.GetJson<Version>($"{BaseAiUrl}/Version/json"));

            if (!httpRequestResult.IsSuccessful)
            {
                currentVersion = defaultVersion;
                return currentVersion;
            }

            currentVersion = httpRequestResult.Payload;
            currentVersionAsString = currentVersion.ToString();

            return currentVersion;
        }
        public async Task<string> GetCurrentVersionAsString()
        {
            if (currentVersionAsString != null)
                return currentVersionAsString;

            OperationResult<string> httpRequestResult =
                await SafelyRequest(() => httpClient.GetJson<string>($"{BaseAiUrl}/Version"));

            if (!httpRequestResult.IsSuccessful)
            {
                currentVersionAsString = defaultVersion.ToString();
                return currentVersionAsString;
            }

            currentVersionAsString = httpRequestResult.Payload;

            return httpRequestResult.Payload;
        }
    }
}
