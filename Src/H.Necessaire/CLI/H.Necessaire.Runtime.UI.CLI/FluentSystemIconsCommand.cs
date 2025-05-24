using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.UI.CLI.BLL;
using System.Text;

namespace H.Necessaire.Runtime.UI.CLI
{
    [ID("fluent-ui")]
    internal class FluentSystemIconsCommand : CommandBase
    {
        static readonly string[] usageSyntaxes = [
            "fluent-ui index-icons"
        ];
        protected override string[] GetUsageSyntaxes() => usageSyntaxes;

        public override Task<OperationResult> Run() => RunSubCommand();

        [ID("index-icons")]
        class IndexIconsSubCommand : SubCommandBase
        {
            #region Construct
            FluentUiRepoFinder fluentUiRepoFinder;
            FluentUiGlyphsParser fluentUiGlyphsParser;
            FluentUiGlyphsExporter fluentUiGlyphsExporter;
            HNecessaireMauiRepoFinder hNecessaireMauiRepoFinder;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                fluentUiRepoFinder = dependencyProvider.Get<FluentUiRepoFinder>();
                fluentUiGlyphsParser = dependencyProvider.Get<FluentUiGlyphsParser>();
                fluentUiGlyphsExporter = dependencyProvider.Get<FluentUiGlyphsExporter>();
                hNecessaireMauiRepoFinder = dependencyProvider.Get<HNecessaireMauiRepoFinder>();
            }
            #endregion
            public override async Task<OperationResult> Run(params Note[] args)
            {
                OperationResult<DirectoryInfo> fluentUiDirectoryFindResult = fluentUiRepoFinder.Find();
                if (!fluentUiDirectoryFindResult.IsSuccessful)
                    return fluentUiDirectoryFindResult;

                DirectoryInfo fluentUiDirectory = fluentUiDirectoryFindResult.Payload;

                OperationResult<Dictionary<string, Dictionary<string, int>>> glyphsParseResult = await fluentUiGlyphsParser.Parse(fluentUiDirectory);

                if (!glyphsParseResult.IsSuccessful)
                    return glyphsParseResult;

                Dictionary<string, Dictionary<string, int>> glyphs = glyphsParseResult.Payload;

                OperationResult<DataBin[]> csharpExportResults = await fluentUiGlyphsExporter.ExportToCsharp(glyphs);

                if (!csharpExportResults.IsSuccessful)
                    return csharpExportResults;

                DataBin[] csharpExports = csharpExportResults.Payload;

                OperationResult<DirectoryInfo> outFolderResult = hNecessaireMauiRepoFinder.FindWellKnownFluentUiGlyphsFolder();

                if (!outFolderResult.IsSuccessful)
                    return outFolderResult;

                DirectoryInfo outFolder = outFolderResult.Payload;

                foreach (DataBin csharpExport in csharpExports)
                {
                    using (ImADataBinStream dataStream = await csharpExport.OpenDataBinStream())
                    {
                        FileInfo csFile = new FileInfo(Path.Combine(outFolder.FullName, $"{csharpExport.Name.ToSafeFileName()}.{csharpExport.Format.Extension}"));

                        await File.WriteAllTextAsync(csFile.FullName, await dataStream.DataStream.ReadAsStringAsync(), Encoding.GetEncoding(csharpExport.Format.Encoding));
                    }
                }

                return OperationResult.Win();
            }
        }
    }
}
