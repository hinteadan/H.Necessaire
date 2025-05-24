using System.Text;

namespace H.Necessaire.Runtime.UI.CLI.BLL
{
    internal class FluentUiGlyphsExporter
    {
        static readonly Encoding contentEncoding = Encoding.UTF8;
        const string defaultNamespace = "H.Necessaire.Runtime.MAUI.WellKnown.FluentUI.Glyphs";
        const string fluentUiGlyphsTemplateName = "FluentUiGlyphs.cs.tmpl.txt";
        const string fluentUiGlyphUnicodeDictionaryEntryTemplateName = "FluentUiGlyphUnicodeDictionaryEntry.cs.tmpl.txt";
        static readonly char[] invalidCSharpChars = ['-'];
        static readonly DataBinFormatInfo cSharpFormat = new DataBinFormatInfo
        {
            ID = $"CSharp{contentEncoding.WebName}",
            Encoding = contentEncoding.WebName,
            Extension = "cs",
            MimeType = "text/plain",
        };

        public async Task<OperationResult<DataBin[]>> ExportToCsharp(Dictionary<string, Dictionary<string, int>> glyphs)
        {
            if (glyphs.IsEmpty())
                return OperationResult.Win().WithPayload(Array.Empty<DataBin>());

            OperationResult<DataBin>[] exportsResults = await Task.WhenAll(glyphs.Select(x => ExportToCsharp(x.Key, x.Value))).ConfigureAwait(false);

            OperationResult exportGlobalResult = exportsResults.Merge();
            if (!exportGlobalResult.IsSuccessful)
                return exportGlobalResult.WithoutPayload<DataBin[]>();

            return
                exportsResults
                    .Select(x => x.Payload)
                    .ToNoNullsArray()
                    .ToWinResult()
                    ;
        }

        Task<OperationResult<DataBin>> ExportToCsharp(string name, Dictionary<string, int> glyphs)
        {
            string processedName = MakeValueCSharpSafe(name);
            DataBinMeta meta = new DataBinMeta
            {
                Name = processedName,
                Format = cSharpFormat,
                Notes = [
                    name.NoteAs("OriginalName"),
                    $"{glyphs.Count}".NoteAs("GlyphCount"),
                ],
            };

            DataBin dataBin = meta.ToBin(x => BuildCSharpExportStream(x, glyphs));

            return dataBin.ToWinResult().AsTask();
        }

        async Task<ImADataBinStream> BuildCSharpExportStream(DataBinMeta meta, Dictionary<string, int> glyphs)
        {
            string csFileTemplate = await fluentUiGlyphsTemplateName.OpenEmbeddedResource().ReadAsStringAsync(isStreamLeftOpen: false);
            string entryTemplate = await fluentUiGlyphUnicodeDictionaryEntryTemplateName.OpenEmbeddedResource().ReadAsStringAsync(isStreamLeftOpen: false);

            string result = csFileTemplate;

            result
                = result
                .ReplaceTag("Namespace", defaultNamespace)
                .ReplaceTag("ClassName", meta.Name)
                .ReplaceTag("GlyphsUnicodeDictionaryEntries", BuildGlyphsUnicodeDictionaryEntries(entryTemplate, glyphs))
                ;

            return result.ToStream(contentEncoding).ToDataBinStream();
        }

        string BuildGlyphsUnicodeDictionaryEntries(string entryTemplate, Dictionary<string, int> glyphs)
        {
            if (glyphs.IsEmpty())
                return null;

            StringBuilder resultBuilder = new StringBuilder();

            foreach (KeyValuePair<string, int> glyph in glyphs)
            {
                resultBuilder
                    .Append(
                        entryTemplate
                        .ReplaceTag("Name", glyph.Key)
                        .ReplaceTag("Unicode", $"char.ConvertFromUtf32({glyph.Value})")
                    )
                    .Append(Environment.NewLine)
                    ;
            }

            resultBuilder.Remove(resultBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return resultBuilder.ToString();
        }

        static string MakeValueCSharpSafe(string value)
        {
            if (value.IsEmpty())
                return value;

            StringBuilder resultBuilder = new StringBuilder(value.Length);

            bool shouldCapitalize = true;
            foreach (char c in value)
            {
                if (c.In(invalidCSharpChars))
                {
                    shouldCapitalize = true;
                    continue;
                }

                resultBuilder.Append(shouldCapitalize ? char.ToUpperInvariant(c) : c);

                shouldCapitalize = false;
            }

            return resultBuilder.ToString();
        }
    }

    static class FluentUiGlyphsExporterExtensions
    {
        const string tagMarkerStart = "(>";
        const string tagMarkerEnd = "<)";
        public static string ReplaceTag(this string value, string tag, string tagValue)
        {
            if (value.IsEmpty())
                return value;

            return value.Replace($"{tagMarkerStart}{tag}{tagMarkerEnd}", tagValue);
        }
    }
}
