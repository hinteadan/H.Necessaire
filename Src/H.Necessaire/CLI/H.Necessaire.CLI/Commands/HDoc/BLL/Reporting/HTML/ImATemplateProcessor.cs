using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal interface ImATemplateProcessor
    {
        Task<string> Process(string template, IEnumerable<ImATemplateParam> templateParams);
    }
}
