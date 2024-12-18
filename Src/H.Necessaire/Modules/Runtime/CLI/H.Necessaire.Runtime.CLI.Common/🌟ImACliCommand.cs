using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands
{
    public interface ImACliCommand
    {
        Task<OperationResult> Run();
    }
}
