using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML
{
    internal interface ImATemplateParam : IStringIdentity
    {
        Task<string> Read();
    }
}
