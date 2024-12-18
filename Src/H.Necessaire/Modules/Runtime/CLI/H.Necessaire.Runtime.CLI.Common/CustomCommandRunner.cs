using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Common
{
    internal class CustomCommandRunner : ImADependency
    {
        CliCommandFactory commandRunner;
        ArgsParser argsParser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            commandRunner = dependencyProvider.Get<CliCommandFactory>();
            argsParser = dependencyProvider.Get<ArgsParser>();
        }

        public async Task<OperationResult> RunCliCommand(params Note[] args)
        {
            using (CustomizableCliContextProvider.WithArgs(args))
            {
                return await commandRunner.Run();
            }
        }

        public async Task<OperationResult> RunCliCommand(params string[] args)
        {
            using (CustomizableCliContextProvider.WithArgs(await argsParser.Parse(args)))
            {
                return await commandRunner.Run();
            }
        }
    }
}
