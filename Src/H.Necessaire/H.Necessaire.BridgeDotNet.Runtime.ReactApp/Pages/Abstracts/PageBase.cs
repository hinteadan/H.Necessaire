using Bridge;
using Bridge.Html5;
using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class PageBase<TProps, TState>
        : ComponentBase<TProps, TState>, ImAnAppPage
        where TState : ImAUiComponentState, new()
        where TProps : ImPageProps
    {
        #region Construct
        protected string titlePrefix = string.Empty;

        protected PageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
        #endregion

        public virtual string Title => !string.IsNullOrWhiteSpace(titlePrefix) ? $"{titlePrefix} - {this.GetType().Name.Replace("Page", string.Empty)}" : this.GetType().Name.Replace("Page", string.Empty);

        public override async Task Initialize()
        {
            await base.Initialize();

            titlePrefix = Config.Get("PageTitlePrefix")?.ToString() ?? string.Empty;

            Document.Title = Title;

            await state.Use(props?.NavigationParams ?? UiNavigationParams.None);
        }
    }
}
