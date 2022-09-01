using H.Necessaire.Runtime.Analytics.Managers;
using H.Necessaire.Runtime.Analytics.Managers.Concrete;

namespace H.Necessaire.Runtime.Analytics
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<Resources.DependencyGroup>(() => new Resources.DependencyGroup())

                .Register<ImAnAnalyticsManager>(() => new AnalyticsManager())
                ;
        }
    }
}
