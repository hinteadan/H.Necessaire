using H.Necessaire.CLI.Commands;
using H.Necessaire.Runtime.CLI.UI;
using H.Necessaire.Runtime.ExternalCommandRunner;
using H.Necessaire.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Host
{
    internal class DebugCommand : CommandBase
    {
        public override Task<OperationResult> Run() => RunSubCommand();

        class DefaultSubCommand : SubCommandBase
        {
            public override Task<OperationResult> Run(params Note[] args)
            {
                PartialPeriodOfTime january = (new PartialDateTime { Month = 1 }, new PartialDateTime { Month = 1 });

                PartialPeriodOfTime monthlyReportingPerod = (new PartialDateTime { DayOfMonth = 1 }, new PartialDateTime { DayOfMonth = 25 });

                PartialDateTime christmasDay = new PartialDateTime { DayOfMonth = 25, Month = 12 };

                ApproximatePeriodOfTime approximatePeriodOfTime
                    = (
                        (PartialPeriodOfTime)(new DateTime(2010, 1, 1), new DateTime(2010, 6, 1)),
                        (PartialPeriodOfTime)(new DateTime(2025, 1, 1), new DateTime(2025, 6, 1))
                    );

                Console.WriteLine($"Christmas in 2050 will be on a {christmasDay.ToDateTime(fallbackYear: 2050).PrintDayOfWeek()}");


                PeriodOfTime thisYear = new PeriodOfTime
                {
                    From = new DateTime(year: DateTime.UtcNow.Year, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, microsecond: 0, DateTimeKind.Utc),
                    To = new DateTime(year: DateTime.UtcNow.Year, month: 12, day: 31, hour: 23, minute: 59, second: 59, millisecond: 999, microsecond: 999, DateTimeKind.Utc),
                };

                PeriodOfTime fromNowOn = new PeriodOfTime { From = DateTime.UtcNow };

                return OperationResult.Win().AsTask();
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
                DateTime.Now.CliUiPrintCalendar(events: [new DateTime(2024, 12 ,25)]);

                Log("ALL Done");

                return OperationResult.Win();
            }
        }
    }
}
