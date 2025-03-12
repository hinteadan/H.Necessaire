using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.ExternalCommandRunner;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        public override Task<OperationResult> Run() => RunSubCommand();

        class DefaultSubCommand : SubCommandBase
        {
            ImAPeriodicAction periodicAction;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                periodicAction = dependencyProvider.Get<ImAPeriodicAction>();
            }

            public override async Task<OperationResult> Run(params Note[] args)
            {
                periodicAction.Start(TimeSpan.FromSeconds(.5), async () => {
                    await Logger.LogInfo("Period Action's action");
                });

                await Task.Delay(TimeSpan.FromSeconds(2));

                periodicAction.Stop();

                periodicAction.StartDelayed(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(.1), async () => {
                    await Logger.LogInfo("Period Action's action");
                });

                await Task.Delay(TimeSpan.FromSeconds(5));

                periodicAction.Stop();

                return OperationResult.Win();
            }
        }

        class ExternalCommandSubCommand : SubCommandBase
        {
            ExternalCommandRunner externalCommandRunner;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                externalCommandRunner = dependencyProvider.Get<ExternalCommandRunner>();
            }

            public override async Task<OperationResult> Run(params Note[] args)
            {
                var x = await externalCommandRunner
                    .WithContext(new ExternalCommandRunContext { IsOutputCaptured = true, IsOutputPrinted = false, IsMetricsCollectionEnabled = true })
                    .Run("node", "--version");
                string result = x.Payload.OutputData.ToString().Trim();
                var nodeVersion = VersionNumber.Parse(result);

                OperationResult<ExternalCommandRunContext>[] results = await Task.WhenAll(
                    externalCommandRunner.WithContext(new ExternalCommandRunContext { IsOutputCaptured = true, IsMetricsCollectionEnabled = true }).RunCmd("tasklist"),
                    externalCommandRunner.WithContext(new ExternalCommandRunContext { IsOutputCaptured = true, IsMetricsCollectionEnabled = true }).RunCmd("dir")
                );


                return
                    await externalCommandRunner
                    .WithContext(new ExternalCommandRunContext
                    {
                        IsUserInputExpected = true,
                        IsMetricsCollectionEnabled = true,
                        UserInputProvider = () => new string[] { "ping google.com", "exit" }.AsTask(),
                    })
                    .RunCmd();
            }
        }

        class UiSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                DateTime.Now.CliUiPrintCalendar(events: [new DateTime(2024, 12, 25)]);

                Log("ALL Done");

                return OperationResult.Win();
            }
        }
    }
}
