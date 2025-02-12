using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Sync.Concrete
{
    internal class SyncRequestProcessorFactory : ImASyncRequestProcessorFactory, ImADependency
    {
        ImASyncRequestProcessor[] knownProcessors = new ImASyncRequestProcessor[0];
        ImASyncRequestExilationProcessor catchAllSyncRequestProcessor = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            Type[] processorTypes
                = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(
                    assembly
                        => assembly
                        .GetTypes()
                        .Where(p => p != typeof(CatchAllSyncRequestProcessor) && typeof(ImASyncRequestProcessor).IsAssignableFrom(p) && !p.IsAbstract)
                )
                .ToArray();

            catchAllSyncRequestProcessor = dependencyProvider?.Get<CatchAllSyncRequestProcessor>();
            knownProcessors
                = (dependencyProvider?.Get<ImASyncRequestProcessor>()?.AsArray() ?? new ImASyncRequestProcessor[0])
                .Union(dependencyProvider?.Get<ImASyncRequestProcessor[]>() ?? new ImASyncRequestProcessor[0])
                .Union(processorTypes.Select(x => dependencyProvider.Get(x) as ImASyncRequestProcessor))
                .Union(catchAllSyncRequestProcessor?.AsArray() ?? new ImASyncRequestProcessor[0])
                .ToArray();
        }

        public async Task<ImASyncRequestProcessor> BuildProcessorFor(SyncRequest syncRequest)
        {
            foreach (ImASyncRequestProcessor syncRequestProcessor in knownProcessors ?? new ImASyncRequestProcessor[0])
            {
                if (!await syncRequestProcessor.IsEligibleFor(syncRequest))
                    continue;

                return syncRequestProcessor;
            }

            return null;
        }

        public Task<ImASyncRequestExilationProcessor> BuildExilationProcessor() => catchAllSyncRequestProcessor.AsTask();
    }
}
