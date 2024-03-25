using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Reporting.Abstracts
{
    public abstract class MarkdownReportBuilderBase : ReportBuilderBase
    {
        protected MarkdownReportBuilderBase()
            : base
        (
            new Note("md", "text/markdown"),
            new Note("markdown", "text/markdown")
        )
        { }


        protected virtual async Task<StreamWriter> PrintHeader(StreamWriter streamWriter, string value, byte level = 1, bool isBold = false)
        {
            await streamWriter.WriteAsync(new string('#', level));
            await streamWriter.WriteAsync(" ");
            if (isBold)
                await streamWriter.WriteAsync("**");
            await streamWriter.WriteAsync(value);
            if (isBold)
                await streamWriter.WriteAsync("**");

            await PrintSpacer(streamWriter);

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintQuote(StreamWriter streamWriter, string value)
        {
            if (value.IsEmpty() == true)
                return streamWriter;

            string[] lines = value.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            foreach (string line in lines)
            {
                await streamWriter.WriteAsync("> ");
                await streamWriter.WriteAsync(line);
                await streamWriter.WriteLineAsync();
                await streamWriter.WriteAsync("> ");
                await streamWriter.WriteLineAsync();
            }

            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintSeparator(StreamWriter streamWriter, byte length = 30)
        {
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            await streamWriter.WriteAsync(new string('-', length));

            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintOrderedList(StreamWriter streamWriter, IEnumerable<string> values, byte level = 0)
        {
            if (values?.Any() != true)
                return streamWriter;

            uint index = 0;
            foreach (string value in values)
            {
                index++;
                await PrintOrderedListItem(streamWriter, value, level, index);
            }

            await PrintSpacer(streamWriter);

            return streamWriter;
        }
        protected virtual async Task<StreamWriter> PrintOrderedListItem(StreamWriter streamWriter, string value, byte level = 0, uint number = 1)
        {
            if (level > 0)
                await streamWriter.WriteAsync(new string(' ', level * 4));
            await streamWriter.WriteAsync(number.ToString());
            await streamWriter.WriteAsync(". ");
            await streamWriter.WriteAsync(value);
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintUnorderedList(StreamWriter streamWriter, IEnumerable<string> values, byte level = 0)
        {
            if (values?.Any() != true)
                return streamWriter;

            foreach (string value in values)
            {
                await PrintUnorderedListItem(streamWriter, value, level);
            }

            await PrintSpacer(streamWriter);

            return streamWriter;
        }
        protected virtual async Task<StreamWriter> PrintUnorderedListItem(StreamWriter streamWriter, string value, byte level = 0)
        {
            if (level > 0)
                await streamWriter.WriteAsync(new string(' ', level * 4));
            await streamWriter.WriteAsync("- ");
            await streamWriter.WriteAsync(value);
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintSpacer(StreamWriter streamWriter)
        {
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }

        protected virtual async Task<StreamWriter> PrintNone(StreamWriter streamWriter)
        {
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteAsync("_**None**_");
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();

            return streamWriter;
        }
    }

    public abstract class MarkdownReportBuilderBase<T> : MarkdownReportBuilderBase, ImAReportBuilder<T>
    {
        protected MarkdownReportBuilderBase() : base() { }

        public abstract Task<OperationResult<Stream>> BuildReport(T reportData);
    }
}
