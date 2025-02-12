namespace H.Necessaire.Runtime.MAUI.Core
{
    internal class MauiAppToUseCaseContextProvider : ImAUseCaseContextProvider, ImADependency
    {
        public Task<UseCaseContext> GetCurrentContext()
        {
            throw new NotImplementedException();
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            throw new NotImplementedException();
        }
    }
}
