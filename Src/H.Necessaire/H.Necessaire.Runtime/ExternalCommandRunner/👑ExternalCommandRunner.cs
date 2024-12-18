using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.ExternalCommandRunner
{
    public sealed class ExternalCommandRunner : ImAnExternalCommandRunner, ImAContextualExternalCommandRunnerFactory, ImADependency
    {
        #region Construct
        static readonly TimeSpan minimumMetricsCollectionInterval = TimeSpan.FromSeconds(5);
        ArgsBuilder argsBuilder = new ArgsBuilder();
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            argsBuilder = dependencyProvider.Get<ArgsBuilder>();
        }
        #endregion

        /// <summary>
        /// Run a cmd.exe command
        /// Equivalent of Run("cmd.exe", "/c", "...")
        /// </summary>
        /// <param name="args">Args for cmd.exe /c</param>
        /// <returns>Operation Result</returns>
        public async Task<OperationResult<ExternalCommandRunContext>> RunCmd(params Note[] args)
        {
            ExternalCommandRunContext context = ExternalCommandRunContext.GetCurrent();
            Note[] actualArgs = new Note[2 + (args?.Length ?? 0)];
            actualArgs[0] = "cmd.exe";
            actualArgs[1] = context?.IsUserInputExpected == true ? null : "/c";
            for (int i = 0; i < (args?.Length ?? 0); i++)
            {
                actualArgs[i + 2] = args[i];
            }

            return await Run(actualArgs);
        }

        public async Task<OperationResult<ExternalCommandRunContext>> Run(params Note[] args)
        {
            string command = argsBuilder.BuildInline(args?.FirstOrDefault() ?? default);
            if (command.IsEmpty())
            {
                return OperationResult.Fail("Missing external command to run, as first argument").WithoutPayload<ExternalCommandRunContext>();
            }

            OperationResult<ExternalCommandRunContext> result = OperationResult.Fail("Not yet started").WithoutPayload<ExternalCommandRunContext>();

            ExternalCommandRunContext context = ExternalCommandRunContext.GetCurrent() ?? new ExternalCommandRunContext { };

            await new Func<Task>(async () =>
            {
                await Task.Run(() =>
                {
                    Process externalProcess = BuildProcess(context, command, args.Jump(1));

                    StartProcess(context, externalProcess);

                    CancellationTokenSource userInputCancelTokenSource = context.CancellationTokenSource;
                    Task userInputTask = context.IsUserInputExpected ? BuildUserInputMonitoringTask(context, externalProcess, userInputCancelTokenSource) : null;

                    externalProcess.WaitForExit();

                    if (context.IsUserInputExpected)
                    {
                        userInputCancelTokenSource.Cancel();
                        new Action(userInputTask.Dispose).TryOrFailWithGrace();
                        userInputTask = null;
                    }

                    DisposeProcess(context, externalProcess);

                    result = OperationResult.Win().WithPayload(context);

                }, context.CancellationTokenSource.Token);
            })
            .TryOrFailWithGrace(
                onFail: ex => result = OperationResult.Fail(ex, $"Error occurred while running external process. Message: {ex.Message}").WithoutPayload<ExternalCommandRunContext>()
            );

            return result;
        }

        public ImAContextualExternalCommandRunner WithContext(ExternalCommandRunContext context)
        {
            return new ContextualExternalCommandRunner(this, context);
        }

        public ImAContextualExternalCommandRunner WithContext(bool isOutputPrinted = true, bool isOutputCaptured = false, bool isUserInputExpected = false)
        {
            return WithContext(new ExternalCommandRunContext
            {
                IsOutputPrinted = isOutputPrinted,
                IsOutputCaptured = isOutputCaptured,
                IsUserInputExpected = isUserInputExpected,
            });
        }

        private static Task BuildUserInputMonitoringTask(ExternalCommandRunContext context, Process externalProcess, CancellationTokenSource cancelTokenSource)
        {
            return Task.Run(async () =>
            {
                if (context.UserInputProvider != null)
                {
                    string[] userInputs = await context.UserInputProvider.Invoke();

                    foreach (string userInput in userInputs)
                    {
                        await externalProcess.StandardInput.WriteLineAsync(userInput);
                        CollectMetricsIfNecessary(context, externalProcess);
                    }

                    return;
                }

                while (!cancelTokenSource.IsCancellationRequested)
                {
                    string userInput = await Task.Run(async () => await Console.In.ReadLineAsync(), cancelTokenSource.Token);

                    Console.WriteLine();

                    await externalProcess.StandardInput.WriteLineAsync(userInput);
                    CollectMetricsIfNecessary(context, externalProcess);
                }
            });
        }

        static void OnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs, ExternalCommandRunContext context)
        {
            if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
                return;

            if (context.IsOutputCaptured)
            {
                context.OutputData.AppendLine(dataReceivedEventArgs.Data);
            }

            if (context.IsOutputPrinted)
            {
                Console.WriteLine(dataReceivedEventArgs.Data);
            }
        }

        static void OnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs, ExternalCommandRunContext context)
        {
            if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
                return;

            if (context.IsOutputCaptured)
            {
                context.ErrorData.AppendLine(dataReceivedEventArgs.Data);
            }

            if (context.IsOutputPrinted)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(dataReceivedEventArgs.Data);
                Console.ResetColor();
            }
        }

        Process BuildProcess(ExternalCommandRunContext context, string command, params Note[] args)
        {
            Process externalProcess = new Process();
            externalProcess.StartInfo.FileName = command;
            externalProcess.StartInfo.Arguments = argsBuilder.BuildInline(args.ToNoNullsArray());
            externalProcess.StartInfo.UseShellExecute = false;
            externalProcess.StartInfo.RedirectStandardOutput = context.IsOutputCaptured;
            externalProcess.StartInfo.RedirectStandardError = context.IsOutputCaptured;
            externalProcess.StartInfo.RedirectStandardInput = context.IsUserInputExpected;
            externalProcess.EnableRaisingEvents = true;
            externalProcess.OutputDataReceived += (s, e) =>
            {
                OnOutputDataReceived(s, e, context);
                CollectMetricsIfNecessary(context, externalProcess);
            };
            externalProcess.ErrorDataReceived += (s, e) =>
            {
                OnErrorDataReceived(s, e, context);
                CollectMetricsIfNecessary(context, externalProcess);
            };

            CollectMetricsIfNecessary(context, externalProcess);

            return externalProcess;
        }

        static void DisposeProcess(ExternalCommandRunContext context, Process externalProcess)
        {
            CollectMetricsIfNecessary(context, externalProcess, isForcedRegardlessOfMetricsInterval: true);

            if (context.IsOutputCaptured)
            {
                new Action(externalProcess.CancelOutputRead).TryOrFailWithGrace();
                new Action(externalProcess.CancelErrorRead).TryOrFailWithGrace();
            }
            if (context.IsUserInputExpected)
            {
                new Action(externalProcess.StandardInput.Close).TryOrFailWithGrace();
                new Action(externalProcess.StandardInput.Dispose).TryOrFailWithGrace();
            }
            new Action(externalProcess.Dispose).TryOrFailWithGrace();
        }

        static bool StartProcess(ExternalCommandRunContext context, Process externalProcess)
        {
            bool isNewProcess = externalProcess.Start();
            if (context.IsOutputCaptured)
            {
                externalProcess.BeginOutputReadLine();
                externalProcess.BeginErrorReadLine();
            }

            CollectMetricsIfNecessary(context, externalProcess);

            return isNewProcess;
        }

        static void CollectMetricsIfNecessary(ExternalCommandRunContext context, Process externalProcess, bool isForcedRegardlessOfMetricsInterval = false)
        {
            if (!context.IsMetricsCollectionEnabled)
                return;

            if (!isForcedRegardlessOfMetricsInterval)
            {
                DateTime latestMetricsCollectionTimestamp = context.Metrics.IsEmpty() ? DateTime.MinValue : context.Metrics.Max(x => x.Key);

                if (DateTime.UtcNow - latestMetricsCollectionTimestamp <= minimumMetricsCollectionInterval)
                    return;
            }

            (context.Metrics as IDictionary<DateTime, Note[]>).Add(DateTime.UtcNow,
                Note
                    .GetEnvironmentInfo()
                    .AppendProcessInfo(externalProcess, prefix: "ExternalProcess-")
                    .AppendProcessInfo(prefix: "HostProcess-")
            );
        }
    }
}
