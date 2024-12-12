using H.Necessaire.Runtime.CLI.CommandInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Commands
{
    [ID("cli")]
    internal class CommandInterpreterCommand : CommandBase
    {
        const string cliMarker = "(> ";
        static readonly string[] exitCommands = new string[] { "exit", "quit", "bye" };
        readonly CancellationTokenSource commandCancelTokenSource = new CancellationTokenSource();
        static bool isAlreadyInCliMode = false;
        CliConsoleInteractor cliConsoleInteractor;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            cliConsoleInteractor = dependencyProvider.Get<CliConsoleInteractor>();
        }

        public override async Task<OperationResult> Run()
        {
            if (isAlreadyInCliMode)
                return OperationResult.Fail("Already running in CLI mode");

            isAlreadyInCliMode = true;

            Log("Starting Command Interpreter...");
            using (new TimeMeasurement(x => Log($"DONE Starting Command Interpreter in {x}")))
            {
                await Task.Delay(0);

                Console.CancelKeyPress += CancelKeyPress;

                WaitForUserInput(commandCancelTokenSource.Token);

                Log("NOTE: You can always force close via Ctr+C");
            }

            await Console.Out.WriteAsync(cliMarker);

            await KeepAlive();

            return OperationResult.Win();
        }

        private async Task OnUserInput(string userInput)
        {
            if (commandCancelTokenSource.IsCancellationRequested)
                return;

            OperationResult commandResult = await InterpretUserInput(userInput);

            if (commandResult?.IsSuccessful == false)
            {
                await Logger.LogError(string.Join(Environment.NewLine, commandResult.FlattenReasons()));
            }
        }

        private async Task KeepAlive()
        {
            while (!commandCancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(-1, commandCancelTokenSource.Token);
                }
                catch (TaskCanceledException)
                {

                }
            }
        }

        private void WaitForUserInput(CancellationToken cancellationToken)
        {
            Task.Run(
                async () =>
                {
                    string userInput = await cliConsoleInteractor.ProcessConsoleUserInput(cancellationToken);

                    await OnUserInput(userInput);

                    if (!IsExitCommand(userInput))
                        await Console.Out.WriteAsync(cliMarker);

                    WaitForUserInput(cancellationToken);
                },
                cancellationToken
            );
        }

        private async Task<OperationResult> InterpretUserInput(string userInput)
        {
            if (IsExitCommand(userInput))
            {
                commandCancelTokenSource.Cancel();
                return OperationResult.Win();
            }

            if (userInput.IsEmpty())
            {
                return OperationResult.Win();
            }

            return await RunCliCommand(userInput?.Split(" ".AsArray(), StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>());
        }

        private static bool IsExitCommand(string userInput) => IsCommand(userInput, exitCommands);

        private static bool IsCommand(string userInput, params string[] commandsToMatch)
        {
            if (commandsToMatch?.Any() != true)
                return false;

            return userInput.In(commandsToMatch, (input, item) => input.Is(item));
        }

        private void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            commandCancelTokenSource.Cancel();
        }
    }
}