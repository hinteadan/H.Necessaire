using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class Throttler
    {
        #region Construct
        readonly Func<Task> asyncActionToTame;
        readonly TimeSpan throttleInterval = TimeSpan.Zero;
        ImAPeriodicAction periodicActionExecutioner;
        ImAPeriodicAction periodicActionDestroyer;

        public Throttler(Func<Task> asyncActionToTame, TimeSpan throttleInterval, Func<ImAPeriodicAction> periodicActionFactory)
        {
            this.asyncActionToTame = asyncActionToTame;
            this.throttleInterval = throttleInterval;
            this.periodicActionExecutioner = periodicActionFactory();
            this.periodicActionDestroyer = periodicActionFactory();
        }

        public Throttler(Func<Task> asyncActionToTame, TimeSpan throttleInterval) : this(asyncActionToTame, throttleInterval, ConcreteFactory.BuildNewPeriodicAction) { }
        #endregion

        public async Task Invoke()
        {
            periodicActionExecutioner.Start(throttleInterval, async () =>
            {
                await asyncActionToTame?.Invoke();
            });

            periodicActionDestroyer.Stop();
            periodicActionDestroyer.StartDelayed(throttleInterval, throttleInterval, () => { periodicActionExecutioner.Stop(); return true.AsTask(); });

            await Task.FromResult(true);
        }
    }
}
