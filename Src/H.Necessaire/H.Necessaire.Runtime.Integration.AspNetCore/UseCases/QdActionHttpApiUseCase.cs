namespace H.Necessaire.Runtime.Integration.AspNetCore.UseCases
{
    internal class QdActionHttpApiUseCase : UseCaseBase, ImAnActionQer
    {
        ImAnActionQer actionQer;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            actionQer = dependencyProvider.Get<ImAnActionQer>();
        }

        public async Task<OperationResult> Queue(QdAction action)
        {
            await EnsureAuthentication().ThrowOnFailOrReturn();

            return await actionQer.Queue(action);
        }
    }
}
