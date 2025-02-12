using H.Necessaire.Runtime.UI.Concrete;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI.Abstractions
{
    public abstract class HUIComponentBase : ImAnHUIComponent, ImADependency
    {
        public virtual HViewModel ViewModel { get; }
        public virtual object NativeViewReference { get; private set; }
        ImAnHViewModelBinder viewModelBinder;
        ImAnHUILabelProvider labelProvider = DefaultHUILabelProvider.Instance;
        ImAnHUIRefresher hUIRefresher;
        protected HUIComponentBase(HViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            viewModelBinder = dependencyProvider.Get<ImAnHViewModelBinder>();
            hUIRefresher = dependencyProvider.Get<ImAnHUIRefresher>();
            labelProvider = dependencyProvider.Get<ImAnHUILabelProvider>() ?? DefaultHUILabelProvider.Instance;
            DependencyProvider = dependencyProvider;
            Logger = dependencyProvider.GetLogger(GetType(), application: "H.Necessaire.Runtime.UI");

            Construct();
        }

        public virtual void Construct()
        {
            ViewModel.WithLabelProvider(labelProvider);
            ViewModel.OnViewModelChanged += OnViewModelChanged;
        }

        public Task Destroy()
        {
            ViewModel.OnViewModelChanged -= OnViewModelChanged;
            return Task.CompletedTask;
        }

        public virtual Task Initialize() => Task.CompletedTask;

        public virtual Task InitializeAndBindViewModel()
        {
            if (!(viewModelBinder is null))
                viewModelBinder.Bind(ViewModel);

            return Task.CompletedTask;
        }

        public virtual void ReferenceNativeView(object nativeViewReference)
        {
            NativeViewReference = nativeViewReference;
        }

        public virtual async Task RefreshUI()
        {
            if (hUIRefresher is null)
                return;

            await hUIRefresher.RefreshUI(NativeViewReference, ViewModel, CallContext<HViewModelChangedEventArgs>.GetData(nameof(RefreshUI)));
        }
        protected HViewModelChangedEventArgs GetCurrentViewModelChangedEventArgs()
            => CallContext<HViewModelChangedEventArgs>.GetData(nameof(RefreshUI));

        protected ImALogger Logger { get; private set; }
        protected ImADependencyProvider DependencyProvider { get; private set; }
        protected T Get<T>() => DependencyProvider is null ? default : DependencyProvider.Get<T>();
        protected T Build<T>(string id, params Assembly[] assembliesToScan) where T : class => DependencyProvider is null ? default : DependencyProvider.Build<T>(id, defaultTo: default, assembliesToScan);


        protected virtual async Task OnViewModelChanged(object sender, HViewModelChangedEventArgs e)
        {
            using (new ScopedRunner(
                () => CallContext<HViewModelChangedEventArgs>.SetData(nameof(RefreshUI), e),
                () => CallContext<HViewModelChangedEventArgs>.ZapData(nameof(RefreshUI))
                ))
            {
                await RefreshUI();
            }                
        }
    }
}
