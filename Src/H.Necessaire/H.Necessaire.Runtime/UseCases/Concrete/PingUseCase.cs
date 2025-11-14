using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    class PingUseCase : UseCaseBase, ImAPingUseCase
    {
        public Task<string> Pong()
        {
            return $"Pong @ {DateTime.Now}".AsTask();
        }

        public async Task<string> SecuredPong()
        {
            UseCaseContext context = (await EnsureAuthentication()).ThrowOnFailOrReturn();

            return $"Pong by {(context.SecurityContext.User?.ToString() ?? $"[Unknown User - {context.ID}]")} @ {DateTime.Now}";
        }
    }
}
