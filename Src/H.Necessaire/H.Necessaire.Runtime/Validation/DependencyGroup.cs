using H.Necessaire.Runtime.Validation.Engines;
using H.Necessaire.Runtime.Validation.Engines.Concrete;

namespace H.Necessaire.Runtime.Validation
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<ImAValidationEngine<UserInfo>>(() => new UserInfoValidationEngine());
        }
    }
}
