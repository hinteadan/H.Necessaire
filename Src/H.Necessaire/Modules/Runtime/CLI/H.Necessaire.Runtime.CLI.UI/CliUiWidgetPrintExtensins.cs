using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.CLI.UI
{
    public static class CliUiWidgetPrintExtensins
    {
        public static void CliUiPrintPanel(this Note note)
        {
            if (note.IsEmpty())
                return;

            AnsiConsole.Write(note.Value.InPanel(note.ID));
        }

        public static void CliUiPrintCalendar(this DateTime dateTime, IEnumerable<DateTime> events = null, bool isHeaderVisible = true, System.Globalization.CultureInfo cultureInfo = null)
        {
            Calendar calendar
                = new Calendar(dateTime)
                .AndIf(!isHeaderVisible, x => x.HideHeader())
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                .Culture(cultureInfo is null ? System.Globalization.CultureInfo.CurrentCulture : cultureInfo)
                .And(x => events.ProcessStream(ev => x.AddCalendarEvent(ev)))
                .HeaderStyle(Style.Plain.Foreground(Color.Yellow))
                .And(x => x.HighlightStyle = Style.Plain.Foreground(Color.Yellow))
                ;

            AnsiConsole.Write(calendar);
        }

        internal static Panel InPanel(this string content, string title = null)
            => ((MultiType<string, IRenderable>)content).CreatePanel(title);

        internal static Panel InPanel(this IRenderable content, string title = null)
            => ((MultiType<string, IRenderable>)content).CreatePanel(title);

        static Panel CreatePanel(this MultiType<string, IRenderable> content, string title = null)
        {
            return
                (content.HasA ? new Panel(content.A ?? "") : new Panel(content.B ?? new Markup("")))
                .AndIf(!title.IsEmpty(), p => p.Header(title))
                .Header(title)
                .Collapse()
                .Padding(2, 1)
                .RoundedBorder()
                .BorderColor(Color.Yellow)
                ;
        }
    }
}
