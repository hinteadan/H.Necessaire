using Bridge;
using Bridge.Html5;
using Bridge.React;
using H.Necessaire.Models.Branding;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class ComponentBase<TProps, TState>
        : Component<TProps, TState>, ImAUiComponent
        where TState : ImAUiComponentState, new()
    {
        #region Construct
        int? runAtStartupTimeoutId;
        int? flySafeTimeoutId;
        AlertUserFeedback alertUserFeedback;
        ConfirmUserFeedback confirmUserFeedback;
        RuntimeConfig runtimeConfig;

        protected SecurityContext SecurityContext { get; private set; }
        protected string CurrentLocationPath => Window.Location.Hash.Substring(1);
        protected RuntimeConfig Config => runtimeConfig ?? RuntimeConfig.Empty;
        protected string BaseUrl => AppBase.BaseUrl;
        protected string BaseApiUrl => AppBase.BaseApiUrl;

        protected ComponentBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
        #endregion

        protected static BrandingStyle Branding => AppBase.Branding ?? BrandingStyle.Default;
        protected T Get<T>() => AppBase.Get<T>();
        public virtual bool IsBusy { get; private set; }
        public virtual Task Initialize() => true.AsTask();
        public virtual Task Destroy() => true.AsTask();
        public virtual Task RunAtStartup() => true.AsTask();

        protected override async void ComponentDidMount()
        {
            base.ComponentDidMount();

            await state.Initialize();

            await Initialize();

            runAtStartupTimeoutId = Window.SetTimeout(async () => await RunAtStartup());
        }

        protected override async void ComponentWillUnmount()
        {
            if (runAtStartupTimeoutId != null)
                Window.ClearTimeout(runAtStartupTimeoutId.Value);

            if (flySafeTimeoutId != null)
                Window.ClearTimeout(flySafeTimeoutId.Value);

            await state.Destroy();

            await Destroy();

            base.ComponentWillUnmount();
        }

        protected virtual void EnsureDependencies()
        {
            alertUserFeedback = new AlertUserFeedback();
            confirmUserFeedback = new ConfirmUserFeedback();
            SecurityContext = Get<SecurityContext>();
            runtimeConfig = AppBase.Config;
        }

        protected override TState GetInitialState()
        {
            TState state = base.GetInitialState();

            EnsureDependencies();

            if (state != null)
                return state;

            state = Get<TState>();

            if (state != null)
                return state;

            state = new TState();

            return state;
        }

        #region State Management
        protected void SetState() => SetState(state);
        protected Task SetStateAsync() => SetStateAsync(state);
        protected async Task DoAndSetStateAsync(Action<TState> doThis)
        {
            doThis?.Invoke(state);
            await SetStateAsync();
        }
        protected async Task DoAndSetStateAsync(Func<TState, Task> doThis)
        {
            await doThis?.Invoke(state);
            await SetStateAsync();
        }
        protected void DoAndSetState(Action<TState> doThis)
        {
            doThis?.Invoke(state);
            SetState();
        }
        #endregion

        #region User Feedback
        protected Task Alert(params string[] messages) => alertUserFeedback.Go(messages);
        protected Task<bool> Confirm(params string[] messages) => confirmUserFeedback.Go(messages);
        #endregion

        #region Execution Utils
        protected ScopedRunner BusyFlag(bool toggleBusyCursor = true)
        {
            return
                new ScopedRunner(
                    onStart: () => DoAndSetState(_ =>
                    {
                        IsBusy = true;
                        if (toggleBusyCursor)
                            React.Render(new BusyCurtain(), AppBase.CurtainContainer);
                    }),
                    onStop: () => DoAndSetState(_ =>
                    {
                        React.UnmountComponentAtNode(AppBase.CurtainContainer);
                        IsBusy = false;
                    })
                );
        }

        protected void FlySafe(Action withThis)
        {
            if (withThis == null)
                return;

            flySafeTimeoutId = Window.SetTimeout(withThis);
        }
        #endregion



        //protected async Task CopyToClipboard(string text)
        //{
        //    await clipboardResource.Write(text);
        //}

        //protected Task Alert(params string[] messages) => alertUserFeedback.Go(messages);
        //protected Task<bool> Confirm(params string[] messages) => confirmUserFeedback.Go(messages);
    }
}
