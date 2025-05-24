using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.UI
{
    public class HApp
    {
        public static readonly HApp Default = new HApp();

        const int defaultSizingUnit = 10;
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();

        public ImADependencyRegistry DependencyRegistry => dependencyRegistry;
        public BrandingStyle Branding { get; set; } = BrandingStyle.Default;
        public int SizingUnit => Branding?.SizingUnitInPixels ?? defaultSizingUnit;

        public virtual void Initialize()
        {
            // Initialize the app
        }
    }
}
