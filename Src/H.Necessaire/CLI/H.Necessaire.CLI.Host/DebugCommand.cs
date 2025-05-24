using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.ExternalCommandRunner;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        public override Task<OperationResult> Run() => RunSubCommand();

        class DefaultSubCommand : SubCommandBase
        {
            AsyncEventRaiser<EventArgs> demoEvent;
            public override void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                base.ReferDependencies(dependencyProvider);
                demoEvent = new AsyncEventRaiser<EventArgs>(this);
            }

            event AsyncEventHandler<EventArgs> OnDemo { add => demoEvent.OnEvent += value; remove => demoEvent.OnEvent -= value; }

            public override async Task<OperationResult> Run(params Note[] args)
            {
                OnDemo += async (sender, args) => { await Task.Delay(2000); Log($"Handler 1 event from {sender.TypeName() ?? "[Unknown]"}"); };

                OnDemo += async (sender, args) => { await Task.Delay(3000); Log($"Handler 2 event from {sender.TypeName() ?? "[Unknown]"}"); };

                await demoEvent.Raise(EventArgs.Empty);

                return OperationResult.Win();
            }
        }

        [ID("tamers")]
        class ExecutionTamersDebugSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                var semaphore = new SemaphoreSlim(0, 1);

                Throttler throttler = new Throttler(async () => {
                    await Logger.LogInfo("Throttled action");
                }, TimeSpan.FromSeconds(1));

                new Thread(async () => {

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
            public override async Task<OperationResult> Run(params Note[] args)
            {
                DateTime.Now.CliUiPrintCalendar(events: [new DateTime(2024, 12, 25)]);

                Log("ALL Done");

                return OperationResult.Win();
            }
        }
    }
}
