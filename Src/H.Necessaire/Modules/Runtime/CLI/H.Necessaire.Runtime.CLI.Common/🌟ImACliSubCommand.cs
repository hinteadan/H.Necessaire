using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    public interface ImACliSubCommand
    {
        Task<OperationResult> Run(params Note[] args);
    }
}
