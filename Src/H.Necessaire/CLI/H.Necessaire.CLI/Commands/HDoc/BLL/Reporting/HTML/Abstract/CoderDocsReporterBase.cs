using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class CoderDocsReporterBase<T> : StaticWebSiteReporterBase<T>
    {
        const string prefix = "H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Templates.CoderDocs_BS5_v3._0.assets.";

        protected override async Task<IEnumerable<Task<TaggedStream>>> GetContentStreams()
        {
            await Task.CompletedTask;

            IEnumerable<string> assetResourceNames
                = GetType()
                .Assembly
                .GetManifestResourceNames()
                .Where(x => x.Contains("Templates.CoderDocs_BS5_v3._0.assets"))
                ;

            IEnumerable<Task<TaggedStream>> streams
                = assetResourceNames
                .Select(assetResourceName => new TaggedStream
                {
                    ID = assetResourceName.Substring(prefix.Length).Replace(".", "/"),
                    Value = assetResourceName.OpenEmbeddedResource()
                }.AsTask())
                ;

            return streams;
        }
    }
}
