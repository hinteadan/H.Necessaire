﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class CoderDocsReporterBase<T> : StaticWebSiteReporterBase<T>
    {
        const string templateRootPath = "Commands/HDoc/BLL/Reporting/HTML/Templates/CoderDocs-BS5-v3.0/";
        const string templateAssetsPath = "Commands/HDoc/BLL/Reporting/HTML/Templates/CoderDocs-BS5-v3.0/assets/";

        protected override async Task<IEnumerable<Task<TaggedStream>>> GetContentStreams()
        {
            IEnumerable<Task<TaggedStream>> assets = await BuildAssetsStreams();
            IEnumerable<Task<TaggedStream>> pages = await BuildPagesStreams();


            return 
                assets
                .Union(pages)
                .Union(Enumerable.Repeat(true, 1).Select(_ => BuildIndexStream()))
                ;
        }

        protected abstract Task<TaggedStream> BuildIndexStream();
        protected abstract Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams();

        protected virtual Task<IEnumerable<Task<TaggedStream>>> BuildAssetsStreams()
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
