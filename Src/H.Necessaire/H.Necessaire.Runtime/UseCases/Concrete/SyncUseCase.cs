using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    class SyncUseCase : UseCaseBase, ImASyncUseCase
    {
        static readonly IDentity useCaseIdentity = new InternalIdentity
        {
            ID = Guid.Parse("46B99D7E-12D2-4943-9EEF-45225AC944CF"),
            IDTag = nameof(SyncUseCase),
            DisplayName = "Sync Use Case",
        };

        ImAStorageService<string, SyncRequest> syncRequestStorageService;
        ImAnAuditingService auditingService = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            syncRequestStorageService = dependencyProvider.Get<ImAStorageService<string, SyncRequest>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
        }

        public async Task<OperationResult<SyncResponse>[]> Sync(SyncRequest[] syncRequests)
        {
            return
                await Task.WhenAll(syncRequests.Select(SyncRequest));
        }

        private async Task<OperationResult<SyncResponse>> SyncRequest(SyncRequest syncRequest)
        {
            OperationResult<SyncResponse> result = null;

            await
                new Func<Task>(async () =>
                {
                    UseCaseContext context = await GetCurrentContext();

                    syncRequest.OperationContext
                    = (syncRequest.OperationContext?.Consumer != null ? syncRequest.OperationContext : context.OperationContext ?? new OperationContext())
                    .And(x =>
                    {
                        if (x.Consumer != null)
                        {
                            x.Consumer.ID = context?.OperationContext?.Consumer?.ID ?? x.Consumer.ID;
                            x.Consumer.Notes = x.Consumer.Notes.AddOrReplace(
                                context?.OperationContext?.Parameters?.Get("Connection.RemoteIpAddress")?.NoteAs(WellKnownConsumerIdentityNote.IpAddress) ?? Note.Empty
                                , context?.OperationContext?.Parameters?.Get("Connection.RemotePort")?.NoteAs(WellKnownConsumerIdentityNote.Port) ?? Note.Empty
                            );
                        }
                        x.Parameters = x.Parameters.Add(context?.OperationContext?.Parameters);
                        x.Notes = x.Notes.Add(context?.OperationContext?.Notes);
                    });


                    OperationResult saveResult = await syncRequestStorageService.Save(syncRequest);
                    if (saveResult.IsSuccessful)
                    {
                        result = saveResult.WithPayload(syncRequest.ToWinResponse());
                        await auditingService.Append(syncRequest.ToAuditMeta<SyncRequest, string>(AuditActionType.Create, useCaseIdentity), syncRequest);
                    }
                    else
                    {
                        result = saveResult.WithPayload(syncRequest.ToFailResponse());
                    }

                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex).WithPayload(syncRequest.ToFailResponse()));

            return result;
        }
    }
}
