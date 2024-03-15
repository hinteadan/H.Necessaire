using Bridge.React;
using Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Components.Elements;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class TimeoutProgressIndicator : ComponentBase<TimeoutProgressIndicator.Props, TimeoutProgressIndicator.State>
    {
        #region Construct
        static readonly ColorInfo defaultBackgroundTint = Branding.Colors.PrimaryIsh().Lighter();
        static readonly ColorInfo defaultForegroundTint = Branding.Colors.Complementary.Darker(5);
        static readonly TimeSpan defaultProgressTimeout = TimeSpan.FromSeconds(7);
        ActionRepeater progressRefresher;
        DataNormalizer percentNormalizer;
        public TimeoutProgressIndicator(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected override void EnsureDependencies()
        {
            base.EnsureDependencies();
            progressRefresher = new ActionRepeater(RefreshProgress, TimeSpan.FromSeconds(.1));
            decimal maxMilliseconds = Math.Abs((decimal)(props?.Timeout.TotalMilliseconds ?? 0));
            if (maxMilliseconds == 0)
                maxMilliseconds = (decimal)defaultProgressTimeout.TotalMilliseconds;
            percentNormalizer
                = new DataNormalizer(
                    fromInterval: new NumberInterval(0, maxMilliseconds),
                    toInterval: new NumberInterval(0, 100)
                );
        }

        public override async Task RunAtStartup()
        {
            await base.RunAtStartup();
            state.StartedAt = DateTime.UtcNow;
            await progressRefresher.Start();
        }

        public override async Task Destroy()
        {
            await progressRefresher.Stop();
            await base.Destroy();
        }
        #endregion

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                    }
                    ,
                    (
                        (props?.Label == null && props?.IsPercentageLabelHidden == true) ? null :
                        RenderProgressInfo()
                    )
                    ,
                    RenderProgressBar()
                );
        }

        private ReactElement RenderProgressInfo()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.Flex,
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = (props?.ForegroundTint ?? defaultForegroundTint).ToCssRGBA(),
                            MarginBottom = Branding.SizingUnitInPixels / 3,
                        }
                    },
                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                JustifyContent = JustifyContent.Center,
                            }
                            .FlexNode(isVerticalFlow: true)
                        }
                        ,
                        DOM.Div(
                            new Attributes
                            {
                                Style = new ReactStyle
                                {
                                    Display = Display.Flex,
                                    JustifyContent = JustifyContent.Center,
                                }
                            },
                            (
                                props?.Label == null ? null :
                                DOM.Span(
                                    new Attributes
                                    {
                                        Style = new ReactStyle
                                        {
                                            PaddingRight = Branding.SizingUnitInPixels / 3,
                                        }
                                    }
                                    ,
                                    props.Label
                                )
                            )
                            ,
                            (
                                props?.IsPercentageLabelHidden == true ? null :
                                DOM.Strong(
                                    new Attributes
                                    {

                                    },
                                    $"{Math.Round(state.CurrentProgressPercent, 0)} %"
                                )
                            )
                        )
                    )
                );
        }

        private ReactElement RenderProgressBar()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            BackgroundColor = (props?.BackgroundTint ?? defaultBackgroundTint).Clone().And(x => x.Opacity = .2f).ToCssRGBA(),
                        }
                        .FlexNode()
                        .And(x =>
                        {
                            x.MinHeight = props?.BarHeight ?? (Branding.SizingUnitInPixels / 2);
                        })
                    }
                    ,
                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                Width = $"{Math.Round(state.CurrentProgressPercent, 0)}%",
                                BackgroundColor = (props?.BackgroundTint ?? defaultBackgroundTint).ToCssRGBA(),
                            }
                        }
                    )
                );
        }

        private async Task RefreshProgress()
        {
            await
                DoAndSetStateAsync(state =>
                {
                    state.CurrentProgressPercent = percentNormalizer.Do((decimal)(DateTime.UtcNow - state.StartedAt).TotalMilliseconds);
                });

            if (state.CurrentProgressPercent >= 100)
                await progressRefresher.Stop();
        }



        public class State : ComponentStateBase
        {
            public DateTime StartedAt { get; set; }
            public decimal CurrentProgressPercent { get; set; } = 0;
        }
        public class Props : ComponentPropsBase
        {
            public bool IsPercentageLabelHidden { get; set; } = false;
            public Union<ReactElement, string> Label { get; set; } = null;
            public TimeSpan Timeout { get; set; } = defaultProgressTimeout;
            public ColorInfo BackgroundTint { get; set; } = defaultBackgroundTint;
            public ColorInfo ForegroundTint { get; set; } = defaultForegroundTint;
            public int BarHeight { get; set; } = Branding.SizingUnitInPixels / 2;
        }
    }
}
