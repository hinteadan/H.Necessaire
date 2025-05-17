using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Chromes;

namespace H.Necessaire.Runtime.MAUI.Components.Pages
{
    [Category("Main")]
    class DebuggerPage : HMauiPageBase
    {
        public DebuggerPage() : base(isHeavyInitializer: true) { }

        protected override View ConstructContent()
        {
            return new DefaultChrome
            {
                Content = new HMauiDebugger(),
            };
        }
    }
}
