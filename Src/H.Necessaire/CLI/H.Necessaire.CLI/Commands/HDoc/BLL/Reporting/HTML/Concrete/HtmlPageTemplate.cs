﻿using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete
{
    internal class HtmlPageTemplate : TemplateBase
    {
        public string PageTitle { get; set; }
        public string AssetsRootPath { get; set; } = "";
    }
}