using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Switch : ComponentBase<Switch.Props, Switch.State>
    {
        public Switch(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Switch(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                MinHeight = Branding.SizingUnitInPixels,
                Width = props.Width,
            };

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = reactStyle.FlexNode(),
                    },

                    RenderSwitch(),

                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle { Height = "100%", PaddingLeft = 5 }.FlexNode()
                        },

                        Children

                    )
                );
        }

        private ReactElement RenderSwitch()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Height = Branding.SizingUnitInPixels,
                            Width = Branding.SizingUnitInPixels * 2,
                            Border = "solid 1px",
                            BorderColor = Branding.Colors.Primary.Color.ToCssRGBA(),
                            Position = Bridge.Html5.Position.Relative,
                            Cursor = props.IsDisabled ? Bridge.Html5.Cursor.NotAllowed : Bridge.Html5.Cursor.Pointer,
                            Opacity = props.IsDisabled ? "0.25" : null,
                        },
                        OnClick = x => { if (props.IsDisabled) return; ToggleSwitch(); },
                    },
                    RenderSwitchKnob()
                );
        }

        private ReactElement RenderSwitchKnob()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                Height = Branding.SizingUnitInPixels - 2,
                Width = Branding.SizingUnitInPixels - 2,
                Position = Bridge.Html5.Position.Absolute,
                Top = 1,
                Right = 1,
                BackgroundColor = Branding.Colors.Primary.Color.ToCssRGBA(),
            };

            if (state.IsOff)
            {
                reactStyle.Right = null;
                reactStyle.Left = 1;
                reactStyle.BackgroundColor = Branding.Colors.Primary.Lighter(2).ToCssRGBA();
            }

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = reactStyle,
                    }
                );
        }

        private void ToggleSwitch()
        {
            state.IsOn = !state.IsOn;
            SetState();
            props.OnChange?.Invoke(state.IsOn);
        }

        protected override State GetInitialState()
        {
            State state = base.GetInitialState();
            state.IsOn = props.IsOn;
            return state;
        }

        protected override void ComponentWillReceiveProps(Props nextProps)
        {
            base.ComponentWillReceiveProps(nextProps);

            state.IsOn = nextProps.IsOn;
            SetState();
        }

        public class State : ComponentStateBase
        {
            public bool IsOn { get; set; } = false;
            public bool IsOff => !IsOn;
        }
        public class Props : ComponentPropsBase
        {
            public bool IsDisabled { get; set; } = false;
            public bool IsOn { get; set; } = false;
            public bool IsOff => !IsOn;
            public Union<string, int> Width { get; set; } = 200;
            public Action<bool> OnChange { get; set; }
        }
    }
}
