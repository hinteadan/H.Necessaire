using H.Necessaire.CLI.Commands.HDoc.Model;
using H.Necessaire.Runtime.Reporting;
using H.Necessaire.Runtime.Reporting.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.CLI.Commands.HDoc.BLL.Reporting.HTML.Abstract
{
    internal abstract class StaticWebSiteReporterBase<T> : ReportBuilderBase, ImAReportBuilder<T>
    {
        protected StaticWebSiteReporterBase()
            : this
        (
            new Note("web", "application/zip"),
            new Note("website", "application/zip"),
            new Note("staticwebsite", "application/zip"),
            new Note("web.zip", "application/zip"),
            new Note("website.zip", "application/zip"),
            new Note("staticwebsite.zip", "application/zip"),
            new Note("html.zip", "application/zip")
        )
        { }
        protected StaticWebSiteReporterBase(params Note[] supportedFormats) : base(supportedFormats) { }

        public async Task<OperationResult<Stream>> BuildReport(T reportData)
        {
            if (reportData == null)
                return OperationResult.Fail("Report Data is NULL").WithoutPayload<Stream>();

            OperationResult<Stream> result = OperationResult.Fail("Not yet started").WithoutPayload<Stream>();

            await
                new Func<Task>(async () =>
                {

                    MemoryStream resultStream = new MemoryStream();

                    using (ZipArchive zipArchive = new ZipArchive(resultStream, ZipArchiveMode.Create, leaveOpen: true, Encoding.UTF8))
                    {
                        foreach (Task<TaggedStream> sourceStreamTask in await GetContentStreams())
                        {
                            using (TaggedStream sourceStream = await sourceStreamTask)
                            {
                                ZipArchiveEntry zipEntry = zipArchive.CreateEntry(sourceStream.ID);
                                using (Stream zipStream = zipEntry.Open())
                                {
                                    await sourceStream.Stream.CopyToAsync(zipStream);
                                }
                            }
                        }
                    }

                    resultStream.Seek(0, SeekOrigin.Begin);
                    result = (resultStream as Stream).ToWinResult();

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to Build {typeof(T).Name} StaticWebSite Report. Message: {ex.Message}").WithoutPayload<Stream>();
                    }
                );

            return result;
        }

        protected abstract Task<IEnumerable<Task<TaggedStream>>> GetContentStreams();
    }
}
