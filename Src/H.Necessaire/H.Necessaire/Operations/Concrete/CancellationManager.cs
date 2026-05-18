using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Concrete
{
    internal class CancellationManager : ImACancellationManager
    {
        readonly AsyncEventRaiser<EventArgs> beforeCancelRaiser;
        readonly AsyncEventRaiser<EventArgs> afterCancelRaiser;
        readonly CancellationTokenSource cancellationTokenSource;
        public CancellationManager(params CancellationToken[] linkedTokens)
        {
            cancellationTokenSource = linkedTokens.IsEmpty() ? new CancellationTokenSource() : CancellationTokenSource.CreateLinkedTokenSource(linkedTokens);
            beforeCancelRaiser = new AsyncEventRaiser<EventArgs>(this);
            afterCancelRaiser = new AsyncEventRaiser<EventArgs>(this);
        }

        public event AsyncEventHandler<EventArgs> BeforeCancel { add => beforeCancelRaiser.OnEvent += value; remove => beforeCancelRaiser.OnEvent -= value; }
        public event AsyncEventHandler<EventArgs> AfterCancel { add => afterCancelRaiser.OnEvent += value; remove => afterCancelRaiser.OnEvent -= value; }

        public CancellationToken Token => cancellationTokenSource.Token;

        public async Task TriggerCancellation()
        {
            await beforeCancelRaiser.Raise(EventArgs.Empty);

            cancellationTokenSource.Cancel();

            await afterCancelRaiser.Raise(EventArgs.Empty);
        }
    }
}
