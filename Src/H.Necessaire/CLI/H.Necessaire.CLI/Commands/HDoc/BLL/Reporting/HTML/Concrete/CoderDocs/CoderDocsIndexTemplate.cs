using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs.Parts;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs
{
    internal class CoderDocsIndexTemplate : HtmlPageTemplate
    {
        public PageHeaderPartTemplate PageHeader { get; set; } = new PageHeaderPartTemplate();
        public ContentHeaderPartTemplate ContentHeader { get; set; } = new ContentHeaderPartTemplate();

        public async Task<string> PageHeaderPart()
            => await PageHeader.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/pageheader.part.tmpl.html");

        public async Task<string> ContentHeaderPart()
            => await ContentHeader.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/contentheader.part.tmpl.html");
    }
}
