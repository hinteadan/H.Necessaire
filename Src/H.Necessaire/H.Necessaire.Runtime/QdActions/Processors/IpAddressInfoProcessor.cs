using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing;
using H.Necessaire.Runtime.QdActions.Processors.IpAddressProcessing.Providers.Concrete;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.QdActions.Processors
{
    internal class IpAddressInfoProcessor : QdActionProcessorBase
    {
        protected override string[] SupportedQdActionTypes { get; } = WellKnown.QdActionType.ProcessIpAddress.AsArray();

        static ImANetworkTraceProvider[] networkTraceProviders;
        ImALogger logger;
        ImAStorageService<Guid, NetworkTrace> networkTraceStorage;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

            logger = dependencyProvider.GetLogger<IpAddressInfoProcessor>();
            networkTraceStorage = dependencyProvider.Get<ImAStorageService<Guid, NetworkTrace>>();

            networkTraceProviders = networkTraceProviders ?? new ImANetworkTraceProvider[] {
                new IpGeolocationIoTracer(),
                new IpApiTracer(),
                new IpLocateIoTracer(),
                new IpStackComTracer(),
                new FreeGeoIpAppTracer(),
                new IpFindComTracer(),
                new IpInfoDbTracer(),
            };
        }

        protected override async Task<OperationResult> ProcessQdAction(QdAction action)
        {
            NetworkTrace networkTrace = null;

            foreach (ImANetworkTraceProvider networkTraceProvider in networkTraceProviders)
            {
                OperationResult<NetworkTrace> result = await TraceIpAddress(networkTraceProvider, action.Payload);
                if (result.IsSuccessful)
                {
                    networkTrace = result.Payload;
                    break;
                }

            }

            if (networkTrace == null)
                return OperationResult.Fail($"Couldn't trace {action.Payload} with any of the exisitng IP Tracers");


            return
                await SaveNetworkTrace(networkTrace);
        }

        private async Task<OperationResult<NetworkTrace>> TraceIpAddress(ImANetworkTraceProvider networkTraceProvider, string ipAddress)
        {
            OperationResult<NetworkTrace> result = OperationResult.Fail("Not yet started").WithoutPayload<NetworkTrace>();

            await
                new Func<Task>(async () =>
                {
                    result = await networkTraceProvider.Trace(ipAddress);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex).WithoutPayload<NetworkTrace>();
                    }
                );

            return result;
        }

        private async Task<OperationResult> SaveNetworkTrace(NetworkTrace networkTrace)
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await networkTraceStorage.Save(networkTrace);
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
