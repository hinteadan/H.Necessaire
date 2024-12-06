using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public class HsFirestoreDebugger : ImADependency
    {
        ImAStorageService<Guid, DataBin> storage;
        ImAStorageBrowserService<DataBin, DataBinFilter> browser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            storage = dependencyProvider.Get<ImAStorageService<Guid, DataBin>>();
            browser = dependencyProvider.Get<ImAStorageBrowserService<DataBin, DataBinFilter>>();
        }

        public async Task Debug()
        {
            using (new ProgressiveScope(WellKnownFirestoreCallContextKey.FirestoreDataBinProgressionScopeID, ReportProgress))
            {
                FileInfo file = new FileInfo(@"C:\Users\Hintea Dan Alexandru\Downloads\DSC_0089.JPG");

                OperationResult saveResult =
                    await storage.Save(
                        new DataBinMeta
                        {
                            ID = Guid.Parse("{889BBBD6-BA51-4825-9329-DB132A67B6D3}"),
                            Name = file.Name.Substring(0, file.Name.LastIndexOf(".")),
                            AsOf = DateTime.UtcNow,
                            Description = "Testing Firstore runtime for DataBins",
                            Format = WellKnownDataBinFormat.GenericByteStream.And(x => x.Extension = file.Extension.Replace(".", "")),
                        }
                        .ToBin
                        (
                            meta => file.OpenRead().ToDataBinStream().AsTask()
                        )
                    );

                DataBin dataBin = (await storage.LoadByID(Guid.Parse("{889BBBD6-BA51-4825-9329-DB132A67B6D3}"))).ThrowOnFailOrReturn();

                FileInfo fileOutput = new FileInfo($@"C:\Users\Hintea Dan Alexandru\Downloads\{dataBin.ID}.{dataBin.Format.Extension}");
                if (fileOutput.Exists)
                    fileOutput.Delete();

                using (FileStream outputStream = fileOutput.Create())
                using (Stream dataStream = (await dataBin.OpenDataBinStream()).DataStream)
                {
                    await dataStream.CopyToAsync(outputStream);
                }
            }

        }

        private Task ReportProgress(object sender, ProgressEventArgs args)
        {
            Console.WriteLine($"{args.CurrentActionName}: {args.PercentValue.Print()} %");
            return Task.CompletedTask;
        }
    }
}
