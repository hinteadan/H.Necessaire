namespace H.Necessaire.CLI.Commands
{
    public interface ImACliCommand
    {
        Task<OperationResult> Run();
    }
}
