using H.Necessaire.CLI.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public abstract class SubCommandBase : UseCaseBase, ImACliSubCommand
    {
        Func<string, ImACliSubCommand> subCommandFinder;
        CustomCommandRunner customCommandRunner;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            subCommandFinder = subCommandFinder ?? (name => dependencyProvider.Build<ImACliSubCommand>(name));
            customCommandRunner = dependencyProvider.Get<CustomCommandRunner>();
        }

        public abstract Task<OperationResult> Run(params Note[] args);

        protected async Task<OperationResult> RunCliCommand(params Note[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected async Task<OperationResult> RunCliCommand(params string[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected async Task<Note[]> GetAllArguments()
        {
            return (await GetCurrentContext() ?? new UseCaseContext()).Notes;
        }

        protected virtual async Task<OperationResult> RunSubCommand(Note[] currentArgs, bool failOnMissingCommand = false)
        {
            Note[] args = currentArgs?.Jump(1);

            if (args?.Any() != true)
            {
                return failOnMissingCommand ? OperationResult.Fail("No Args") : WinWithUsageSyntax();
            }

            ImACliSubCommand subCommand = subCommandFinder.Invoke(args.First().ID);

            if (subCommand is null)
            {
                return failOnMissingCommand ? OperationResult.Fail("No Matching Sub-Command") : WinWithUsageSyntax();
            }

            return await subCommand.Run(args.Jump(1));
        }

        protected virtual string[] GetUsageSyntaxes() => new string[0];

        protected virtual string PrintUsageSyntax()
        {
            return CLIPrinter.PrintUsageSyntax(GetUsageSyntaxes());
        }

        protected virtual OperationResult FailWithUsageSyntax()
        {
            return OperationResult.Fail(PrintUsageSyntax());
        }

        protected virtual OperationResult WinWithUsageSyntax(string reason = null)
        {
            Log($"{reason}{Environment.NewLine}{Environment.NewLine}{PrintUsageSyntax()}");
            return OperationResult.Win();
        }

        protected virtual string Log(string message)
        {
            return CLIPrinter.PrintLog(message);
        }
    }
}
