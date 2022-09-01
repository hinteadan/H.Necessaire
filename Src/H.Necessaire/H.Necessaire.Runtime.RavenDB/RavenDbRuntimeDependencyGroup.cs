using H.Necessaire.RavenDB;

namespace H.Necessaire.Runtime.RavenDB
{
    public class RavenDbRuntimeDependencyGroup : RavenDbDependencyGroup
    {
        #region Constructor
        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            base.RegisterDependencies(dependencyRegistry);

            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;
        }
        #endregion
    }
}
