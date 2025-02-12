using H.Necessaire.Serialization;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class FluentUiGlyphsParser
    {
        public async Task<OperationResult<Dictionary<string, Dictionary<string, int>>>> Parse(DirectoryInfo fluentUiRepoDirectory)
        {
            if (fluentUiRepoDirectory?.Exists != true)
                return OperationResult.Fail("Fluent UI Repo directory does not exist.").WithoutPayload<Dictionary<string, Dictionary<string, int>>>();

            DirectoryInfo fontsDirectory = new DirectoryInfo(Path.Combine(fluentUiRepoDirectory.FullName, "fonts"));

            if (!fontsDirectory.Exists)
                return OperationResult.Fail($"Fonts directory not found within the FluentUI Repo folder {fluentUiRepoDirectory.FullName}.").WithoutPayload<Dictionary<string, Dictionary<string, int>>>();

            FileInfo[] glyphsJsons = fontsDirectory.GetFiles("*.json", SearchOption.AllDirectories);

            Dictionary<string, Dictionary<string, int>> results = new();

            foreach (FileInfo glyphJsonFile in glyphsJsons)
            {
                string id = glyphJsonFile.Name.Replace(glyphJsonFile.Extension, "");

                string jsonString = await File.ReadAllTextAsync(glyphJsonFile.FullName);

                OperationResult<Dictionary<string, int>> glyphsResult = jsonString.TryJsonToObject<Dictionary<string, int>>();

                if (!glyphsResult.IsSuccessful)
                    return glyphsResult.WithoutPayload<Dictionary<string, Dictionary<string, int>>>();

                Dictionary<string, int> glyphs = glyphsResult.Payload;

                results.Add(id, glyphs);
            }

            return results.ToWinResult();
        }
    }
}
