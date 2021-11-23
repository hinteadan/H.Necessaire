namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    internal class DebugWebWorkerDaemon : WebWorkerDaemonBase
    {
        public DebugWebWorkerDaemon() : base(() => new Worker())
        {
        }

        class Worker : ImAWebWorkerDaemonAction
        {
            public void DoWork()
            {

            }
        }
    }
}
