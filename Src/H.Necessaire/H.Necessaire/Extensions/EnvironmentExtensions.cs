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
                        $"{process.Id}".NoteAs($"{prefix}Id"),

                        $"{process.PriorityClass}({(int)process.PriorityClass})".NoteAs($"{prefix}PriorityClass"),
                        $"{process.BasePriority}".NoteAs($"{prefix}BasePriority"),
                        $"{process.PriorityBoostEnabled}".NoteAs($"{prefix}PriorityBoostEnabled"),
                        $"{process.ProcessorAffinity}".NoteAs($"{prefix}ProcessorAffinity"),

                        $"{process.PeakWorkingSet64}".NoteAs($"{prefix}PeakWorkingSet64"),
                        $"{process.PeakVirtualMemorySize64}".NoteAs($"{prefix}PeakVirtualMemorySize64"),
                        $"{process.PeakPagedMemorySize64}".NoteAs($"{prefix}PeakPagedMemorySize64"),

                        $"{process.PagedMemorySize64}".NoteAs($"{prefix}PagedMemorySize64"),
                        $"{process.NonpagedSystemMemorySize64}".NoteAs($"{prefix}NonpagedSystemMemorySize64"),
                        $"{process.MinWorkingSet}".NoteAs($"{prefix}MinWorkingSet"),
                        $"{process.MaxWorkingSet}".NoteAs($"{prefix}MaxWorkingSet"),
                        $"{process.PagedSystemMemorySize64}".NoteAs($"{prefix}PagedSystemMemorySize64"),
                        $"{process.PrivateMemorySize64}".NoteAs($"{prefix}PrivateMemorySize64"),
                        $"{process.PrivilegedProcessorTime}".NoteAs($"{prefix}PrivilegedProcessorTime"),
                        $"{process.WorkingSet64}".NoteAs($"{prefix}WorkingSet64"),
                        $"{process.VirtualMemorySize64}".NoteAs($"{prefix}VirtualMemorySize64"),

                        $"{process.ProcessName}".NoteAs($"{prefix}ProcessName"),
                        $"{process.MachineName}".NoteAs($"{prefix}MachineName"),
                        $"{process.MainWindowTitle}".NoteAs($"{prefix}MainWindowTitle"),
                        $"{process.MainWindowHandle}".NoteAs($"{prefix}MainWindowHandle"),
                        $"{process.SessionId}".NoteAs($"{prefix}SessionId"),
                        $"{process.Responding}".NoteAs($"{prefix}Responding"),
                        $"{process.HasExited}".NoteAs($"{prefix}HasExited"),
                        $"{process.EnableRaisingEvents}".NoteAs($"{prefix}EnableRaisingEvents"),

                        $"{process.MainModule.ModuleName}".NoteAs($"{prefix}MainModule-ModuleName"),
                        $"{process.MainModule.FileName}".NoteAs($"{prefix}MainModule-FileName"),
                        $"{process.MainModule.ModuleMemorySize}".NoteAs($"{prefix}MainModule-ModuleMemorySize"),
                        $"{process.MainModule.BaseAddress}".NoteAs($"{prefix}MainModule-BaseAddress"),
                        $"{process.MainModule.EntryPointAddress}".NoteAs($"{prefix}MainModule-EntryPointAddress"),
                        $"{process.MainModule.FileVersionInfo}".NoteAs($"{prefix}MainModule-FileVersionInfo"),


                        $"{process.UserProcessorTime}".NoteAs($"{prefix}UserProcessorTime"),
                        $"{process.TotalProcessorTime}".NoteAs($"{prefix}TotalProcessorTime"),

                        $"{process.Modules.Count}".NoteAs($"{prefix}Modules"),
                        $"{process.Threads.Count}".NoteAs($"{prefix}Threads"),
                        $"{process.HandleCount}".NoteAs($"{prefix}HandleCount"),
                        $"{process.Handle}".NoteAs($"{prefix}Handle"),

                        $"{process.StartTime}".NoteAs($"{prefix}StartTime"),
                    }
                    .Concat(
                        process.GetStartInfo($"{prefix}-StartInfo-")
                    )
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
                        $"{startInfo.FileName}".NoteAs($"Process-StartInfo-FileName"),
                        $"{startInfo.WorkingDirectory}".NoteAs($"Process-StartInfo-WorkingDirectory"),
                        $"{startInfo.Arguments}".NoteAs($"Process-StartInfo-Arguments"),
                        $"{startInfo.WindowStyle}({(int)startInfo.WindowStyle})".NoteAs($"Process-StartInfo-WindowStyle"),
                        $"{startInfo.Domain}".NoteAs($"Process-StartInfo-Domain"),
                        $"{startInfo.UserName}".NoteAs($"Process-StartInfo-UserName"),
                        $"{startInfo.LoadUserProfile}".NoteAs($"Process-StartInfo-LoadUserProfile"),
                        $"{startInfo.CreateNoWindow}".NoteAs($"Process-StartInfo-CreateNoWindow"),
                        $"{startInfo.Verb}".NoteAs($"Process-StartInfo-Verb"),
                        $"{string.Join(", ", startInfo.Verbs ?? Array.Empty<string>())}".NoteAs($"Process-StartInfo-Verbs"),
                        $"{startInfo.UseShellExecute}".NoteAs($"Process-StartInfo-UseShellExecute"),
                        $"{startInfo.StandardOutputEncoding.WebName}".NoteAs($"Process-StartInfo-StandardOutputEncoding"),
                        $"{startInfo.StandardErrorEncoding.WebName}".NoteAs($"Process-StartInfo-StandardErrorEncoding"),
                        $"{startInfo.RedirectStandardOutput}".NoteAs($"Process-StartInfo-RedirectStandardOutput"),
                        $"{startInfo.RedirectStandardInput}".NoteAs($"Process-StartInfo-RedirectStandardInput"),
                        $"{startInfo.RedirectStandardError}".NoteAs($"Process-StartInfo-RedirectStandardError"),
                        $"{startInfo.ErrorDialogParentHandle}".NoteAs($"Process-StartInfo-ErrorDialogParentHandle"),
                        $"{startInfo.ErrorDialog}".NoteAs($"Process-StartInfo-ErrorDialog"),
                    }
                );

                list.AddRange(process.GetEnvironmentVariables($"{prefix}-EnvironmentVariable-"));
                list.AddRange(process.GetEnvironment($"{prefix}-Environment-"));

            }).TryOrFailWithGrace();

            return list.Where(x => !x.IsEmpty()).ToArray();
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
