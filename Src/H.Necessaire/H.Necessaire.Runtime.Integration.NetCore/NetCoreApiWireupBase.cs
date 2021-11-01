using H.Necessaire.Runtime.Wireup.Abstracts;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class NetCoreApiWireupBase : ApiWireupBase
    {
        public override ImAnApiWireup WithEverything()
        {
            return
                base
                .WithEverything()
                ;
        }
    }
}
