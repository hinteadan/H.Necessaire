namespace H.Necessaire.Runtime
{
    public class DefaultDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ImAPingUseCase>(() => new PingUseCase());
            dependencyRegistry.RegisterAlwaysNew<ImASecurityUseCase>(() => new SecurityUseCase());
        }
        #endregion
    }
}
