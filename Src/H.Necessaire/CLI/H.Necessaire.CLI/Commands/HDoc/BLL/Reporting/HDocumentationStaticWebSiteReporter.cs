using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using H.Necessaire.CLI.Commands.HDoc.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting
{
    [ID("hdoc-reporter-web")]
    [Alias("hdoc-reporter-static-website")]
    internal class HDocumentationStaticWebSiteReporter : CoderDocsReporterBase<HDocumentation>
    {
        protected override Task<Stream> BuildIndexContentStream()
        {
            return "Index Page".ToStream().AsTask();
        }

        protected override Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams()
        {
            return Enumerable.Empty<Task<TaggedStream>>().AsTask();
        }
    }
}
