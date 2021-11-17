using H.Necessaire.BridgeDotNet.Runtime.ReactApp;

namespace H.Necessaire.ReactAppSample
{
    public class AppWireup : AppWireupBase
    {
        public override ImAnAppWireup WithEverything()
        {
            return
                base
                .WithEverything()
                .With(x => x.Register<RuntimeConfig>(() => AppConfig.Default))
                .With(x => x.Register<DaemonsDependencyGroup>(() => new DaemonsDependencyGroup()))
                ;
        }
    }
}
