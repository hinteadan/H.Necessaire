using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs.Parts;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs
{
    internal class CoderDocsIndexTemplate : HtmlPageTemplate
    {
        public PageHeaderPartTemplate PageHeader { get; set; } = new PageHeaderPartTemplate();
        public ContentHeaderPartTemplate ContentHeader { get; set; } = new ContentHeaderPartTemplate();
        public ContentCardPartTemplate[] ContentCards { get; set; } = Array.Empty<ContentCardPartTemplate>();
        public ContentFooterPartTemplate ContentFooter { get; set; } = new ContentFooterPartTemplate();
        public PageFooterPartTemplate PageFooter { get; set; } = new PageFooterPartTemplate();

        public async Task<string> PageHeaderPart()
            => await PageHeader.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/Index/pageheader.part.tmpl.html");

        public async Task<string> ContentHeaderPart()
            => await ContentHeader.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/Index/contentheader.part.tmpl.html");

        public async Task<string> ContentCardsPart()
            => await ContentCards.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/Index/contentcard.part.tmpl.html");

        public async Task<string> ContentFooterPart()
            => await ContentFooter.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/Index/contentfooter.part.tmpl.html");

        public async Task<string> PageFooterPart()
            => await ContentFooter.ProcessEmbeddedResource("CoderDocs-BS5-v3.0/Parts/Index/pagefooter.part.tmpl.html");
    }
}
