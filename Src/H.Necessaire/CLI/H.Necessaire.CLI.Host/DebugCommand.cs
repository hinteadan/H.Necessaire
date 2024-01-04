using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Runtime.Google.FirestoreDB.Core;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        HsFirestoreDebugger debugger;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            debugger = dependencyProvider.Get<HsFirestoreDebugger>();
        }

        public override async Task<OperationResult> Run()
        {
            await debugger.Debug();
            return OperationResult.Win();
        }
    }
}
