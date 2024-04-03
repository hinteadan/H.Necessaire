using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal interface ImATemplateParamsBin
    {
        Task<IEnumerable<ImATemplateParam>> ReadParams();
    }
}
