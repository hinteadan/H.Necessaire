using H.Necessaire.Runtime.Wireup.Abstracts;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class NetCoreApiWireupBase : ApiWireupBase
    {
        public override ImAnApiWireup WithEverything()
        {
            ImAnApiWireup result =
                base
                .WithEverything()
                .With(x => x.Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup()))
                .With(x => x.Register<SyncRequestProcessingHostedServiceDaemon>(() => new SyncRequestProcessingHostedServiceDaemon()))
                //.With(x => x.Register<ConsolePingDaemon>(() => new ConsolePingDaemon()))
            ;

            return result;
        }
    }
}
