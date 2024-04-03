using H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Concrete
{
    internal class TemplateParam : TemplateParamBase
    {
        readonly Func<Task<string>> valueLoader;
        public TemplateParam(string id, Func<Task<string>> valueLoader) : base(id)
        {
            this.valueLoader = valueLoader;
        }

        protected override async Task<string> LoadValue() => await valueLoader();
    }
}
