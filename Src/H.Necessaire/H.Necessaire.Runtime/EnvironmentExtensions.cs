using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;

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
                            $"{currentProcess.Id}".NoteAs($"Process-Id"),

                            $"{currentProcess.PriorityClass}({(int)currentProcess.PriorityClass})".NoteAs($"Process-PriorityClass"),
                            $"{currentProcess.BasePriority}".NoteAs($"Process-BasePriority"),
                            $"{currentProcess.PriorityBoostEnabled}".NoteAs($"Process-PriorityBoostEnabled"),
                            $"{currentProcess.ProcessorAffinity}".NoteAs($"Process-ProcessorAffinity"),

                            $"{currentProcess.PeakWorkingSet64}".NoteAs($"Process-PeakWorkingSet64"),
                            $"{currentProcess.PeakVirtualMemorySize64}".NoteAs($"Process-PeakVirtualMemorySize64"),
                            $"{currentProcess.PeakPagedMemorySize64}".NoteAs($"Process-PeakPagedMemorySize64"),

                            $"{currentProcess.PagedMemorySize64}".NoteAs($"Process-PagedMemorySize64"),
                            $"{currentProcess.NonpagedSystemMemorySize64}".NoteAs($"Process-NonpagedSystemMemorySize64"),
                            $"{currentProcess.MinWorkingSet}".NoteAs($"Process-MinWorkingSet"),
                            $"{currentProcess.MaxWorkingSet}".NoteAs($"Process-MaxWorkingSet"),
                            $"{currentProcess.PagedSystemMemorySize64}".NoteAs($"Process-PagedSystemMemorySize64"),
                            $"{currentProcess.PrivateMemorySize64}".NoteAs($"Process-PrivateMemorySize64"),
                            $"{currentProcess.PrivilegedProcessorTime}".NoteAs($"Process-PrivilegedProcessorTime"),
                            $"{currentProcess.WorkingSet64}".NoteAs($"Process-WorkingSet64"),
                            $"{currentProcess.VirtualMemorySize64}".NoteAs($"Process-VirtualMemorySize64"),

                            $"{currentProcess.ProcessName}".NoteAs($"Process-ProcessName"),
                            $"{currentProcess.MachineName}".NoteAs($"Process-MachineName"),
                            $"{currentProcess.MainWindowHandle}".NoteAs($"Process-MainWindowHandle"),
                            $"{currentProcess.SessionId}".NoteAs($"Process-SessionId"),
                            $"{currentProcess.Responding}".NoteAs($"Process-Responding"),
                            $"{currentProcess.HasExited}".NoteAs($"Process-HasExited"),
                            $"{currentProcess.EnableRaisingEvents}".NoteAs($"Process-EnableRaisingEvents"),

                            $"{currentProcess.MainModule.ModuleName}".NoteAs($"Process-MainModule-ModuleName"),
                            $"{currentProcess.MainModule.FileName}".NoteAs($"Process-MainModule-FileName"),
                            $"{currentProcess.MainModule.ModuleMemorySize}".NoteAs($"Process-MainModule-ModuleMemorySize"),
                            $"{currentProcess.MainModule.BaseAddress}".NoteAs($"Process-MainModule-BaseAddress"),
                            $"{currentProcess.MainModule.EntryPointAddress}".NoteAs($"Process-MainModule-EntryPointAddress"),
                            $"{currentProcess.MainModule.FileVersionInfo}".NoteAs($"Process-MainModule-FileVersionInfo"),


                            $"{currentProcess.UserProcessorTime}".NoteAs($"Process-UserProcessorTime"),
                            $"{currentProcess.TotalProcessorTime}".NoteAs($"Process-TotalProcessorTime"),

                            $"{currentProcess.Modules.Count}".NoteAs($"Process-Modules"),
                            $"{currentProcess.Threads.Count}".NoteAs($"Process-Threads"),
                            $"{currentProcess.HandleCount}".NoteAs($"Process-HandleCount"),
                            $"{currentProcess.Handle}".NoteAs($"Process-Handle"),

                            $"{currentProcess.StartTime}".NoteAs($"Process-StartTime"),
                        }
                        .Concat(
                            currentProcess.GetStartInfo()
                        )
                    );
            }
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

                list.AddRange(process.GetEnvironmentVariables());
                list.AddRange(process.GetEnvironment());

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
