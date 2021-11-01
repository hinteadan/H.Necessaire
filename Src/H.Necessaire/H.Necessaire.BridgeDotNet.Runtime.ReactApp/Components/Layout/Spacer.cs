using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Spacer : ComponentBase<Spacer.Props, Spacer.State>
    {
        public Spacer(Props props) : base(props, null) { }
        public Spacer() : base(new Props(), null) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle()
                            .And(x =>
                            {
                                int size = Branding.SizingUnitInPixels * (props?.Scale ?? 1);
                                if (props.IsVertical)
                                    x.Width = size;
                                else
                                    x.Height = size;
                            })

                    }
                );
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase
        {
            public int Scale { get; set; } = 1;
            public bool IsVertical { get; set; } = false;
        }
    }
}
