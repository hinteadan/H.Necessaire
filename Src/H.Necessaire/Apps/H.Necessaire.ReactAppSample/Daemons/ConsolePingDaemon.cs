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

            public void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                repeater = new ActionRepeater(Ping, TimeSpan.FromSeconds(1));
            }

            public async void DoWork()
            {
                await repeater.Start();
            }

            private Task Ping()
            {
                Console.WriteLine($"{DateTime.Now.PrintDateAndTime()} Ping from [{App.WebWorkerID}] Thread");

                return true.AsTask();
            }
        }
    }
}
