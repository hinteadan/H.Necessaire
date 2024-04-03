using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class CoderDocsReporterBase<T> : StaticWebSiteReporterBase<T>
    {
        protected const string templateRootPath = "Commands/HDoc/BLL/Reporting/HTML/Templates/CoderDocs-BS5-v3.0/";
        protected const string templateAssetsPath = "Commands/HDoc/BLL/Reporting/HTML/Templates/CoderDocs-BS5-v3.0/assets/";
        protected const string templatePartsPath = "Commands/HDoc/BLL/Reporting/HTML/Templates/CoderDocs-BS5-v3.0/Parts/";

        protected override async Task<IEnumerable<Task<TaggedStream>>> GetContentStreams(T reportData)
        {
            IEnumerable<Task<TaggedStream>> assets = await BuildAssetsStreams(reportData);
            IEnumerable<Task<TaggedStream>> pages = await BuildPagesStreams(reportData);


            return 
                assets
                .Union(pages)
                .Union(Enumerable.Repeat(true, 1).Select(_ => BuildIndexStream(reportData)))
                ;
        }
        protected abstract Task<Stream> BuildIndexContentStream(T reportData);
        protected abstract Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams(T reportData);

        protected virtual async Task<TaggedStream> BuildIndexStream(T reportData)
        {
            return
                new TaggedStream
                {
                    ID = "index.html",
                    Name = "index.html",
                    Value = await BuildIndexContentStream(reportData),
                }
                ;
        }

        protected virtual Task<IEnumerable<Task<TaggedStream>>> BuildAssetsStreams(T reportData)
        {
            IEnumerable<string> assetResourceNames
                = GetType()
                .Assembly
                .GetManifestResourceNames()
                .Where(x => x.Contains(templateAssetsPath))
                ;

            IEnumerable<Task<TaggedStream>> streams
                = assetResourceNames
                .Select(assetResourceName => new TaggedStream
                {
                    ID = assetResourceName.Substring(templateRootPath.Length),
                    Name = assetResourceName.Substring(assetResourceName.LastIndexOf('/') + 1),
                    Value = assetResourceName.OpenEmbeddedResource()
                }.AsTask())
                ;

            return streams.AsTask();
        }
    }
}
