using DeviceDetectorNET;
using H.Necessaire.CLI.Commands.HDoc;
using H.Necessaire.CLI.Commands.HDoc.Model;
using H.Necessaire.Runtime.CLI.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    [ID("h-doc")]
    public class HDocCommand : CommandBase
    {
        HDocManager hDocManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hDocManager = dependencyProvider.Get<HDocManager>();
        }

        public override async Task<OperationResult> Run()
        {
            OperationResult<HDocumentation> docResult = await hDocManager.ParseDocumentation();
            if (!docResult.IsSuccessful)
                return docResult;
            HDocumentation documentation = docResult.Payload;

            var x = docResult.Payload.AllTypes.Where(t => t.Methods?.Any(m => m.IsVirtual) == true).ToArray();

            return OperationResult.Win();
        }
    }
}
