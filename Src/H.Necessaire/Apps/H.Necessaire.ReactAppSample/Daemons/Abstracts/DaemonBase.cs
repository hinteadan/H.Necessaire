using Bridge.Html5;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;

namespace H.Necessaire.ReactAppSample
{
    internal class DaemonBase : WebWorkerDaemonBase
    {
        public DaemonBase()
            : base(
                () => App.Get<ConsolePingDaemon.Worker>(),
                () => new AppWireup(),
                Window.Location.Origin + "/H.Necessaire.ReactAppSample.js",
                Window.Location.Origin + "/H.Necessaire.ReactAppSample.meta.js"
            )
        {
        }
    }
}
