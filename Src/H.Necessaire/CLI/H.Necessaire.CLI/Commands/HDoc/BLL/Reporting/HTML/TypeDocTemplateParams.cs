using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal class TypeDocTemplateParams : HtmlPageTemplate
    {
        public string ContentTitle { get; set; }
    }
}
