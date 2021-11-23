using System;
using System.Linq;

namespace H.Necessaire
{
    public class LogConfig
    {
        public static LogEntryLevel[] AllLogLevels { get; } = Enum.GetValues(typeof(LogEntryLevel)).Cast<LogEntryLevel>().ToArray();

        public static LogConfig None { get; } = new LogConfig();
        public static LogConfig Everything { get; } = new LogConfig { EnabledLevels = AllLogLevels };
        public static LogConfig Default { get; } = Everything;

        public LogEntryLevel[] EnabledLevels { get; set; } = new LogEntryLevel[0];
        public LogEntryLevel MinimumLevelForStackTrace { get; set; } = LogEntryLevel.Error;
        public ComponentConfig[] PerComponent { get; set; } = null;
        public Note[] Notes { get; set; }

        public LogEntryLevel[] ProcessEnabledLevelsFor(string component = null)
        {
            return
                PerComponent?.LastOrDefault(
                    cfg => !string.IsNullOrWhiteSpace(cfg?.Component)
                    && (
                        (component?.StartsWith(cfg.Component, StringComparison.InvariantCultureIgnoreCase) ?? false)
                        || (component?.EndsWith(cfg.Component, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    )
                )?.EnabledLevels
                ?? EnabledLevels
                ?? Default.EnabledLevels
                ;
        }

        public static LogEntryLevel[] LevelsHigherOrEqualTo(LogEntryLevel minimumLogLevel, bool includeNone = true)
            => AllLogLevels.Where(x => x >= minimumLogLevel).Union(includeNone ? LogEntryLevel.None.AsArray() : new LogEntryLevel[0]).ToArray();

        public static LogEntryLevel[] LevelsBetweenAndIncluding(LogEntryLevel from, LogEntryLevel to, bool includeNone = true)
            => AllLogLevels.Where(x => x >= from && x <= to).Union(includeNone ? LogEntryLevel.None.AsArray() : new LogEntryLevel[0]).ToArray();

        public class ComponentConfig
        {
            public string Component { get; set; } = null;
            public LogEntryLevel[] EnabledLevels { get; set; } = new LogEntryLevel[0];
        }
    }
}
