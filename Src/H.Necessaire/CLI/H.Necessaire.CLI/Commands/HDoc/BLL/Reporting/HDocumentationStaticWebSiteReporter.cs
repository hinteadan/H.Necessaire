using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML;
using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs;
using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs.Parts;
using H.Necessaire.CLI.Commands.HDoc.Model;
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
        protected override async Task<Stream> BuildIndexContentStream(HDocumentation reportData)
        {
            CoderDocsIndexTemplate template = new CoderDocsIndexTemplate { 
                PageTitle = $"H.Necessaire v{reportData.Version.Number}",
                PageHeader = new PageHeaderPartTemplate
                {
                    
                },
                ContentHeader = new ContentHeaderPartTemplate { 

                },
                ContentCards = reportData.AllTypes.GroupBy(t => t.Module).Select(g => new ContentCardPartTemplate { 
                    Title = g.Key.Replace(".", " ."),
                    Description = $"<ul><li><strong>{g.Count()}</strong> types</li></ul>",
                }).ToArray(),
                ContentFooter = new ContentFooterPartTemplate {

                },
                PageFooter = new PageFooterPartTemplate { 

                },
            };

            return
                (await template.ProcessEmbeddedResource($"{templateRootPath}index.html"))
                .ToStream();
        }

        protected override Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams(HDocumentation reportData)
        {
            return 
                Enumerable.Repeat(true, 1).Select(_ =>
                    new TaggedStream { 
                        ID = "docs-page.html",
                        Name = "docs-page.html",
                        Value = $"{templateRootPath}docs-page.html".OpenEmbeddedResource()
                    }.AsTask()
                ).AsTask();
        }
    }
}
