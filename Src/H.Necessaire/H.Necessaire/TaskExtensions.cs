using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class TaskExtensions
    {
        public static Task<T> AsTask<T>(this T value) => Task.FromResult(value);
        public static Func<Task> AsAsync(this Action action)
        {
            return new Func<Task>(() => { action?.Invoke(); return Task.FromResult(true); });
        }
    }
}
