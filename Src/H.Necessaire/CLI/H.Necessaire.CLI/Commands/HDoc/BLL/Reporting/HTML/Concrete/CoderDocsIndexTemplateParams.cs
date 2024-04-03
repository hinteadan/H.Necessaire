using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete
{
    internal class CoderDocsIndexTemplateParams : HtmlPageTemplateParams
    {
        public Task<string> PageHeaderPart() => "".AsTask();
        public Task<string> ContentHeaderPart() => "".AsTask();
    }
}
