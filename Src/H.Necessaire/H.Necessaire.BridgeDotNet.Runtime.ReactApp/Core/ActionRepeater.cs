using Bridge.Html5;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ActionRepeater
    {
        #region Construct
        readonly Func<Task> action;
        readonly TimeSpan every;
        int? latestSetTimeoutId = null;
        bool isStopRequested = false;
        public ActionRepeater(Func<Task> action, TimeSpan every)
        {
            this.action = action;
            this.every = every;
        }
        #endregion

        public async Task Start()
        {
            isStopRequested = false;

            await RunActionAndQueueAnother();
        }

        public Task Stop()
        {
            isStopRequested = true;

            if (latestSetTimeoutId == null)
                return true.AsTask();

            Window.ClearTimeout(latestSetTimeoutId.Value);

            latestSetTimeoutId = null;

            return true.AsTask();
        }

        private async Task RunActionAndQueueAnother()
        {
            await action();

            if (isStopRequested)
                return;

            latestSetTimeoutId = Window.SetTimeout(async () =>
            {

                await RunActionAndQueueAnother();

            }, (int)every.TotalMilliseconds);
        }
    }
}
