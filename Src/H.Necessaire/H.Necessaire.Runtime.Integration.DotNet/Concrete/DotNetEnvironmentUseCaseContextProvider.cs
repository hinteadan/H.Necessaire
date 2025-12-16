using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.DotNet.Concrete
{
    internal sealed class DotNetEnvironmentUseCaseContextProvider : ImAUseCaseContextProvider
    {
        public async Task<UseCaseContext> GetCurrentContext()
        {
            return
                new UseCaseContext
                {
                    Notes = Environment.GetCommandLineArgs().ToNotes("Args::"),
                    SecurityContext = new SecurityContext
                    {
                        User = new UserInfo
                        {
                            DisplayName = Environment.UserName,
                            IDTag = Environment.UserDomainName ?? Environment.UserName,
                            Username = Environment.UserName,
                        },
                    },
                }
                .And(x =>
                {
                    x.OperationContext = new OperationContext { User = x.SecurityContext.User };
                })
                ;
        }
    }
}
