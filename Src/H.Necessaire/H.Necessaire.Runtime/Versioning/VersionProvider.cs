using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Versioning
{
    internal class VersionProvider
    {
        static readonly Version defaultVersion = Version.Unknown;

        public async Task<Version> GetCurrentVersion()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            string versionResourceName = entryAssembly.GetManifestResourceNames()?.FirstOrDefault(x => x.EndsWith("version.txt", System.StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(versionResourceName))
                return defaultVersion;

            using (Stream stream = entryAssembly.GetManifestResourceStream(versionResourceName))
            {
                return Version.Parse(await stream.ReadAsStringAsync());
            }
        }
    }
}
