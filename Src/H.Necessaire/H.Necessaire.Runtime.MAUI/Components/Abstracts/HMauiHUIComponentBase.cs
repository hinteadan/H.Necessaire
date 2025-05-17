using H.Necessaire.Runtime.MAUI.Components.Builders;
using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiHUIComponentBase : HMauiComponentBase
    {
        ImAnHUIComponent hUIComponent;
        ImAHMauiHUIComponentBuilder hMauiComponentBuilder;
        protected HMauiHUIComponentBase(ImAnHUIComponent hUIComponent) : base(hUIComponent) { }

        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies();

            this.hUIComponent = constructionArgs[0] as ImAnHUIComponent;
            this.hUIComponent.ReferenceNativeView(this);
            this.hMauiComponentBuilder = Get<ImAHMauiHUIComponentBuilder>();
        }

        protected override void Construct()
        {
            hUIComponent.Construct();

            base.Construct();
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            await hUIComponent.Initialize();

            await hUIComponent.InitializeAndBindViewModel();
        }

        protected override async Task Destroy()
        {
            await hUIComponent.Destroy();

            await base.Destroy();
        }

        protected override View ConstructContent()
        {
            return hMauiComponentBuilder.BuildComponentFor(hUIComponent);
        }
    }
}
