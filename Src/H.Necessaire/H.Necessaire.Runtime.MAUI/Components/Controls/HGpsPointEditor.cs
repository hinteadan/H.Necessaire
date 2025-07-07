using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Components.Layouts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HGpsPointEditor : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HGpsPointChangedEventArgs> OnGpsPointChanged;

        protected override void EnsureDependencies(params object[] constructionArgs)
        {
            base.EnsureDependencies(constructionArgs);
            GpsPoint = (46.612783, 23.435114);
        }

        View degreesEditor;
        View dmsEditor;
        HGpsPointEditorMode mode = HGpsPointEditorMode.Degrees;
        private HButton editorModeSwitchBtn;

        public HGpsPointEditorMode Mode
        {
            get => mode;
            set
            {
                if (value == mode)
                    return;

                mode = value;

                if (mode == HGpsPointEditorMode.Degrees)
                {
                    dmsEditor.IsVisible = false;
                    degreesEditor.IsVisible = true;
                    editorModeSwitchBtn.Text = "Switch To D°M'S\"";
                }
                else
                {
                    degreesEditor.IsVisible = false;
                    dmsEditor.IsVisible = true;
                    editorModeSwitchBtn.Text = "Switch to Degrees";
                }
            }
        }

        GeoDmsCoordinates dmsCoordinates;
        public GpsPoint GpsPoint
        {
            get => (GpsPoint)ViewData;
            set => ViewData = value.And(x => dmsCoordinates = value);
        }

        protected override View ConstructLabeledContent()
        {
            return
                new Grid
                {
                    RowDefinitions = [
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                    ],
                }
                .And(lay =>
                {

                    lay.Add(BuildDegreesEditor().RefTo(out degreesEditor));

                    lay.Add(BuildDMSEditor().RefTo(out dmsEditor));

                    lay.Add(new HButton
                    {
                        Margin = new Thickness(0, SizingUnit / 2, 0, 0),
                        HorizontalOptions = LayoutOptions.Start,
                        BackgroundColor = Colors.Transparent,
                        TextColor = Branding.TextColor.ToMaui(),
                        FontSize = Branding.Typography.FontSizeSmall,
                        Text = mode == HGpsPointEditorMode.Degrees ? "Switch To D°M'S\"" : "Switch to Degrees",
                    }
                    .RefTo(out editorModeSwitchBtn)
                    .And(btn => btn.Clicked += (s, e) =>
                    {
                        Mode = mode == HGpsPointEditorMode.Degrees ? HGpsPointEditorMode.DMS : HGpsPointEditorMode.Degrees;
                    }), row: 1);

                });
        }

        View BuildDegreesEditor()
        {
            return new Grid
            {
                IsVisible = mode == HGpsPointEditorMode.Degrees,
            }
            .And(lay =>
            {

                lay.Add(new HVerticalStack
                {
                    Views = [

                        new HNumberEditor
                        {
                            Label = "Latitude in degreees",
                            Description = "E.g.: 46.612893",
                            Min = -90,
                            Max = 90,
                            IncrementUnit = 0.0005,
                        }
                        .Bind(this, null, x => x.Number = GpsPoint.LatInDegrees)
                        .And(x => x.NumberChanged += (s, e) => IfNotBinding(_ => {
                            if (e.NewValue is null)
                                return;
                            GpsPoint = (e.NewValue.Value, GpsPoint.LngInDegrees);
                            RaiseGpsPointChanged();
                        }))
                        ,

                        new HNumberEditor
                        {
                            Margin = new Thickness(0, SizingUnit / 2, 0, 0),
                            Label = "Longitude in degreees",
                            Description = "E.g.: 23.434951",
                            Min = -180,
                            Max = 180,
                            IncrementUnit = 0.0005,
                        }
                        .Bind(this, null, x => x.Number = GpsPoint.LngInDegrees)
                        .And(x => x.NumberChanged += (s, e) => IfNotBinding(_ => {
                            if (e.NewValue is null)
                                return;
                            GpsPoint = (GpsPoint.LatInDegrees, e.NewValue.Value);
                            RaiseGpsPointChanged();
                        }))
                        ,

                    ],
                });

            });
        }
        View BuildDMSEditor()
        {
            return new Grid
            {
                IsVisible = mode == HGpsPointEditorMode.DMS,
            }
            .And(lay =>
            {

                lay.Add(new HVerticalStack
                {
                    Views =
                    [
                        BuildDMSCoordinateEditor(isForLng: false),
                        BuildDMSCoordinateEditor(isForLng: true),
                    ],
                });

            });
        }

        View BuildDMSCoordinateEditor(bool isForLng)
        {
            return new Grid
            {
                Margin = new Thickness(0, isForLng ? SizingUnit / 2 : 0, 0, 0),
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                ],
            }
            .And(lay =>
            {

                lay.Add(new HNumberEditor(isStepperHidden: true)
                {
                    Label = isForLng ? null : "°",
                    Description = isForLng ? "Deg" : null,
                    Min = 0,
                    Max = isForLng ? 179 : 89,
                    IncrementUnit = 1,
                }
                .Bind(this, null, x => x.Number = isForLng ? dmsCoordinates.Lng.Degrees : dmsCoordinates.Lat.Degrees)
                .And(x => x.NumberChanged += (s, e) => IfNotBinding(_ =>
                {
                    if (e.NewValue is null)
                        return;
                    GpsPoint
                        = isForLng
                        ? (dmsCoordinates.Lat, ((int)e.NewValue.Value, dmsCoordinates.Lng.Minutes, dmsCoordinates.Lng.Seconds, dmsCoordinates.Lng.Direction))
                        : (((int)e.NewValue.Value, dmsCoordinates.Lat.Minutes, dmsCoordinates.Lat.Seconds, dmsCoordinates.Lat.Direction), dmsCoordinates.Lng)
                        ;
                    RaiseGpsPointChanged();
                }))
                , column: 0);

                lay.Add(new HNumberEditor(isStepperHidden: true)
                {
                    Label = isForLng ? null : "'",
                    Description = isForLng ? "Min" : null,
                    Min = 0,
                    Max = 59,
                    IncrementUnit = 1,
                }
                .Bind(this, null, x => x.Number = isForLng ? dmsCoordinates.Lng.Minutes : dmsCoordinates.Lat.Minutes)
                .And(x => x.NumberChanged += (s, e) => IfNotBinding(_ =>
                {
                    if (e.NewValue is null)
                        return;
                    GpsPoint
                        = isForLng
                        ? (dmsCoordinates.Lat, (dmsCoordinates.Lng.Degrees, (int)e.NewValue.Value, dmsCoordinates.Lng.Seconds, dmsCoordinates.Lng.Direction))
                        : ((dmsCoordinates.Lat.Degrees, (int)e.NewValue.Value, dmsCoordinates.Lat.Seconds, dmsCoordinates.Lat.Direction), dmsCoordinates.Lng)
                        ;
                    RaiseGpsPointChanged();
                }))
                , column: 1);

                lay.Add(new HNumberEditor(isStepperHidden: true)
                {
                    Label = isForLng ? null : "\"",
                    Description = isForLng ? "Sec" : null,
                    Min = 0,
                    Max = 59,
                    IncrementUnit = 0.5,
                }
                .Bind(this, null, x => x.Number = isForLng ? dmsCoordinates.Lng.Seconds : dmsCoordinates.Lat.Seconds)
                .And(x => x.NumberChanged += (s, e) => IfNotBinding(_ =>
                {
                    if (e.NewValue is null)
                        return;
                    GpsPoint
                        = isForLng
                        ? (dmsCoordinates.Lat, (dmsCoordinates.Lng.Degrees, dmsCoordinates.Lng.Minutes, e.NewValue.Value, dmsCoordinates.Lng.Direction))
                        : ((dmsCoordinates.Lat.Degrees, dmsCoordinates.Lat.Minutes, e.NewValue.Value, dmsCoordinates.Lat.Direction), dmsCoordinates.Lng)
                        ;
                    RaiseGpsPointChanged();
                }))
                , column: 2);

                lay.Add(new HPicker
                {
                    Label = isForLng ? null : "^",
                    Description = isForLng ? "Dir" : null,
                    SelectedIndex = 0,
                }
                .SetDataSource<string>(isForLng ? ["E", "W"] : ["N", "S"])
                .And(x => x.SelectedIndex = 0)
                .Bind(this, null, x => x.SelectedIndex = isForLng ? (dmsCoordinates.Lng.Direction == GeoDmsLngDirection.East ? 0 : 1) : (dmsCoordinates.Lat.Direction == GeoDmsLatDirection.North ? 0 : 1))
                .And(x => x.SelectedIndexChanged += (s, e) => IfNotBinding(_ =>
                {
                    GpsPoint
                        = isForLng
                        ? (dmsCoordinates.Lat, (dmsCoordinates.Lng.Degrees, dmsCoordinates.Lng.Minutes, dmsCoordinates.Lng.Seconds, x.SelectedIndex == 0 ? GeoDmsLngDirection.East : GeoDmsLngDirection.West))
                        : ((dmsCoordinates.Lat.Degrees, dmsCoordinates.Lat.Minutes, dmsCoordinates.Lat.Seconds, x.SelectedIndex == 0 ? GeoDmsLatDirection.North : GeoDmsLatDirection.South), dmsCoordinates.Lng)
                        ;
                }))
                , column: 3);

            })
            .WithLabelAndDescription(
                label: isForLng ? null : "Latitude and Longitude in D°M'S\"",
                description: isForLng ? "(Degress(°) Minutes(') Seconds(\"). E.g.: 46°36'46.2\"N or 23°26'06.1\"E" : null
            )
            ;
        }

        void RaiseGpsPointChanged()
        {
            IfNotBinding(_ => OnGpsPointChanged?.Invoke(this, new HGpsPointChangedEventArgs(GpsPoint)));
        }
    }

    public enum HGpsPointEditorMode : byte
    {
        Degrees = 0,
        DMS = 1,
        DegreesMinutesSeconds = 1,
    }

    public class HGpsPointChangedEventArgs : EventArgs
    {
        public HGpsPointChangedEventArgs(GpsPoint newGpsPoint)
        {
            NewGpsPoint = newGpsPoint;
        }

        public GpsPoint NewGpsPoint { get; }
    }
}
