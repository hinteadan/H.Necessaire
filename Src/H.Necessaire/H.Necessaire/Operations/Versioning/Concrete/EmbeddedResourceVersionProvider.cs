using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Versioning.Concrete
{
    internal class EmbeddedResourceVersionProvider : ImAVersionProvider
    {
        static readonly Version defaultVersion = Version.Unknown;

        static Version currentVersion = null;

        public async Task<Version> GetCurrentVersion()
        {
            if (currentVersion != null)
                return currentVersion;

            Assembly entryAssembly = Assembly.GetEntryAssembly();

            string versionResourceName = entryAssembly.GetManifestResourceNames()?.FirstOrDefault(x => x.EndsWith("version.txt", StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrWhiteSpace(versionResourceName))
            {
                currentVersion = defaultVersion;
                return currentVersion;
            }

            using (Stream stream = entryAssembly.GetManifestResourceStream(versionResourceName))
            {
                await
                    new Func<Task>(async () =>
                        currentVersion = Version.Parse(
                            await stream.ReadAsStringAsync()
                        )
                    )
                    .TryOrFailWithGrace(onFail: ex => currentVersion = defaultVersion);
            }

            return currentVersion;
        }
    }
}
