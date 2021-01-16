using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAPeriodicAction
    {
        void Start(TimeSpan interval, Func<Task> action);

        void StartDelayed(TimeSpan delay, TimeSpan interval, Func<Task> action);

        void Stop();
    }

    public static class PeriodicAction
    {
        public static readonly ImAPeriodicAction Instance = new Operations.Concrete.NetStandardPeriodicAction();
    }
}
