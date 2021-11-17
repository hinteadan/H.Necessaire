using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    public class ConsolePingDaemon : HostedServiceDaemonBase
    {
        static readonly TimeSpan pingInterval = TimeSpan.FromSeconds(15);

        protected override Task DoWork(CancellationToken? cancellationToken = null)
        {
            Console.WriteLine($"{DateTime.Now} Ping from [{Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}] Thread");

            return true.AsTask();
        }

        protected override TimeSpan WorkCycleInterval { get; } = pingInterval;
    }
}
