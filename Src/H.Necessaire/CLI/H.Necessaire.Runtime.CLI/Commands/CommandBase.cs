﻿using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.Builders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    public abstract class CommandBase : UseCaseBase, ImACliCommand
    {
        Func<string, ImACliSubCommand> subCommandFinder;
        CustomCommandRunner customCommandRunner;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            subCommandFinder = subCommandFinder ?? (name => dependencyProvider.Build<ImACliSubCommand>(name));
            customCommandRunner = dependencyProvider.Get<CustomCommandRunner>();
        }

        protected async Task<Note[]> GetArguments()
        {
            return (await GetCurrentContext() ?? new UseCaseContext()).Notes;
        }

        public abstract Task<OperationResult> Run();

        protected async Task<OperationResult> RunCliCommand(params Note[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected async Task<OperationResult> RunCliCommand(params string[] args)
            => await customCommandRunner.RunCliCommand(args);

        protected virtual async Task<OperationResult> RunSubCommand(bool failOnMissingCommand = false)
        {
            Note[] args = (await GetArguments())?.Jump(1);

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
