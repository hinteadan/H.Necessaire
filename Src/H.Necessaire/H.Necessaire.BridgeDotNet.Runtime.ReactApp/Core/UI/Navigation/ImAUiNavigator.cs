using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImAUiNavigator
    {
        void Bind<TViewState>(Action<UiNavigationParams> platformSpecificNavigationInvoker) where TViewState : ImAUiComponentState;
        void Go<T>(UiNavigationParams uiNavigationParams = null);
    }
}
