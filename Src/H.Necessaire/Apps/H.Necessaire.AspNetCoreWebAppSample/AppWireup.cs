using H.Necessaire.Runtime;
using H.Necessaire.Runtime.Integration.NetCore;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    public class AppWireup : NetCoreApiWireupBase
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
