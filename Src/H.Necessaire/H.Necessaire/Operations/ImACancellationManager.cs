using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACancellationManager
    {
        event AsyncEventHandler<EventArgs> BeforeCancel;
        event AsyncEventHandler<EventArgs> AfterCancel;

        CancellationToken Token { get; }

        Task TriggerCancellation();
    }
}
