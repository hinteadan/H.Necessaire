namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class MauiAppUseCaseContextProvider : ImAUseCaseContextProvider, ImADependency
    {
        ConsumerIdentity consumerIdentity;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            consumerIdentity = dependencyProvider.Get<ConsumerIdentity>();
        }

        public Task<UseCaseContext> GetCurrentContext()
        {
            return
                new UseCaseContext
                {
                    Notes = Note.GetEnvironmentInfo().AppendProcessInfo().AppendDeviceInfo(),
                    SecurityContext = new SecurityContext
                    {
                        User = new UserInfo
                        {
                            DisplayName = Environment.UserName,
                            IDTag = Environment.UserDomainName ?? Environment.UserName,
                            Username = Environment.UserName,
                            RuntimePlatform = ConsumerInfo.Platform,
                        },
                    },
                }
                .And(x =>
                {
                    x.OperationContext = new OperationContext
                    {
                        User = x.SecurityContext.User,
                        Consumer = consumerIdentity,
                    };
                })
                .AsTask()
                ;
        }
    }
}
