using H.Necessaire.Runtime;
using System;
using System.IO;
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
                        "NuSpecRootFolderPath".ConfigWith(GetCodebaseFolderPath()),
                        //"SqlConnections".ConfigWith(
                        //    "DefaultConnectionString".ConfigWith(ReadConnectionStringFromFile("DebugConnectionString.txt")),
                        //    "DatabaseNames".ConfigWith(
                        //        "Core".ConfigWith("H.Necessaire.CLI.Local.Core.Debug")
                        //    )
                        //),
                        "Google".ConfigWith(
                            "Firestore".ConfigWith(
                                "ProjectID".ConfigWith("free-tier-playground-409721"),
                                "DefaultCollectionName".ConfigWith("HNecessaireDefault"),
                                "Auth".ConfigWith(
                                    "Json".ConfigWith("free-tier-playground-409721-6f5f5d8af5a6.cfg.json")
                                )
                            )
                        ),
                        "AzureCosmosDB".ConfigWith(
                            //"URL".ConfigWith(ReadConnectionStringFromFile("AzureCosmosDB.URL.cfg.txt")),
                            "URL".ConfigWith("https://localhost:8081"),
                            "Keys".ConfigWith(
                                //"Primary".ConfigWith(ReadConnectionStringFromFile("AzureCosmosDB.Key.cfg.txt"))
                                "Primary".ConfigWith("C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==")
                            ),
                            "DatabaseID".ConfigWith("FreePlayDB"),
                            "ContainerID".ConfigWith("FreePlayContainer")
                        )
                    },
                }));
            ;
        }

        private static string? ReadConnectionStringFromFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                return null;

            string result = null;

            new Action(() =>
            {
                result = File.ReadAllText(fileInfo.FullName);
            })
            .TryOrFailWithGrace(onFail: ex => result = null);

            return result;
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
