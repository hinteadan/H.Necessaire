using Spectre.Console;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI.Concrete
{
    internal class CliUI : ImACliUI
    {
        public static CliUI Shared { get; } = new CliUI();

        public Task<IDisposable> DeterministicProgressScope(params ProgressiveScope[] progressiveScopes)
        {
            return
                BuildAndStartCliUiProgressiveScopeWithProgressTasksFromCurrentContext(
                    new CliUiProgressiveScope(progressiveScopes)
                );
        }

        public Task<IDisposable> DeterministicProgressScope(params string[] progressiveScopes)
        {
            return
                BuildAndStartCliUiProgressiveScopeWithProgressTasksFromCurrentContext(
                    new CliUiProgressiveScope(progressiveScopes)
                );
        }

        public Task<IDisposable> IndeterministicProgressScope(string defaultStatus = "...", params ProgressiveScope[] progressiveScopes)
        {
            return
                BuildAndStartCliUiProgressiveScopeWithStatusTaskFromCurrentContext(
                    defaultStatus,
                    new CliUiProgressiveScope(progressiveScopes)
                );
        }

        public Task<IDisposable> IndeterministicProgressScope(string defaultStatus = "...", params string[] progressiveScopes)
        {
            return
                BuildAndStartCliUiProgressiveScopeWithStatusTaskFromCurrentContext(
                    defaultStatus,
                    new CliUiProgressiveScope(progressiveScopes)
                );
        }

        async Task<IDisposable> BuildAndStartCliUiProgressiveScopeWithProgressTasksFromCurrentContext(CliUiProgressiveScope cliUiProgressiveScope)
        {
            return new CollectionOfDisposables<IDisposable>(
                cliUiProgressiveScope,
                await BuildAndStartProgressTasksFromCurrentContext()
            );
        }

        async Task<IDisposable> BuildAndStartCliUiProgressiveScopeWithStatusTaskFromCurrentContext(string defaultStatus, CliUiProgressiveScope cliUiProgressiveScope)
        {
            return new CollectionOfDisposables<IDisposable>(
                cliUiProgressiveScope,
                await BuildAndStartStatusTaskFromCurrentContext(defaultStatus)
            );
        }

        async Task<IDisposable> BuildAndStartProgressTasksFromCurrentContext()
        {
            TaskCompletionSource<bool> startCompletionSource = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> progressCompletionSource = new TaskCompletionSource<bool>();

            ProgressReporter[] progressReporters = CliUiProgressiveScope.Current?.ProgressReporters;

            if (progressReporters.IsEmpty())
            {
                return Enumerable.Empty<bool>().GetEnumerator();
            }

            ProgressTask[] progressTasks = new ProgressTask[progressReporters.Length];

            ScopedRunner scopedRunner = new ScopedRunner(
                onStart: () =>
                {
                    foreach (ProgressReporter progressReporter in progressReporters)
                    {
                        progressReporter.OnProgress += onProgress;
                    }
                },
                onStop: () =>
                {
                    foreach (ProgressReporter progressReporter in progressReporters)
                    {
                        progressReporter.OnProgress -= onProgress;
                    }

                    progressCompletionSource.SetResult(true);
                }
            );

            Task onProgress(object sender, ProgressEventArgs args)
            {
                ProgressReporter progressReporter = sender as ProgressReporter;
                int index = Array.IndexOf(progressReporters, progressReporter);
                ProgressTask progressTask = progressTasks[index];

                if (progressTask is null)
                    return Task.CompletedTask;

                progressTask.Description(args.CurrentActionName);
                progressTask.Value((double)args.PercentValue);

                PrintAdditionalInfoNecessary(args);

                return Task.CompletedTask;
            }



#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await
                    AnsiConsole
                    .Progress()
                    .StartAsync(async ctx =>
                    {
                        for (int index = 0; index < progressReporters.Length; index++)
                        {
                            string title = progressReporters[index].ID;
                            progressTasks[index] = ctx.AddTask(title.IsEmpty() ? "..." : title, new ProgressTaskSettings { AutoStart = true, MaxValue = 100 });
                        }

                        startCompletionSource.SetResult(true);

                        await progressCompletionSource.Task;
                    });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await startCompletionSource.Task;

            return scopedRunner;
        }

        async Task<IDisposable> BuildAndStartStatusTaskFromCurrentContext(string defaultStatus)
        {
            TaskCompletionSource<bool> startCompletionSource = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> progressCompletionSource = new TaskCompletionSource<bool>();

            ProgressReporter[] progressReporters = CliUiProgressiveScope.Current?.ProgressReporters;

            if (progressReporters.IsEmpty())
            {
                return Enumerable.Empty<bool>().GetEnumerator();
            }

            StatusContext statusContext = null;

            ScopedRunner scopedRunner = new ScopedRunner(
                onStart: () =>
                {
                    foreach (ProgressReporter progressReporter in progressReporters)
                    {
                        progressReporter.OnProgress += onProgress;
                    }
                },
                onStop: () =>
                {
                    foreach (ProgressReporter progressReporter in progressReporters)
                    {
                        progressReporter.OnProgress -= onProgress;
                    }

                    progressCompletionSource.SetResult(true);
                }
            );

            Task onProgress(object sender, ProgressEventArgs args)
            {
                ProgressReporter progressReporter = sender as ProgressReporter;

                if (args.PercentValue == 0)
                {
                    statusContext?.Status(args.CurrentActionName);
                }
                else
                {
                    statusContext?.Status($"{args.CurrentActionName} ({args.PercentValue.ToString("0.00")}%)");
                }

                PrintAdditionalInfoNecessary(args);

                return Task.CompletedTask;
            }



#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await
                    AnsiConsole
                    .Status()
                    .StartAsync(defaultStatus.IsEmpty() ? "..." : defaultStatus, async ctx =>
                    {
                        statusContext = ctx;

                        startCompletionSource.SetResult(true);

                        await progressCompletionSource.Task;
                    });
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await startCompletionSource.Task;

            return scopedRunner;
        }

        static void PrintAdditionalInfoNecessary(ProgressEventArgs args)
        {
            if (args.AdditionalInfo.IsEmpty())
                return;

            foreach (string info in args.AdditionalInfo)
            {
                $"[grey]{DateTime.Now} |[/] {info}".CliUiPrintMarkupLine();
            }
        }
    }
}
