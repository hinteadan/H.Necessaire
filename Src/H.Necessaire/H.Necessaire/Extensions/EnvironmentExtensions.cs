using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H.Necessaire
{
    public static class EnvironmentExtensions
    {
        public static Note[] AppendProcessInfo(this Note[] notes, Process process = null, string prefix = "Process-")
        {
            if (process != null)
                return notes.Push(process.GetProcessInfo(prefix));

            using (process = Process.GetCurrentProcess())
            {
                return notes.Push(process.GetProcessInfo(prefix));
            }
        }

        public static Note[] GetProcessInfo(this Process process, string prefix = "Process-")
        {
            Note[] result = Array.Empty<Note>();

            new Action(() =>
            {
                result
                    = new Note[]
                    {
                        $"{DateTime.UtcNow.PrintTimeStampAsIdentifier()}".NoteAs($"{prefix}AsOf"),
                        $"{DateTime.UtcNow.Ticks}".NoteAs($"{prefix}AsOf-Ticks"),

                        SafelyGrabValueOrNull(() => $"{process.Id}").NoteAs($"{prefix}Id"),

                        SafelyGrabValueOrNull(() => $"{process.PriorityClass}({(int)process.PriorityClass})").NoteAs($"{prefix}PriorityClass"),
                        SafelyGrabValueOrNull(() => $"{process.BasePriority}").NoteAs($"{prefix}BasePriority"),
                        SafelyGrabValueOrNull(() => $"{process.PriorityBoostEnabled}").NoteAs($"{prefix}PriorityBoostEnabled"),
                        SafelyGrabValueOrNull(() => $"{process.ProcessorAffinity}").NoteAs($"{prefix}ProcessorAffinity"),

                        SafelyGrabValueOrNull(() => $"{process.PeakWorkingSet64}").NoteAs($"{prefix}PeakWorkingSet64"),
                        SafelyGrabValueOrNull(() => $"{process.PeakVirtualMemorySize64}").NoteAs($"{prefix}PeakVirtualMemorySize64"),
                        SafelyGrabValueOrNull(() => $"{process.PeakPagedMemorySize64}").NoteAs($"{prefix}PeakPagedMemorySize64"),

                        SafelyGrabValueOrNull(() => $"{process.PagedMemorySize64}").NoteAs($"{prefix}PagedMemorySize64"),
                        SafelyGrabValueOrNull(() => $"{process.NonpagedSystemMemorySize64}").NoteAs($"{prefix}NonpagedSystemMemorySize64"),
                        SafelyGrabValueOrNull(() => $"{process.MinWorkingSet}").NoteAs($"{prefix}MinWorkingSet"),
                        SafelyGrabValueOrNull(() => $"{process.MaxWorkingSet}").NoteAs($"{prefix}MaxWorkingSet"),
                        SafelyGrabValueOrNull(() => $"{process.PagedSystemMemorySize64}").NoteAs($"{prefix}PagedSystemMemorySize64"),
                        SafelyGrabValueOrNull(() => $"{process.PrivateMemorySize64}").NoteAs($"{prefix}PrivateMemorySize64"),
                        SafelyGrabValueOrNull(() => $"{process.PrivilegedProcessorTime}").NoteAs($"{prefix}PrivilegedProcessorTime"),
                        SafelyGrabValueOrNull(() => $"{process.WorkingSet64}").NoteAs($"{prefix}WorkingSet64"),
                        SafelyGrabValueOrNull(() => $"{process.VirtualMemorySize64}").NoteAs($"{prefix}VirtualMemorySize64"),

                        SafelyGrabValueOrNull(() => $"{process.ProcessName}").NoteAs($"{prefix}ProcessName"),
                        SafelyGrabValueOrNull(() => $"{process.MachineName}").NoteAs($"{prefix}MachineName"),
                        SafelyGrabValueOrNull(() => $"{process.MainWindowTitle}").NoteAs($"{prefix}MainWindowTitle"),
                        SafelyGrabValueOrNull(() => $"{process.MainWindowHandle}").NoteAs($"{prefix}MainWindowHandle"),
                        SafelyGrabValueOrNull(() => $"{process.SessionId}").NoteAs($"{prefix}SessionId"),
                        SafelyGrabValueOrNull(() => $"{process.Responding}").NoteAs($"{prefix}Responding"),
                        SafelyGrabValueOrNull(() => $"{process.HasExited}").NoteAs($"{prefix}HasExited"),
                        SafelyGrabValueOrNull(() => $"{process.EnableRaisingEvents}").NoteAs($"{prefix}EnableRaisingEvents"),

                        SafelyGrabValueOrNull(() => $"{process.MainModule.ModuleName}").NoteAs($"{prefix}MainModule-ModuleName"),
                        SafelyGrabValueOrNull(() => $"{process.MainModule.FileName}").NoteAs($"{prefix}MainModule-FileName"),
                        SafelyGrabValueOrNull(() => $"{process.MainModule.ModuleMemorySize}").NoteAs($"{prefix}MainModule-ModuleMemorySize"),
                        SafelyGrabValueOrNull(() => $"{process.MainModule.BaseAddress}").NoteAs($"{prefix}MainModule-BaseAddress"),
                        SafelyGrabValueOrNull(() => $"{process.MainModule.EntryPointAddress}").NoteAs($"{prefix}MainModule-EntryPointAddress"),
                        SafelyGrabValueOrNull(() => $"{process.MainModule.FileVersionInfo}").NoteAs($"{prefix}MainModule-FileVersionInfo"),


                        SafelyGrabValueOrNull(() => $"{process.UserProcessorTime}").NoteAs($"{prefix}UserProcessorTime"),
                        SafelyGrabValueOrNull(() => $"{process.TotalProcessorTime}").NoteAs($"{prefix}TotalProcessorTime"),

                        SafelyGrabValueOrNull(() => $"{process.Modules.Count}").NoteAs($"{prefix}Modules"),
                        SafelyGrabValueOrNull(() => $"{process.Threads.Count}").NoteAs($"{prefix}Threads"),
                        SafelyGrabValueOrNull(() => $"{process.HandleCount}").NoteAs($"{prefix}HandleCount"),
                        SafelyGrabValueOrNull(() => $"{process.Handle}").NoteAs($"{prefix}Handle"),

                        SafelyGrabValueOrNull(() => $"{process.StartTime}").NoteAs($"{prefix}StartTime"),
                    }
                    .Concat(
                        process.GetStartInfo($"{prefix}StartInfo-")
                    )
                    .Where(x => !x.Value.IsEmpty())
                    .ToArray()
                    ;

            }).TryOrFailWithGrace();

            return result;
        }

        public static Note[] GetStartInfo(this Process process, string prefix = "Process-StartInfo-")
        {
            List<Note> list = new List<Note>();

            new Action(() =>
            {
                ProcessStartInfo startInfo = process.StartInfo;

                list.AddRange(
                    new Note[] {
                        SafelyGrabValueOrNull(() => $"{startInfo.FileName}").NoteAs($"{prefix}FileName"),
                        SafelyGrabValueOrNull(() => $"{startInfo.WorkingDirectory}").NoteAs($"{prefix}WorkingDirectory"),
                        SafelyGrabValueOrNull(() => $"{startInfo.Arguments}").NoteAs($"{prefix}Arguments"),
                        SafelyGrabValueOrNull(() => $"{startInfo.WindowStyle}({(int)startInfo.WindowStyle})").NoteAs($"{prefix}WindowStyle"),
                        SafelyGrabValueOrNull(() => $"{startInfo.Domain}").NoteAs($"{prefix}Domain"),
                        SafelyGrabValueOrNull(() => $"{startInfo.UserName}").NoteAs($"{prefix}UserName"),
                        SafelyGrabValueOrNull(() => $"{startInfo.LoadUserProfile}").NoteAs($"{prefix}LoadUserProfile"),
                        SafelyGrabValueOrNull(() => $"{startInfo.CreateNoWindow}").NoteAs($"{prefix}CreateNoWindow"),
                        SafelyGrabValueOrNull(() => $"{startInfo.Verb}").NoteAs($"{prefix}Verb"),
                        SafelyGrabValueOrNull(() => $"{string.Join(", ", startInfo.Verbs ?? Array.Empty<string>())}").NoteAs($"{prefix}Verbs"),
                        SafelyGrabValueOrNull(() => $"{startInfo.UseShellExecute}").NoteAs($"{prefix}UseShellExecute"),
                        SafelyGrabValueOrNull(() => $"{startInfo.StandardOutputEncoding.WebName}").NoteAs($"{prefix}StandardOutputEncoding"),
                        SafelyGrabValueOrNull(() => $"{startInfo.StandardErrorEncoding.WebName}").NoteAs($"{prefix}StandardErrorEncoding"),
                        SafelyGrabValueOrNull(() => $"{startInfo.RedirectStandardOutput}").NoteAs($"{prefix}RedirectStandardOutput"),
                        SafelyGrabValueOrNull(() => $"{startInfo.RedirectStandardInput}").NoteAs($"{prefix}RedirectStandardInput"),
                        SafelyGrabValueOrNull(() => $"{startInfo.RedirectStandardError}").NoteAs($"{prefix}RedirectStandardError"),
                        SafelyGrabValueOrNull(() => $"{startInfo.ErrorDialogParentHandle}").NoteAs($"{prefix}ErrorDialogParentHandle"),
                        SafelyGrabValueOrNull(() => $"{startInfo.ErrorDialog}").NoteAs($"{prefix}ErrorDialog"),
                    }
                );

                list.AddRange(process.GetEnvironmentVariables($"{prefix}-EnvironmentVariable-"));
                list.AddRange(process.GetEnvironment($"{prefix}-Environment-"));

            }).TryOrFailWithGrace();

            return list.Where(x => !x.Value.IsEmpty()).ToArray();
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
                result.Add(SafelyGrabValueOrNull(() => process.StartInfo.EnvironmentVariables[id]).NoteAs($"{prefix}{id}"));
            }

            return result.Where(x => !x.Value.IsEmpty()).ToArray();
        }

        public static Note[] GetEnvironment(this Process process, string prefix = "Process-Environment-")
        {
            if (process?.StartInfo?.Environment?.Any() != true)
                return Array.Empty<Note>();

            return
                process.StartInfo.Environment
                .Select(x => SafelyGrabValueOrNull(() => x.Value).NoteAs($"{prefix}{x.Key}"))
                .Where(x => !x.Value.IsEmpty())
                .ToArray()
                ;
        }

        static string SafelyGrabValueOrNull(Func<object> valueSelector)
        {
            string result = null;

            new Action(() =>
            {

                result = $"{valueSelector.Invoke()}";

            }).TryOrFailWithGrace(onFail: ex => result = null);

            return result;
        }
    }
}
