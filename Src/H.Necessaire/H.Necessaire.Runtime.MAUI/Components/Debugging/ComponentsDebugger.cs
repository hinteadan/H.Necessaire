using System.Collections.Concurrent;

namespace H.Necessaire.Runtime.MAUI.Components.Debugging
{
    static class ComponentsDebugger
    {
        static readonly ConcurrentBag<DebugEntry> debugEntries = new ConcurrentBag<DebugEntry>();

        public static void Log(string id, TimeSpan duration)
        {
            debugEntries.Add(new DebugEntry
            {
                ID = id,
                Duration = duration,
            });
        }

        public static DebugEntry[] Entries => debugEntries.OrderByDescending(e => e.Duration).ToArray();
    }


}
