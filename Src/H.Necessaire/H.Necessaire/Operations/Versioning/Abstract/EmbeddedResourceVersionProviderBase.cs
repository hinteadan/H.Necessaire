using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Versioning.Abstract
{
    public abstract class EmbeddedResourceVersionProviderBase : ImAVersionProvider
    {
        static readonly Version defaultVersion = Version.Unknown;
        static Version currentVersion = null;
        readonly Assembly[] assembliesToScan = null;
        readonly string versionFileName = "version.txt";
        protected EmbeddedResourceVersionProviderBase(string versionFileName, params Assembly[] assembliesToScan)
        {
            this.assembliesToScan = !assembliesToScan.IsEmpty() ? assembliesToScan : (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).AsArray();
            this.versionFileName = versionFileName;
        }
        protected EmbeddedResourceVersionProviderBase(params Assembly[] assembliesToScan) : this(versionFileName: "version.txt", assembliesToScan) { }
        protected EmbeddedResourceVersionProviderBase() : this(versionFileName: "version.txt", assembliesToScan: null) { }

        public virtual async Task<Version> GetCurrentVersion()
        {
            if (currentVersion != null)
                return currentVersion;

            foreach (Assembly assembly in assembliesToScan)
            {
                string versionResourceName = assembly.GetManifestResourceNames()?.FirstOrDefault(x => x.EndsWith(versionFileName, StringComparison.InvariantCultureIgnoreCase));

                if (string.IsNullOrWhiteSpace(versionResourceName))
                {
                    currentVersion = defaultVersion;
                    return currentVersion;
                }

                using (Stream stream = assembly.GetManifestResourceStream(versionResourceName))
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

            currentVersion = defaultVersion;
            return currentVersion;
        }
    }
}
