namespace H.Necessaire.Runtime
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ImAPingUseCase>(() => new PingUseCase());
            dependencyRegistry.RegisterAlwaysNew<ImASecurityUseCase>(() => new SecurityUseCase());

            dependencyRegistry.Register<HasherFactory>(() => new HasherFactory());

            dependencyRegistry.Register<Validation.DependencyGroup>(() => new Validation.DependencyGroup());
            dependencyRegistry.Register<Security.DependencyGroup>(() => new Security.DependencyGroup());
        }
        #endregion
    }
}
