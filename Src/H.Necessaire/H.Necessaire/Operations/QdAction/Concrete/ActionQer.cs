using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class ActionQer : ImAnActionQer, ImADependency
    {
        ImAStorageService<Guid, QdAction> storageService;
        ImALogger logger;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<ActionQer>();
            storageService = dependencyProvider.Get<ImAStorageService<Guid, QdAction>>();
        }

        public async Task<OperationResult> Queue(QdAction action)
        {
            if (storageService == null)
                return OperationResult.Fail("Cannot find any QdAction Storage Service");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await storageService.Save(action);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex);
                    }
                );

            return result;
        }
    }
}
