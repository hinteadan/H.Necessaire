using H.Necessaire.Models.Branding;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class ComponentStateBase : ImAUiComponentState
    {
        protected static BrandingStyle Branding => AppBase.Branding ?? BrandingStyle.Default;
        protected static RuntimeConfig Config => AppBase.Config;

        public virtual Task Initialize() => true.AsTask();
        public virtual Task Use(UiNavigationParams uiNavigationParams) => true.AsTask();
        public virtual Task Destroy() => true.AsTask();
    }
}
