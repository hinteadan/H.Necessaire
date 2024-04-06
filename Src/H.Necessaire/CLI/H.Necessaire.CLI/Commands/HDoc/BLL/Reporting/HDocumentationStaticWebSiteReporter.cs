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
            CoderDocsIndexTemplate template = new CoderDocsIndexTemplate
            {
                PageTitle = $"H.Necessaire v{reportData.Version.Number}",
                PageHeader = new PageHeaderPartTemplate
                {

                },
                ContentHeader = new ContentHeaderPartTemplate
                {

                },
                ContentCards = reportData.AllTypes.GroupBy(t => t.Module).Select(g => new ContentCardPartTemplate
                {
                    Title = g.Key.Replace(".", " ."),
                    Description = $"<ul><li><strong>{g.Count()}</strong> types</li></ul>",
                }).ToArray(),
                ContentFooter = new ContentFooterPartTemplate
                {

                },
                PageFooter = new PageFooterPartTemplate
                {

                },
            };

            return
                (await template.ProcessEmbeddedResource($"{templateRootPath}index.html"))
                .ToStream();
        }

        protected override async Task<IEnumerable<Task<TaggedStream>>> BuildPagesStreams(HDocumentation reportData)
        {
            IEnumerable<IGrouping<string, HDocTypeInfo>> modules = reportData.AllTypes.GroupBy(t => t.Module);

            IEnumerable<Task<TaggedStream>> result = Enumerable.Empty<Task<TaggedStream>>();

            foreach (IGrouping<string, HDocTypeInfo> moduleGroup in modules)
            {
                result = result.Concat(await BuildModulePagesStreams(moduleGroup));
            }

            return result;
        }

        private Task<IEnumerable<Task<TaggedStream>>> BuildModulePagesStreams(IEnumerable<HDocTypeInfo> hDocTypes)
        {
            return
                hDocTypes
                .Select(BuildDocTypeStream)
                .AsTask()
                ;
        }

        private async Task<TaggedStream> BuildDocTypeStream(HDocTypeInfo hDocTypeInfo)
        {
            CoderDocsPageTemplate template = new CoderDocsPageTemplate
            {
                PageTitle = $"{hDocTypeInfo.Module} - {hDocTypeInfo.Name}",
                AssetsRootPath = "../"
            };

            return new TaggedStream
            {
                ID = $"{hDocTypeInfo.Module}/{hDocTypeInfo.Name.ToSafeFileName()}.html",
                Name = hDocTypeInfo.Name,
                Value = (await template.ProcessEmbeddedResource($"{templateRootPath}docs-page.html")).ToStream()
            };
        }
    }
}
