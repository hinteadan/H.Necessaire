using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using H.Necessaire.CLI.Commands.HDoc.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting
{
    [ID("hdoc-reporter-web")]
    [Alias("hdoc-reporter-static-website")]
    internal class HDocumentationStaticWebSiteReporter : CoderDocsReporterBase<HDocumentation>
    {

    }
}
