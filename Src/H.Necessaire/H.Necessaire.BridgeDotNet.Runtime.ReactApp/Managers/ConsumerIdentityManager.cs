﻿using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ConsumerIdentityManager : ImADependency, ImADependencyGroup
    {
        #region Construct
        ConsumerIdentity consumerIdentity;
        DeviceInfoResource deviceInfoResource;
        ConsumerIdentityLocalStorageResource consumerIdentityLocalStorageResource;
        HttpClient httpClient;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            deviceInfoResource = dependencyProvider.Get<DeviceInfoResource>();
            consumerIdentityLocalStorageResource = dependencyProvider.Get<ConsumerIdentityLocalStorageResource>();
            httpClient = dependencyProvider.Get<HttpClient>();
        }

        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ConsumerIdentity>(() => consumerIdentity);
        }
        #endregion

        public async Task<ConsumerIdentity> CreateOrResurrect()
        {
            consumerIdentity
                = (await consumerIdentityLocalStorageResource.Search())
                ??
                new ConsumerIdentity()
                .And(x => x.IDTag = x.ID.ToString())
                ;

            await DecorateConsumerIdentity();

            await consumerIdentityLocalStorageResource.Save(consumerIdentity);

            return consumerIdentity;
        }

        public async Task<ConsumerIdentity> Resurrect()
        {
            consumerIdentity = await consumerIdentityLocalStorageResource.Search();

            await DecorateConsumerIdentity();

            return consumerIdentity;
        }

        private async Task DecorateConsumerIdentity()
        {
            if (deviceInfoResource == null)
                return;

            consumerIdentity.Notes = await deviceInfoResource.GetRawProperties();
            consumerIdentity.DisplayName = consumerIdentity.Notes.Get("UserAgent", ignoreCase: true);

            await
                new Func<Task>(async () =>
                {

                    OperationResult<ConsumerPlatformInfo> platformDataResult = await UserAgentData.GetPlatformDetails();

                    if (!platformDataResult.IsSuccessful)
                        return;

                    consumerIdentity.RuntimePlatform = platformDataResult.Payload;

                    consumerIdentity.DisplayName
                        = !string.IsNullOrWhiteSpace(platformDataResult.Payload?.UserAgent)
                        ? platformDataResult.Payload.UserAgent
                        : consumerIdentity.DisplayName
                        ;

                }).TryOrFailWithGrace(onFail: ex => { });

            consumerIdentity.AsOf = DateTime.UtcNow;

            httpClient.SetConsumer(consumerIdentity.ID);

            CallContext<OperationContext>.SetData(CallContextKey.OperationContext, new OperationContext
            {
                Consumer = consumerIdentity,
            });
        }
    }
}
