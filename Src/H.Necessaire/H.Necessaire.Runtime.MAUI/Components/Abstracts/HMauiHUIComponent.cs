using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.MAUI.Components.Abstracts
{
    public abstract class HMauiHUIComponent : HMauiComponent
    {
        readonly ImAnHUIComponent hUIComponent;
        readonly ImAHMauiComponentBuilder hMauiComponentBuilder;
        protected HMauiHUIComponent(ImAnHUIComponent hUIComponent) : base(isCustomConstruct: true)
        {
            this.hUIComponent = hUIComponent;
            this.hUIComponent.ReferenceNativeView(this);
            this.hMauiComponentBuilder = Get<ImAHMauiComponentBuilder>();

            Construct();
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

        protected override View ConstructDefaultContent()
        {
            return hMauiComponentBuilder.BuildComponentFor(hUIComponent.ViewModel);
        }
    }
}
