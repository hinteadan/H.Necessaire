using H.Necessaire.Models.Branding;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class ComponentPropsBase : ImAUiComponentProps
    {
        protected static BrandingStyle Branding => AppBase.Branding ?? BrandingStyle.Default;
        protected static RuntimeConfig Config => AppBase.Config;
    }
}
