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

        protected virtual string[] GetUsageSyntaxes() => new string[0];

        protected virtual string PrintUsageSyntax()
        {
            return CLIPrinter.PrintUsageSyntax(GetUsageSyntaxes());
        }

        protected OperationResult FailWithUsageSyntax()
        {
            return OperationResult.Fail(PrintUsageSyntax());
        }

        protected string Log(string message)
        {
            return CLIPrinter.PrintLog(message);
        }
    }
}
