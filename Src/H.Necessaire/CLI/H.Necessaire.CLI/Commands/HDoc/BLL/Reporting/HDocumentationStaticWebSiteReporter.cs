using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using H.Necessaire.CLI.Commands.HDoc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting
{
    [ID("hdoc-reporter-web")]
    [Alias("hdoc-reporter-static-website")]
    internal class HDocumentationStaticWebSiteReporter : CoderDocsReporterBase<HDocumentation>
    {
        protected override Task<TaggedStream> BuildIndexStream()
        {
            return
                new TaggedStream { 
                    ID = "index.html",
                    Name = "index.html",
                    Value = "Index Page".ToStream(),
                }
                .AsTask()
                ;
        }

        protected override Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams()
        {
            return Enumerable.Empty<Task<TaggedStream>>().AsTask();
        }
    }
}
