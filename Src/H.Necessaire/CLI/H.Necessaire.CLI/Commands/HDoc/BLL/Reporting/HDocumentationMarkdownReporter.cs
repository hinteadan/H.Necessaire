using H.Necessaire.CLI.Commands.HDoc.Model;
using H.Necessaire.Runtime.Reporting.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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

                    using (ZipArchive zipArchive = new ZipArchive(resultStream, ZipArchiveMode.Create, leaveOpen: true, Encoding.UTF8))
                    {
                        IEnumerable<IGrouping<string, HDocTypeInfo>> modules = reportData.AllTypes.GroupBy(t => t.Module);

                        foreach(HDocTypeInfo docTypeInfo in reportData.AllTypes)
                        {
                            string path = docTypeInfo.Module.ToSafeFileName();
                            if (!docTypeInfo.Category.IsEmpty())
                                path = $"{path}/{docTypeInfo.Category.ToSafeFileName()}";
                            path = $"{path}/{docTypeInfo.Name.ToSafeFileName(replacementValue: ".")}.txt.md";

                            ZipArchiveEntry moduleFileEntry = zipArchive.CreateEntry(path);

                            using (Stream moduleFileEntryStream = moduleFileEntry.Open())
                            using (StreamWriter moduleFileEntryStreamWriter = new StreamWriter(moduleFileEntryStream))
                            {
                                await PrintTypeDocumentation(moduleFileEntryStreamWriter, docTypeInfo);
                            }
                        }
                    }

                    resultStream.Seek(0, SeekOrigin.Begin);
                    result = (resultStream as Stream).ToWinResult();

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to Build HDocumentation Markdown Report. Message: {ex.Message}").WithoutPayload<Stream>();
                    }
                );

            return result;
        }

        private async Task PrintModuleDocumentation(StreamWriter printer, string moduleName, IEnumerable<HDocTypeInfo> moduleTypes)
        {
            await PrintHeader(printer, moduleName, level: 2);
            await PrintSpacer(printer);
            await PrintSeparator(printer);
            await PrintSpacer(printer);

            IEnumerable<IGrouping<string, HDocTypeInfo>> categories = moduleTypes.GroupBy(t => t.Category);

            foreach (IGrouping<string, HDocTypeInfo> category in categories)
            {
                if (!category.Key.IsEmpty())
                {
                    await PrintHeader(printer, $"_**{category.Key}**_", level: 3);
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

        private async Task PrintTypeDocumentation(StreamWriter printer, HDocTypeInfo typeDoc)
        {
            await PrintHeader(printer, $"**{typeDoc.Name.Replace(">", "\\>")}**", level: 4);
            await PrintSeparator(printer);
            await PrintSpacer(printer);

            if (typeDoc.Constructors?.Any() == true)
            {
                await PrintHeader(printer, $"**Constructor(s)**", level: 5);
                await PrintSeparator(printer);
                await PrintSpacer(printer);

                foreach (HDocConstructorInfo constructor in typeDoc.Constructors)
                {
                    await PrintConstructorDocumentation(printer, typeDoc, constructor);
                }
            }

            if (typeDoc.Fields?.Any() == true)
            {
                await PrintHeader(printer, $"**Field(s)**", level: 5);
                await PrintSeparator(printer);
                await PrintSpacer(printer);

                foreach (HDocFieldInfo field in typeDoc.Fields)
                {
                    await PrintFieldDocumentation(printer, field);
                }
            }

            if (typeDoc.Properties?.Any() == true)
            {
                await PrintHeader(printer, $"**Propertie(s)**", level: 5);
                await PrintSeparator(printer);
                await PrintSpacer(printer);

                foreach (var property in typeDoc.Properties)
                {
                    await PrintPropertyDocumentation(printer, property);
                }
            }
        }

        private async Task PrintConstructorDocumentation(StreamWriter printer, HDocTypeInfo typeDoc, HDocConstructorInfo constructorDoc)
        {
            await PrintProtectedMarkerIf(printer, constructorDoc.IsProtected);
            await printer.WriteAsync(typeDoc.Name);
            await printer.WriteAsync("(");
            await PrintParamtersIfAny(printer, constructorDoc.Parameters);
            await printer.WriteAsync(")");
            await PrintSpacer(printer);
        }

        private async Task PrintFieldDocumentation(StreamWriter printer, HDocFieldInfo fieldDoc)
        {
            await PrintProtectedMarkerIf(printer, fieldDoc.IsProtected);

            if (fieldDoc.IsConst)
            {
                await printer.WriteAsync("_const_ ");
            }
            else
            {
                await PrintStaticMarkerIf(printer, fieldDoc.IsStatic);
                await PrintReadonlyMarkerIf(printer, fieldDoc.IsReadonly);
            }

            await printer.WriteAsync(fieldDoc.Type);
            await printer.WriteAsync(" ");
            await printer.WriteAsync(fieldDoc.Name);

            if (fieldDoc.HasDefaultValue)
            {
                await printer.WriteAsync(" = ");
                await printer.WriteAsync(fieldDoc.DefaultsTo);
            }

            await PrintSpacer(printer);
        }

        private async Task PrintPropertyDocumentation(StreamWriter printer, HDocPropertyInfo property)
        {
            await PrintProtectedMarkerIf(printer, property.IsProtected);

            await PrintStaticMarkerIf(printer, property.IsStatic);

            await printer.WriteAsync(property.Type);
            await printer.WriteAsync(" ");
            await printer.WriteAsync(property.Name);

            await PrintSpacer(printer);
        }

        private async Task PrintReportTitle(StreamWriter printer, HDocumentation reportData)
        {
            await PrintHeader(printer, $"**H.Necessaire**");
            await printer.WriteLineAsync(reportData.Version.ToString());
            await PrintSeparator(printer);
            await PrintQuote(printer, $"_as of **{reportData.AsOf.PrintDateAndTime()}**_");
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

        private async Task PrintParamtersIfAny(StreamWriter printer, params HDocParameterInfo[] parameterInfos)
        {
            if (parameterInfos?.Any() != true)
                return;

            int paramIndex = -1;
            foreach (HDocParameterInfo param in parameterInfos)
            {
                paramIndex++;
                await printer.WriteAsync(param.Type);
                await printer.WriteAsync(" ");
                await printer.WriteAsync(param.Name);
                if (param.IsOptional)
                {
                    await printer.WriteAsync(" = ");
                    await printer.WriteAsync(param.DefaultsTo);
                }
                if (paramIndex < parameterInfos.Length - 1)
                {
                    await printer.WriteAsync(", ");
                }
            }
        }

        private async Task PrintProtectedMarkerIf(StreamWriter printer, bool isProtected)
        {
            if (!isProtected)
                return;

            await printer.WriteAsync("_protected_ ");
        }

        private async Task PrintReadonlyMarkerIf(StreamWriter printer, bool isReadOnly)
        {
            if (!isReadOnly)
                return;

            await printer.WriteAsync("_readonly_ ");
        }

        private async Task PrintStaticMarkerIf(StreamWriter printer, bool isStatic)
        {
            if (!isStatic)
                return;

            await printer.WriteAsync("_static_ ");
        }
    }
}
