using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class LoginPage : PageBase<LoginPage.Props, LoginPage.State>
    {
        #region Construct
        SecurityManager securityManager;

        public LoginPage() : base(new Props(), null) { }
        public LoginPage(Props props) : base(props, null) { }


        public override async Task Initialize()
        {
            await base.Initialize();

            state.ReturnTo = props.NavigationParams.GetValue<string>();
        }

        protected override void EnsureDependencies()
        {
            base.EnsureDependencies();

            securityManager = Get<SecurityManager>();
        }

        public override Task RunAtStartup()
        {
            if (SecurityContext != null)
                FlySafe(() => Navi.GoHome());

            jQuery.Select($"input[name='{nameof(state.User)}']").Focus();

            return true.AsTask();
        }
        #endregion

        public override ReactElement Render()
        {
            return
                new DefaultChrome(

                    new CenteredContent(

                        RenderLoginForm()

                    )

                );
        }

        private ReactElement RenderLoginForm()
        {
            return
                DOM.Form(

                    new FormAttributes
                    {
                        OnSubmit = async x => { x.PreventDefault(); await TriggerLogin(); },
                        Style = new ReactStyle { }.FlexNode(),
                    },

                    new FormLayout(

                        new FormLayout.Props { LayoutMode = FormLayoutMode.OnePerRowSmall, RowSpacing = Branding.SizingUnitInPixels * 2 },

                        DOM.Div(

                            new Attributes
                            {
                                Style = new ReactStyle().FlexNode(isVerticalFlow: true)
                            },

                            DOM.Label(new LabelAttributes { HtmlFor = nameof(state.User), Style = new ReactStyle { MarginLeft = Branding.SizingUnitInPixels } }, "Username: "),

                            DOM.Input(new InputAttributes
                            {
                                Name = nameof(state.User),
                                Type = InputType.Text,
                                Disabled = IsBusy,
                                OnChange = x => DoAndSetState(state => state.User = x.CurrentTarget.Value)
                            })
                        )
                        ,

                        DOM.Div(

                            new Attributes
                            {
                                Style = new ReactStyle().FlexNode(isVerticalFlow: true)
                            },

                            DOM.Label(new LabelAttributes { HtmlFor = nameof(state.User), Style = new ReactStyle { MarginLeft = Branding.SizingUnitInPixels } }, "Password: "),

                            DOM.Input(new InputAttributes
                            {
                                Name = nameof(state.Pass),
                                Type = InputType.Password,
                                Disabled = IsBusy,
                                OnChange = x => DoAndSetState(state => state.Pass = x.CurrentTarget.Value)
                            })
                        )
                        ,

                        DOM.Div(

                            new Attributes
                            {
                                Style = new ReactStyle().FlexNode(isVerticalFlow: true)
                            },

                            DOM.Button(

                                new ButtonAttributes
                                {
                                    Disabled = IsBusy,
                                    Type = ButtonType.Submit,
                                },

                                "Login"
                            )
                        )

                        ,

                        RenderAuthResultIfNecesarry()
                    )

                );
        }

        private ReactElement RenderAuthResultIfNecesarry()
        {
            if (state.AuthResult?.IsSuccessful ?? true)
                return null;

            return
                DOM.Div(

                    new Attributes
                    {
                        Style = new ReactStyle().FlexNode(isVerticalFlow: true)
                    },

                    new OperationResultCard(new OperationResultCard.Props { OperationResult = state.AuthResult, Width = "100%" })

                );
        }

        private async Task TriggerLogin()
        {
            using (BusyFlag())
            {
                state.AuthResult = await securityManager.AuthenticateCredentials(state.User, state.Pass);

                if (!state.AuthResult.IsSuccessful)
                    return;
            }

            FlySafe(() => Navi.Go(state.ReturnTo));
        }


        public class State : PageStateBase
        {
            public string ReturnTo { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public OperationResult<SecurityContext> AuthResult { get; set; }
        }


        public class Props : PagePropsBase { }
    }
}
