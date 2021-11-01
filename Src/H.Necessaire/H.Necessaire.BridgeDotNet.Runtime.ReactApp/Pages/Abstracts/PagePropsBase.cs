namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class PagePropsBase : ComponentPropsBase, ImPageProps
    {
        public UiNavigationParams NavigationParams { get; set; }
    }
}
