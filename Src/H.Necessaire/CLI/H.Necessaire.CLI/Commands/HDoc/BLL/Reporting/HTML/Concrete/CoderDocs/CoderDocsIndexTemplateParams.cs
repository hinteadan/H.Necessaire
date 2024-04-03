using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs.Parts;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete.CoderDocs
{
    internal class CoderDocsIndexTemplateParams : HtmlPageTemplateParams
    {
        public PageHeaderPartTemplate PageHeader { get; set; } = new PageHeaderPartTemplate();

        public async Task<string> PageHeaderPart()
        {
            string template
                = await "CoderDocs-BS5-v3.0/Parts/pageheader.part.tmpl.html"
                .OpenEmbeddedResource()
                .ReadAsStringAsync(isStreamLeftOpen: false)
                ;

            return
                await PageHeader.Process(
                    template, 
                    await PageHeader.ReadParams()
                );
        }

        public Task<string> ContentHeaderPart() => "".AsTask();
    }
}
