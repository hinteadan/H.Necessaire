using H.Necessaire.Runtime.MAUI.Components;
using H.Necessaire.Runtime.MAUI.Components.Chromes;
using H.Necessaire.Runtime.MAUI.Components.Controls;

namespace H.Necessaire.Runtime.MAUIAppSample;

public class DebuggerPage : ContentPage
{
    public DebuggerPage()
    {
        Content = new DefaultChrome
        {
            Content = new HMauiDebugger(),
        };
    }
}