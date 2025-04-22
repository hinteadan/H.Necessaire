using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class Debouncer
    {
        #region Construct
        readonly Func<Task> asyncActionToTame;
        readonly TimeSpan debounceInterval = TimeSpan.Zero;
        ImAPeriodicAction periodicActionExecutioner;
        public Debouncer(Func<Task> asyncActionToTame, TimeSpan debounceInterval, Func<ImAPeriodicAction> periodicActionFactory)
        {
            this.asyncActionToTame = asyncActionToTame;
            this.debounceInterval = debounceInterval;
            this.periodicActionExecutioner = periodicActionFactory();
        }

        public Debouncer(Func<Task> asyncActionToTame, TimeSpan debounceInterval) : this(asyncActionToTame, debounceInterval, ConcreteFactory.BuildNewPeriodicAction) { }
        #endregion

        public async Task Invoke()
        {
            periodicActionExecutioner.Stop();
            periodicActionExecutioner.StartDelayed(debounceInterval, debounceInterval, async () =>
            {
                periodicActionExecutioner.Stop();
                await asyncActionToTame.Invoke();
            });

            await true.AsTask();
        }

        public void Dispose()
        {
            periodicActionExecutioner.Stop();
        }
    }
}
