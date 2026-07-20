using Org.BouncyCastle.Crypto.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Daemons
{
    internal class BackgroundServiceDaemonDebugger : ImADependency, IDebug
    {
        BackgroundServiceDaemon backgroundServiceDaemon;
        ImALogger log;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            log = dependencyProvider.GetLogger<BackgroundServiceDaemonDebugger>();
            backgroundServiceDaemon = new BackgroundServiceDaemon(dependencyProvider, DoWork, TimeSpan.FromSeconds(3));
        }

        static readonly Random rnd = new Random();
        async Task DoWork()
        {
            using (await log.LogInfoDuration("DoWork", "⚙⏳"))
            {
                await Task.Delay(rnd.Next(100, 1000));
            }            
        }

        public async Task Debug()
        {
            TimeSpan runFor = TimeSpan.FromSeconds(15);
            int threadCount = 5;

            var cts = new CancellationTokenSource(runFor);

            using (await log.LogInfoDuration("Debugging", "🐞"))
            {
                await Task.WhenAll(
                    Enumerable.Range(0, threadCount).Select(i => backgroundServiceDaemon.Start(cts.Token))
                );

                await HSafe.Run(async () => await Task.Delay(runFor.Add(TimeSpan.FromSeconds(1.5))));
            }
        }
    }
}
