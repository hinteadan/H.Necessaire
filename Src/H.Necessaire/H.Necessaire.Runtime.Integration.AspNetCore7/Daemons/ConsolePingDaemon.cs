﻿using H.Necessaire.Runtime.Integration.AspNetCore7.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.AspNetCore7.Daemons
{
    public class ConsolePingDaemon : HostedServiceDaemonBase
    {
        static readonly TimeSpan pingInterval = TimeSpan.FromSeconds(15);

        protected override async Task DoWork(CancellationToken? cancellationToken = null)
        {
            await Logger.LogInfo($"Ping from [{Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}] Thread");
        }

        protected override TimeSpan WorkCycleInterval { get; } = pingInterval;
    }
}
