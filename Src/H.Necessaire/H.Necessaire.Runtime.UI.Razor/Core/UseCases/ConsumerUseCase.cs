
using H.Necessaire.Runtime.UI.Razor.Core.Managers;

namespace H.Necessaire.Runtime.UI.Razor.Core.UseCases
{
    internal class ConsumerUseCase : UseCaseBase, ImAConsumerUseCase
    {
        static readonly TimeSpan consumerIdentityDetailsTimeout = TimeSpan.FromMinutes(5);
        ConsumerManager consumerManager;
        Func<HJs> hjsProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hjsProvider = () => dependencyProvider.Get<HJs>();
            consumerManager = dependencyProvider.Get<ConsumerManager>();
        }

        public async Task<ConsumerIdentity> CreateOrResurrect()
        {
            ConsumerIdentity consumerIdentity
                = await consumerManager.GetCurrentConsumer()
                ?? await hjsProvider().GetConsumerInfo(Guid.NewGuid())
                ;

            if (consumerIdentity is null)
                return null;

            if (consumerIdentity.AsOf + consumerIdentityDetailsTimeout <= DateTime.UtcNow)
            {
                consumerIdentity = await hjsProvider().GetConsumerInfo(consumerIdentity.ID);
            }

            await DecorateWithRequestDetailsIfNecessary(consumerIdentity);

            await consumerManager.SetCurrentConsumer(consumerIdentity);

            return consumerIdentity;
        }

        public async Task<ConsumerIdentity> Resurrect()
        {
            return await consumerManager.GetCurrentConsumer();
        }

        async Task DecorateWithRequestDetailsIfNecessary(ConsumerIdentity consumerIdentity)
        {
            if (consumerIdentity is null)
                return;

            if (!consumerIdentity.IpAddress.IsEmpty())
                return;

            UseCaseContext context = await GetCurrentContext();
            if (context?.OperationContext is null)
                return;

            consumerIdentity.ID = context.OperationContext.Consumer?.ID ?? consumerIdentity.ID;
            consumerIdentity.Notes = consumerIdentity.Notes.AddOrReplace(
                context?.OperationContext?.Parameters?.Get("Connection.RemoteIpAddress")?.NoteAs(WellKnownConsumerIdentityNote.IpAddress) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Connection.RemotePort")?.NoteAs(WellKnownConsumerIdentityNote.Port) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("TraceIdentifier")?.NoteAs(WellKnownConsumerIdentityNote.TraceIdentifier) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Connection.ID")?.NoteAs(WellKnownConsumerIdentityNote.ConnectionID) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Connection.LocalIpAddress")?.NoteAs(WellKnownConsumerIdentityNote.ServerIpAddress) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Connection.LocalPort")?.NoteAs(WellKnownConsumerIdentityNote.ServerPort) ?? Note.Empty
                , (context?.OperationContext?.Parameters?.Get("Request.Host") ?? context?.OperationContext?.Parameters?.Get("Request.Header.Host"))?.NoteAs(WellKnownConsumerIdentityNote.HostName) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Request.Protocol")?.NoteAs(WellKnownConsumerIdentityNote.Protocol) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Request.Header.User-Agent")?.NoteAs(WellKnownConsumerIdentityNote.UserAgent) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Request.Cookie.ai_user")?.NoteAs(WellKnownConsumerIdentityNote.AiUserID) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Request.Header.Origin")?.NoteAs(WellKnownConsumerIdentityNote.Origin) ?? Note.Empty
                , context?.OperationContext?.Parameters?.Get("Request.Header.Referer")?.NoteAs(WellKnownConsumerIdentityNote.Referer) ?? Note.Empty
            );
        }
    }
}
