using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    public class HsCosmosDebugger : ImADependency
    {
        HsCosmosStorageService cosmosStorageService;
        ImAStorageService<Guid, DataBin> storage;
        ImAStorageBrowserService<DataBin, DataBinFilter> browser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            cosmosStorageService = dependencyProvider.Get<HsCosmosStorageService>();
            storage = dependencyProvider.Get<ImAStorageService<Guid, DataBin>>();
            browser = dependencyProvider.Get<ImAStorageBrowserService<DataBin, DataBinFilter>>();
        }

        public async Task Debug()
        {
            using (new ProgressiveScope(WellKnownCosmosCallContextKey.CosmosDataBinProgressionScopeID, ReportProgress))
            {
                //FileInfo bigFile = new FileInfo(@"C:\Users\Hintea Dan Alexandru\Downloads\Firefly_Lane_S02_EP09_Kate_News_Scene.mp4");

                //OperationResult saveResult =
                //    await storage.Save(
                //        new DataBinMeta
                //        {
                //            ID = Guid.Parse("{DF27B6A3-3B93-4244-A601-A7508D019B16}"),
                //            Name = "BigFileTest",
                //            AsOf = DateTime.UtcNow,
                //            Description = "Bif File",
                //            Format = WellKnownDataBinFormat.GenericByteStream,
                //        }
                //        .ToBin
                //        (
                //            meta => bigFile.OpenRead().ToDataBinStream().AsTask()
                //        )
                //    );

                //DataBin dataBin = (await storage.LoadByID(Guid.Parse("{DF27B6A3-3B93-4244-A601-A7508D019B16}"))).ThrowOnFailOrReturn();

                //FileInfo bigFileOutput = new FileInfo($@"C:\Users\Hintea Dan Alexandru\Downloads\{dataBin.ID}.mp4");
                //if (bigFileOutput.Exists)
                //    bigFileOutput.Delete();

                //using (FileStream outputStream = bigFileOutput.Create())
                //using (Stream dataStream = (await dataBin.OpenDataBinStream()).DataStream)
                //{
                //    await dataStream.CopyToAsync(outputStream);
                //}
            }
        }

        private Task ReportProgress(object sender, ProgressEventArgs args)
        {
            Console.WriteLine($"{args.CurrentActionName}: {args.PercentValue.Print()} %");
            return Task.CompletedTask;
        }
    }
}
