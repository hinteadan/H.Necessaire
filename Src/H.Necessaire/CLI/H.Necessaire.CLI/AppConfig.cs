using H.Necessaire.Runtime;
using System.Reflection;

namespace H.Necessaire.CLI
{
    static class AppConfig
    {
        const string srcFolderRelativePath = "/src/h.necessaire/";

        public static ImAnApiWireup WithDefaultRuntimeConfig(this ImAnApiWireup wireup)
        {
            return
                wireup
                .With(x => x.Register<RuntimeConfig>(() => new RuntimeConfig
                {
                    Values = new[] {
                        "NuSpectRootFolderPath".ConfigWith(GetCodebaseFolderPath()),
                        "SqlConnections".ConfigWith(
                            "DefaultConnectionString".ConfigWith(File.ReadAllText("DebugConnectionString.txt")),
                            "DatabaseNames".ConfigWith(
                                "Core".ConfigWith("H.Necessaire.Core.Debug")
                            )
                        ),
                    },
                }));
            ;
        }

        private static string GetCodebaseFolderPath()
        {
            string codeBase = Assembly.GetExecutingAssembly()?.Location ?? string.Empty;
            UriBuilder uri = new UriBuilder(codeBase);
            string dllPath = Uri.UnescapeDataString(uri.Path);
            int srcFolderIndex = dllPath.ToLowerInvariant().IndexOf(srcFolderRelativePath);
            if (srcFolderIndex < 0)
                return string.Empty;
            string srcFolderPath = Path.GetDirectoryName(dllPath.Substring(0, srcFolderIndex + srcFolderRelativePath.Length)) ?? string.Empty;
            return srcFolderPath;
        }
    }
}
