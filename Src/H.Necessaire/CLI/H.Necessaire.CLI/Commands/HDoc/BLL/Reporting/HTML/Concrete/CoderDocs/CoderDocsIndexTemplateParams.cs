using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs.Parts;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs
{
    internal class CoderDocsIndexTemplateParams : HtmlPageTemplateParams
    {
        public PageHeaderPartTemplate PageHeader { get; set; } = new PageHeaderPartTemplate();
        public ContentHeaderPartTemplate ContentHeader { get; set; } = new ContentHeaderPartTemplate();

        public async Task<string> PageHeaderPart()
            => await ProcessPartFromEmbeddedResource(
                "CoderDocs-BS5-v3.0/Parts/pageheader.part.tmpl.html",
                PageHeader
            );

        public async Task<string> ContentHeaderPart()
            => await ProcessPartFromEmbeddedResource(
                "CoderDocs-BS5-v3.0/Parts/contentheader.part.tmpl.html",
                ContentHeader
            );
    }
}
