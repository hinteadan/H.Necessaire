using H.Necessaire.Runtime;
using H.Necessaire.Runtime.CLI.Commands;
using H.Necessaire.Runtime.Google.FirestoreDB.Core;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
        }

        public override async Task<OperationResult> Run()
        {
            Note[] info = Note.GetEnvironmentInfo().AppendProcessInfo();
            return OperationResult.Win();
        }
    }
}
