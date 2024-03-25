﻿using H.Necessaire.CLI.Commands.HDoc.Model;
using H.Necessaire.Runtime.Reporting.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting
{
    [ID("hdoc-reporter-md")]
    [Alias("hdoc-reporter-markdown")]
    internal class HDocumentationMarkdownReporter : MarkdownReportBuilderBase<HDocumentation>
    {
        public override async Task<OperationResult<Stream>> BuildReport(HDocumentation reportData)
        {
            if (reportData == null)
                return OperationResult.Fail("Report Data is NULL").WithoutPayload<Stream>();

            OperationResult<Stream> result = OperationResult.Fail("Not yet started").WithoutPayload<Stream>();

            await
                new Func<Task>(async () =>
                {

                    MemoryStream resultStream = new MemoryStream();

                    using (StreamWriter printer = new StreamWriter(resultStream, encoding: Encoding.UTF8, bufferSize: 1024 * 10, leaveOpen: true))
                    {
                        await PrintReportTitle(printer, reportData);
                        await PrintSummary(printer, reportData);
                        await PrintDocumentation(printer, reportData);
                    }

                    resultStream.Seek(0, SeekOrigin.Begin);
                    result = (resultStream as Stream).ToWinResult();

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to Build WebDomains Markdown Report. Message: {ex.Message}").WithoutPayload<Stream>();
                    }
                );

            return result;
        }

        private async Task PrintDocumentation(StreamWriter printer, HDocumentation reportData)
        {
            IEnumerable<IGrouping<string, HDocTypeInfo>> modules = reportData.AllTypes.GroupBy(t => t.Module);

            foreach (IGrouping<string, HDocTypeInfo> module in modules)
            {
                await PrintHeader(printer, module.Key, level: 2);
                await PrintSpacer(printer);
                await PrintSeparator(printer);
                await PrintSpacer(printer);

                IEnumerable<IGrouping<string, HDocTypeInfo>> categories = module.GroupBy(t => t.Category);

                foreach (IGrouping<string, HDocTypeInfo> category in categories)
                {
                    if (!category.Key.IsEmpty())
                    {
                        await PrintHeader(printer, category.Key, level: 3);
                        await PrintSpacer(printer);
                        await PrintSeparator(printer);
                        await PrintSpacer(printer);
                    }

                    foreach (HDocTypeInfo typeDoc in category)
                    {
                        await PrintTypeDocumentation(printer, typeDoc);
                    }
                }
            }
        }

        private async Task PrintTypeDocumentation(StreamWriter printer, HDocTypeInfo typeDoc)
        {
            await PrintHeader(printer, $"**{typeDoc.Name}**", level: 4);
            await PrintSeparator(printer);
            await PrintSpacer(printer);
        }

        private async Task PrintReportTitle(StreamWriter printer, HDocumentation reportData)
        {
            await PrintHeader(printer, $"**H.Necessaire** ");
            await printer.WriteLineAsync(reportData.Version.ToString());
            await PrintSeparator(printer);
            await PrintQuote(printer, $"_as of **{reportData.AsOf.PrintDateAndTime()}**");
            await PrintSeparator(printer);
            await PrintSpacer(printer);
        }

        private async Task PrintSummary(StreamWriter printer, HDocumentation reportData)
        {
            await PrintHeader(printer, "Summary", level: 2);

            await PrintUnorderedList(printer, new string[] {

                $"{reportData.AllTypes.Select(t => t.Module).Distinct().Count()} modules",
                $"{reportData.AllTypes.Select(t => t.Category).Distinct().Count()} categories",
                $"{reportData.AllTypes.Length} total types",

            });

            await PrintSeparator(printer);
            await PrintSpacer(printer);
        }
    }
}
