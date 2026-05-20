using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImALimitedConcurrencyRunner
    {
        Task Run(Func<Task> actionToRun);
        Task<OperationResult<IDisposable>> WaitIfNecessary();
    }
}
