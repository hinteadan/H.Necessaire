using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class TemplateParamBase : ImATemplateParam
    {
        protected TemplateParamBase(string id)
        {
            this.ID = id;
        }
        protected abstract Task<string> LoadValue();

        public string ID { get; }
        public async Task<string> Read() => await LoadValue();
    }
}
