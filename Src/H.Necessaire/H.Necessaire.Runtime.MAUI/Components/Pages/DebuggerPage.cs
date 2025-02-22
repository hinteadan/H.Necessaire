using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;

namespace H.Necessaire.Runtime.MAUI.Components.Pages
{
    [Category("Main")]
    class DebuggerPage : HMauiPageBase
    {
        protected override async Task Initialize()
        {
            await base.Initialize();

            Content = new DefaultChrome
            {
                Content = new HMauiDebugger(),
            };
        }
    }
}
