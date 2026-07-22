using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.UseCases
{
    public static class HUseCase
    {
        public static async Task<T> AsExternal<T>(Func<Task<T>> useCase)
        {
            using (new ExternalUseCaseCallScope())
            {
                return await useCase.Invoke();
            }
        }

        public static async Task AsExternal(Func<Task> useCase)
        {
            using (new ExternalUseCaseCallScope())
            {
                await useCase.Invoke();
            }
        }
    }
}
