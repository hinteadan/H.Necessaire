using H.Necessaire.CLI.Commands;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public abstract class CommandBase : UseCaseBase, ImACliCommand
    {
        protected async Task<Note[]> GetArguments()
        {
            return (await GetCurrentContext() ?? new UseCaseContext()).Notes;
        }

        public abstract Task<OperationResult> Run();
    }
}
