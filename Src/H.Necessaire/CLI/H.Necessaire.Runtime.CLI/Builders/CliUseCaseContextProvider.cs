using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.Builders
{
    public sealed class CliUseCaseContextProvider : ImAUseCaseContextProvider, ImADependency
    {
        #region Construct
        ArgsParser argsParser = new ArgsParser();

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            argsParser = dependencyProvider.Get<ArgsParser>();
        }
        #endregion

        public async Task<UseCaseContext> GetCurrentContext()
        {
            return
                new UseCaseContext
                {
                    Notes = await argsParser.Parse(Environment.GetCommandLineArgs().Jump(1)),
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
