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

                        process.SafeRead(p => $"{p.Id}").NoteAs($"{prefix}Id"),

                        process.SafeRead(p => $"{p.PriorityClass}").NoteAs($"{prefix}PriorityClass"),
                        process.SafeRead(p => $"{p.BasePriority}").NoteAs($"{prefix}BasePriority"),
                        process.SafeRead(p => $"{p.PriorityBoostEnabled}").NoteAs($"{prefix}PriorityBoostEnabled"),
                        process.SafeRead(p => $"{p.ProcessorAffinity}").NoteAs($"{prefix}ProcessorAffinity"),

                        process.SafeRead(p => $"{p.PeakWorkingSet64}").NoteAs($"{prefix}PeakWorkingSet64"),
                        process.SafeRead(p => $"{p.PeakVirtualMemorySize64}").NoteAs($"{prefix}PeakVirtualMemorySize64"),
                        process.SafeRead(p => $"{p.PeakPagedMemorySize64}").NoteAs($"{prefix}PeakPagedMemorySize64"),

                        process.SafeRead(p => $"{p.PagedMemorySize64}").NoteAs($"{prefix}PagedMemorySize64"),
                        process.SafeRead(p => $"{p.NonpagedSystemMemorySize64}").NoteAs($"{prefix}NonpagedSystemMemorySize64"),
                        process.SafeRead(p => $"{p.MinWorkingSet}").NoteAs($"{prefix}MinWorkingSet"),
                        process.SafeRead(p => $"{p.MaxWorkingSet}").NoteAs($"{prefix}MaxWorkingSet"),
                        process.SafeRead(p => $"{p.PagedSystemMemorySize64}").NoteAs($"{prefix}PagedSystemMemorySize64"),
                        process.SafeRead(p => $"{p.PrivateMemorySize64}").NoteAs($"{prefix}PrivateMemorySize64"),
                        process.SafeRead(p => $"{p.PrivilegedProcessorTime}").NoteAs($"{prefix}PrivilegedProcessorTime"),
                        process.SafeRead(p => $"{p.WorkingSet64}").NoteAs($"{prefix}WorkingSet64"),
                        process.SafeRead(p => $"{p.VirtualMemorySize64}").NoteAs($"{prefix}VirtualMemorySize64"),

                        process.SafeRead(p => $"{p.ProcessName}").NoteAs($"{prefix}ProcessName"),
                        process.SafeRead(p => $"{p.MachineName}").NoteAs($"{prefix}MachineName"),
                        process.SafeRead(p => $"{p.MainWindowTitle}").NoteAs($"{prefix}MainWindowTitle"),
                        process.SafeRead(p => $"{p.MainWindowHandle}").NoteAs($"{prefix}MainWindowHandle"),
                        process.SafeRead(p => $"{p.SessionId}").NoteAs($"{prefix}SessionId"),
                        process.SafeRead(p => $"{p.Responding}").NoteAs($"{prefix}Responding"),
                        process.SafeRead(p => $"{p.HasExited}").NoteAs($"{prefix}HasExited"),
                        process.SafeRead(p => $"{p.EnableRaisingEvents}").NoteAs($"{prefix}EnableRaisingEvents"),

                        process.SafeRead(p => $"{p.UserProcessorTime}").NoteAs($"{prefix}UserProcessorTime"),
                        process.SafeRead(p => $"{p.TotalProcessorTime}").NoteAs($"{prefix}TotalProcessorTime"),

                        process.SafeRead(p => $"{p.Modules.Count}").NoteAs($"{prefix}Modules"),
                        process.SafeRead(p => $"{p.Threads.Count}").NoteAs($"{prefix}Threads"),
                        process.SafeRead(p => $"{p.HandleCount}").NoteAs($"{prefix}HandleCount"),
                        process.SafeRead(p => $"{p.Handle}").NoteAs($"{prefix}Handle"),

                        process.SafeRead(p => $"{p.StartTime}").NoteAs($"{prefix}StartTime"),
                    }
                    .Concat(
                        process.GetMainModuleInfo($"{prefix}MainModule-")
                    )
                    .Concat(
                        process.GetStartInfo($"{prefix}StartInfo-")
                    )
                    .Where(x => !x.Value.IsEmpty())
                    .ToArray()
                    ;

            }).TryOrFailWithGrace();

            return result;
        }

        public static Note[] GetMainModuleInfo(this Process process, string prefix = "Process-MainModule-")
        {
            List<Note> list = new List<Note>();

            new Action(() =>
            {
                var moduleInfo = process.MainModule;

                list.AddRange(
                    new Note[] {

                        moduleInfo.ModuleName.NoteAs($"{prefix}ModuleName"),
                        moduleInfo.FileName.NoteAs($"{prefix}FileName"),
                        $"{moduleInfo.ModuleMemorySize}".NoteAs($"{prefix}ModuleMemorySize"),
                        $"{moduleInfo.BaseAddress}".NoteAs($"{prefix}BaseAddress"),
                        $"{moduleInfo.EntryPointAddress}".NoteAs($"{prefix}EntryPointAddress"),
                        $"{moduleInfo.FileVersionInfo}".NoteAs($"{prefix}FileVersionInfo"),
                    }
                );

            }).TryOrFailWithGrace();

            return list.Where(x => !x.Value.IsEmpty()).ToArray();
        }

        public static Note[] GetStartInfo(this Process process, string prefix = "Process-StartInfo-")
        {
            List<Note> list = new List<Note>();

            new Action(() =>
            {
                ProcessStartInfo startInfo = process.StartInfo;

                if (startInfo is null)
                    return;

                list.AddRange(
                    new Note[] {

                        startInfo.SafeRead(x => $"{x.FileName}").NoteAs($"{prefix}FileName"),
                        startInfo.SafeRead(x => $"{x.WorkingDirectory}").NoteAs($"{prefix}WorkingDirectory"),
                        startInfo.SafeRead(x => $"{x.Arguments}").NoteAs($"{prefix}Arguments"),
                        startInfo.SafeRead(x => $"{x.WindowStyle}({(int)x.WindowStyle})").NoteAs($"{prefix}WindowStyle"),
                        startInfo.SafeRead(x => $"{x.Domain}").NoteAs($"{prefix}Domain"),
                        startInfo.SafeRead(x => $"{x.UserName}").NoteAs($"{prefix}UserName"),
                        startInfo.SafeRead(x => $"{x.LoadUserProfile}").NoteAs($"{prefix}LoadUserProfile"),
                        startInfo.SafeRead(x => $"{x.CreateNoWindow}").NoteAs($"{prefix}CreateNoWindow"),
                        startInfo.SafeRead(x => $"{x.Verb}").NoteAs($"{prefix}Verb"),
                        startInfo.SafeRead(x => $"{string.Join(", ", x.Verbs ?? Array.Empty<string>())}").NoteAs($"{prefix}Verbs"),
                        startInfo.SafeRead(x => $"{x.UseShellExecute}").NoteAs($"{prefix}UseShellExecute"),
                        startInfo.SafeRead(x => $"{x.StandardOutputEncoding.WebName}").NoteAs($"{prefix}StandardOutputEncoding"),
                        startInfo.SafeRead(x => $"{x.StandardErrorEncoding.WebName}").NoteAs($"{prefix}StandardErrorEncoding"),
                        startInfo.SafeRead(x => $"{x.RedirectStandardOutput}").NoteAs($"{prefix}RedirectStandardOutput"),
                        startInfo.SafeRead(x => $"{x.RedirectStandardInput}").NoteAs($"{prefix}RedirectStandardInput"),
                        startInfo.SafeRead(x => $"{x.RedirectStandardError}").NoteAs($"{prefix}RedirectStandardError"),
                        startInfo.SafeRead(x => $"{x.ErrorDialogParentHandle}").NoteAs($"{prefix}ErrorDialogParentHandle"),
                        startInfo.SafeRead(x => $"{x.ErrorDialog}").NoteAs($"{prefix}ErrorDialog"),
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
                result.Add(process.StartInfo.EnvironmentVariables[id].NoteAs($"{prefix}{id}"));
            }

            return result.Where(x => !x.Value.IsEmpty()).ToArray();
        }

        public static Note[] GetEnvironment(this Process process, string prefix = "Process-Environment-")
        {
            if (process?.StartInfo?.Environment?.Any() != true)
                return Array.Empty<Note>();

            return
                process.StartInfo.Environment
                .Select(x => x.Value.NoteAs($"{prefix}{x.Key}"))
                .Where(x => !x.Value.IsEmpty())
                .ToArray()
                ;
        }
    }
}
