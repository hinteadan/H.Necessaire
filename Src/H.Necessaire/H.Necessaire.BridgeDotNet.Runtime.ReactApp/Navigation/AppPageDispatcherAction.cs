using ProductiveRage.ReactRouting;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class AppPageDispatcherAction<TPage> : INavigationDispatcherAction
    {
        public UiNavigationParams NavigationParams { get; }

        public AppPageDispatcherAction(UiNavigationParams navigationParams) : base() { NavigationParams = navigationParams ?? UiNavigationParams.None; }

        public AppPageDispatcherAction() : this(UiNavigationParams.None) { }
    }
}
