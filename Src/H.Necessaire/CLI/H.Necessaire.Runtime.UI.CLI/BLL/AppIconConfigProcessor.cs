using System;
using System.Text;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    class AppIconConfigProcessor
    {
        internal async Task<OperationResult> Process(string appName, DirectoryInfo appRootFolder, FileInfo svgIconFile, string appIconBaseSize = null, string splashIconBaseSize = null)
        {
            TaggedValue<FileInfo>[] iconFiles = await FindIconFiles(appName, appRootFolder);

            string svgIconFileContent = await File.ReadAllTextAsync(svgIconFile.FullName);

            await ProcessBaseSizesIfNecessary(appName, appRootFolder, appIconBaseSize, splashIconBaseSize);

            await Task.WhenAll(
                iconFiles.Select(x => File.WriteAllTextAsync(x.Value.FullName, svgIconFileContent))
            );

            return OperationResult.Win();
        }

        static async Task ProcessBaseSizesIfNecessary(string appName, DirectoryInfo appRootFolder, string appIconBaseSize, string splashIconBaseSize)
        {
            if (appIconBaseSize.IsEmpty() && splashIconBaseSize.IsEmpty())
                return;

            string csProj
                = appName.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                ? appName
                : $"{appName}.csproj"
                ;
            FileInfo csProjFile = new FileInfo(Path.Combine(appRootFolder.FullName, csProj));

            string fileContent = await File.ReadAllTextAsync(csProjFile.FullName);



            StringBuilder newContentBuilder = new StringBuilder();
            int startFrom = 0;


            if (!appIconBaseSize.IsEmpty())
            {
                ProcessBaseSizeTag("<MauiIcon", appIconBaseSize, fileContent, newContentBuilder, startFrom, out startFrom);
            }

            if (!splashIconBaseSize.IsEmpty())
            {
                ProcessBaseSizeTag("<MauiSplashScreen", splashIconBaseSize, fileContent, newContentBuilder, startFrom, out startFrom);
            }


            newContentBuilder
                .Append(fileContent.Substring(startFrom))
                ;

            await File.WriteAllTextAsync(csProjFile.FullName, newContentBuilder.ToString());
        }

        private static void ProcessBaseSizeTag(string tagID, string baseSize, string fileContent, StringBuilder newContentBuilder, int startFromIndexInFileContent, out int newStartFromIndexInFileContent)
        {
            int tagStartIndex = fileContent.IndexOf(tagID, startFromIndexInFileContent);
            int tagEndIndex = fileContent.IndexOf("/>", tagStartIndex);
            string tag = fileContent.Substring(tagStartIndex, tagEndIndex - tagStartIndex);
            int relativeIndexOfBaseSize = tag.IndexOf("BaseSize=\"");
            bool isBaseSizeAlreadySet = relativeIndexOfBaseSize >= 0;

            if (!isBaseSizeAlreadySet)
            {
                newContentBuilder
                    .Append(fileContent.Substring(startFromIndexInFileContent, tagStartIndex - startFromIndexInFileContent))
                    .Append(tag)
                    .Append("BaseSize=\"").Append(baseSize).Append("\" ")
                    .And(_ => startFromIndexInFileContent = tagEndIndex)
                    ;
            }
            else
            {
                int relativeEndIndexOfBaseSize = tag.IndexOf("\"", relativeIndexOfBaseSize + "BaseSize=\"".Length);
                newContentBuilder
                    .Append(fileContent.Substring(startFromIndexInFileContent, tagStartIndex - startFromIndexInFileContent))
                    .Append(tag.Substring(0, relativeIndexOfBaseSize))
                    .Append("BaseSize=\"").Append(baseSize)
                    .Append(tag.Substring(relativeEndIndexOfBaseSize))
                    .And(_ => startFromIndexInFileContent = tagEndIndex)
                    ;
            }

            newStartFromIndexInFileContent = startFromIndexInFileContent;
        }

        static async Task<TaggedValue<FileInfo>[]> FindIconFiles(string appName, DirectoryInfo appRootFolder)
        {
            TaggedValue<FileInfo>[] result = new TaggedValue<FileInfo>[2];

            string csProj
                = appName.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)
                ? appName
                : $"{appName}.csproj"
                ;
            FileInfo csProjFile = new FileInfo(Path.Combine(appRootFolder.FullName, csProj));

            string csProjContent = await File.ReadAllTextAsync(csProjFile.FullName);

            string searchKey = "<MauiIcon Include=\"Resources\\AppIcon\\appicon.svg\" ForegroundFile=\"";
            int startFrom = 0;
            int startIndex = csProjContent.IndexOf(searchKey, startFrom) + searchKey.Length;
            int endIndex = csProjContent.IndexOf("\"", startIndex);
            int length = endIndex - startIndex;
            string relativePath = csProjContent.Substring(startIndex, length);
            startFrom = endIndex;
            result[0] = new TaggedValue<FileInfo> { ID = "AppIcon", Value = new FileInfo(Path.Combine(appRootFolder.FullName, relativePath)), };

            searchKey = "<MauiSplashScreen Include=\"";
            startIndex = csProjContent.IndexOf(searchKey, startFrom) + searchKey.Length;
            endIndex = csProjContent.IndexOf("\"", startIndex);
            length = endIndex - startIndex;
            relativePath = csProjContent.Substring(startIndex, length);
            startFrom = endIndex;
            result[1] = new TaggedValue<FileInfo> { ID = "SplashScreenIcon", Value = new FileInfo(Path.Combine(appRootFolder.FullName, relativePath)), };

            return result;
        }
    }
}
