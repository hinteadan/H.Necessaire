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
                .With(x => x.Register<NetCoreDependencyGroup>(() => new NetCoreDependencyGroup()))
            ;

            return result;
        }
    }
}
