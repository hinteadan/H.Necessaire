using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Necessaire.Runtime
{
    public static class EnvironmentExtensions
    {
        public static Note[] AppendProcessInfo(this Note[] notes)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                return
                    notes.Push(
                        new Note[]
                        {
                            $"Process-{currentProcess.PriorityClass}({(int)currentProcess.PriorityClass})".NoteAs(nameof(currentProcess.PriorityClass)),
                            $"Process-{currentProcess.PriorityBoostEnabled}".NoteAs(nameof(currentProcess.PriorityBoostEnabled)),

                            $"Process-{currentProcess.PeakWorkingSet64}".NoteAs(nameof(currentProcess.PeakWorkingSet64)),
                            $"Process-{currentProcess.PeakVirtualMemorySize64}".NoteAs(nameof(currentProcess.PeakVirtualMemorySize64)),
                            $"Process-{currentProcess.PeakPagedMemorySize64}".NoteAs(nameof(currentProcess.PeakPagedMemorySize64)),

                            $"Process-{currentProcess.PagedMemorySize64}".NoteAs(nameof(currentProcess.PagedMemorySize64)),
                            $"Process-{currentProcess.NonpagedSystemMemorySize64}".NoteAs(nameof(currentProcess.NonpagedSystemMemorySize64)),
                            $"Process-{currentProcess.MinWorkingSet}".NoteAs(nameof(currentProcess.MinWorkingSet)),
                            $"Process-{currentProcess.PagedSystemMemorySize64}".NoteAs(nameof(currentProcess.PagedSystemMemorySize64)),
                            $"Process-{currentProcess.PrivateMemorySize64}".NoteAs(nameof(currentProcess.PrivateMemorySize64)),
                            $"Process-{currentProcess.PrivilegedProcessorTime}".NoteAs(nameof(currentProcess.PrivilegedProcessorTime)),
                            $"Process-{currentProcess.WorkingSet64}".NoteAs(nameof(currentProcess.WorkingSet64)),
                            $"Process-{currentProcess.VirtualMemorySize64}".NoteAs(nameof(currentProcess.VirtualMemorySize64)),

                            $"Process-{currentProcess.ProcessName}".NoteAs(nameof(currentProcess.ProcessName)),

                            $"Process-{currentProcess.UserProcessorTime}".NoteAs(nameof(currentProcess.UserProcessorTime)),
                            $"Process-{currentProcess.TotalProcessorTime}".NoteAs(nameof(currentProcess.TotalProcessorTime)),

                            $"Process-{currentProcess.Modules.Count}".NoteAs(nameof(currentProcess.Modules)),
                            $"Process-{currentProcess.Threads.Count}".NoteAs(nameof(currentProcess.Threads)),

                            $"Process-{currentProcess.StartTime}".NoteAs(nameof(currentProcess.StartTime)),
                            $"Process-{currentProcess.StartInfo.FileName}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.FileName)}"),
                            $"Process-{currentProcess.StartInfo.WorkingDirectory}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.WorkingDirectory)}"),
                            $"Process-{currentProcess.StartInfo.Arguments}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.Arguments)}"),
                            $"Process-{currentProcess.StartInfo.WindowStyle}({(int)currentProcess.StartInfo.WindowStyle})".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.WindowStyle)}"),
                            $"Process-{currentProcess.StartInfo.Domain}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.Domain)}"),
                            $"Process-{currentProcess.StartInfo.UserName}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.UserName)}"),
                            $"Process-{currentProcess.StartInfo.LoadUserProfile}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.LoadUserProfile)}"),
                            $"Process-{currentProcess.StartInfo.CreateNoWindow}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.CreateNoWindow)}"),
                            $"Process-{currentProcess.StartInfo.Verb}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.Verb)}"),
                            $"Process-{string.Join(", ", currentProcess.StartInfo.Verbs ?? Array.Empty<string>())}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.Verbs)}"),
                            $"Process-{currentProcess.StartInfo.UseShellExecute}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.UseShellExecute)}"),
                            $"Process-{currentProcess.StartInfo.StandardOutputEncoding.WebName}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.StandardOutputEncoding)}"),
                            $"Process-{currentProcess.StartInfo.StandardErrorEncoding.WebName}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.StandardErrorEncoding)}"),
                            $"Process-{currentProcess.StartInfo.RedirectStandardOutput}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.RedirectStandardOutput)}"),
                            $"Process-{currentProcess.StartInfo.RedirectStandardInput}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.RedirectStandardInput)}"),
                            $"Process-{currentProcess.StartInfo.RedirectStandardError}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.RedirectStandardError)}"),
                            $"Process-{currentProcess.StartInfo.ErrorDialogParentHandle}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.ErrorDialogParentHandle)}"),
                            $"Process-{currentProcess.StartInfo.ErrorDialog}".NoteAs($"{nameof(currentProcess.StartInfo)}.{nameof(currentProcess.StartInfo.ErrorDialog)}"),
                        }
                        .Concat(
                            currentProcess.GetEnvironmentVariables()
                        )
                        .Concat(
                            currentProcess.GetEnvironment()
                        )
                    );
            }
        }

        public static Note[] GetEnvironmentVariables(this Process process, string prefix = "Process-EnvironmentVariable-")
        {
            if (process?.StartInfo?.EnvironmentVariables?.Keys is null)
                return Array.Empty<Note>();

            List<Note> result = new List<Note>();

            foreach (object key in process.StartInfo.EnvironmentVariables.Keys)
            {
                string id = key?.ToString();
                if (id is null)
                    continue;
                result.Add(process.StartInfo.EnvironmentVariables[id].NoteAs($"{prefix}{id}"));
            }

            return result.Where(x => !x.IsEmpty()).ToArray();
        }

        public static Note[] GetEnvironment(this Process process, string prefix = "Process-Environment-")
        {
            if (process?.StartInfo?.Environment?.Any() != true)
                return Array.Empty<Note>();

            return
                process.StartInfo.Environment
                .Select(x => x.Value.NoteAs($"{prefix}{x.Key}"))
                .Where(x => !x.IsEmpty())
                .ToArray()
                ;
        }
    }
}
