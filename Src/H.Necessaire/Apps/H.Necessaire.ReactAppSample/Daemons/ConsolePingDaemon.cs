using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.ReactAppSample
{
    internal class ConsolePingDaemon : DaemonBase
    {
        public class Worker : ImAWebWorkerDaemonAction, ImADependency
        {
            ActionRepeater repeater;
            ImALogger logger;

            public void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                repeater = new ActionRepeater(Ping, TimeSpan.FromSeconds(1));
                logger = dependencyProvider.GetLogger<ConsolePingDaemon>();
            }

            public async void DoWork()
            {
                await repeater.Start();
            }

            private async Task Ping()
            {
                await logger.LogTrace($"Ping from [{App.WebWorkerID}] Thread");
            }
        }
    }
}
