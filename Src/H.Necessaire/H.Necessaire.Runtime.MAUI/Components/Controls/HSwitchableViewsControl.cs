using H.Necessaire;
using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HSwitchableViewsControl : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HSwitchableViewsSwitchEventArgs> OnSwitch;

        View mainView = null;
        public View MainView
        {
            get => mainView;
            set
            {
                if (mainView == value)
                    return;

                rootLayGrid.Remove(mainView);
                mainView = value;
                mainView.IsVisible = isMainViewVisible;
                rootLayGrid.Add(mainView, column: 0);
            }
        }

        View altView = null;
        public View AltView
        {
            get => altView;
            set
            {
                if (altView == value)
                    return;

                rootLayGrid.Remove(altView);
                altView = value;
                altView.IsVisible = !isMainViewVisible;
                rootLayGrid.Add(altView, column: 0);
            }
        }

        bool isMainViewVisible = true;
        public bool IsMainViewVisible
        {
            get => isMainViewVisible;
            set
            {
                if (isMainViewVisible == value)
                    return;

                isMainViewVisible = value;

                if (mainView is not null)
                    mainView.IsVisible = isMainViewVisible;
                if (altView is not null)
                    altView.IsVisible = !isMainViewVisible;

                switchButton.Glyph = isMainViewVisible ? mainVisibleGlyph : altVisibleGlyph;
            }
        }

        string mainVisibleGlyph = "ic_fluent_filter_24_filled";
        string altVisibleGlyph = "ic_fluent_filter_dismiss_24_filled";

        public string MainVisibleGlyph
        {
            get => mainVisibleGlyph;
            set => mainVisibleGlyph = value.AndIf(isMainViewVisible, x => switchButton.Glyph = x);
        }
        public string AltVisibleGlyph
        {
            get => altVisibleGlyph;
            set => altVisibleGlyph = value.AndIf(!isMainViewVisible, x => switchButton.Glyph = x);
        }

        Grid rootLayGrid = null;
        HGlyphButton switchButton = null;

        protected override View ConstructLabeledContent()
        {
            return new Grid
            {
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                ],
            }
            .RefTo(out rootLayGrid)


            .And(lay =>
            {

                lay.Add(
                    new HGlyphButton
                    {
                        Margin = new Thickness(SizingUnit / 2, 0, 0, 0),
                        Padding = SizingUnit / 3,
                        Glyph = mainVisibleGlyph,
                        GlyphSize = Branding.Typography.FontSize,
                        GlyphColor = Branding.TextColor.ToMaui(),
                        BackgroundColor = Colors.Transparent,
                    }
                    .RefTo(out switchButton)
                    .And(btn => btn.Clicked += (s, e) =>
                    {
                        IsMainViewVisible = !IsMainViewVisible;
                        IfNotBinding(_ => RaiseOnSwitch());
                    })
                , column: 1);

            });
        }

        protected override void RefreshUI(bool isViewDataIgnored = false)
        {
            base.RefreshUI(isViewDataIgnored: true);
        }

        void RaiseOnSwitch()
        {
            if (IsBinding || IsPageBinding)
                return;

            OnSwitch?.Invoke(this, new HSwitchableViewsSwitchEventArgs(isMainViewVisible));
        }
    }

    public class HSwitchableViewsSwitchEventArgs : EventArgs
    {
        public HSwitchableViewsSwitchEventArgs(bool isMainViewVisible)
        {
            IsMainViewVisible = isMainViewVisible;
        }

        public bool IsMainViewVisible { get; }
    }
}
