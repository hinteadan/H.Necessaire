using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete
{
    internal sealed class StaticTemplateParam : TemplateParamBase
    {
        readonly string value;
        public StaticTemplateParam(string id, string value) : base(id)
        {
            this.value = value;
        }

        protected override Task<string> LoadValue() => value.AsTask();
    }
}
