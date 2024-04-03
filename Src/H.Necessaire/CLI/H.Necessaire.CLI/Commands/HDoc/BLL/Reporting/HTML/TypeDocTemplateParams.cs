using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal class TypeDocTemplateParams : TemplateParamsBinBase
    {
        public string PageTitle { get; set; }
        public string ContentTitle { get; set; }
    }
}
