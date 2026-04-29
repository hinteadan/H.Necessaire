using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.ExternalCommandRunner;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        public override Task<OperationResult> Run() => RunSubCommand();

        class DefaultSubCommand : SubCommandBase
        {
            ImALogger log;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);

                log = dependencyProvider.GetLogger<DefaultSubCommand>();
            }

            public override async Task<OperationResult> Run(params Note[] args)
            {
                int threadCount = 7;

                await Task.WhenAll(Enumerable.Range(1, threadCount).Select(i => log.LogInfo($"Log Info {i}")));

                return true;
            }
        }

        [ID("tamers")]
        class ExecutionTamersDebugSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                var semaphore = new SemaphoreSlim(0, 1);

                Throttler throttler = new Throttler(async () =>
                {
                    await Logger.LogInfo("Throttled action");
                }, TimeSpan.FromSeconds(1));

                new Thread(async () =>
                {

                    var start = Stopwatch.GetTimestamp();
                    while (Stopwatch.GetElapsedTime(start) < TimeSpan.FromSeconds(5))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(.1));
                        await throttler.Invoke();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1.2));

                    semaphore.Release();

                }).Start();

                await semaphore.WaitAsync();

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
            public override Task<OperationResult> Run(params Note[] args)
            {
                DateTime.Now.CliUiPrintCalendar(events: [new DateTime(2024, 12, 25)]);

                Log("ALL Done");

                return OperationResult.Win().AsTask();
            }
        }
    }
}
