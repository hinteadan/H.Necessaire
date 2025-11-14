using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using Microsoft.Maui.Controls.Shapes;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HPhotoSelector : HMauiLabelAndDescriptionComponentBase
    {
        readonly AsyncEventRaiser<PhotoSelectionChangedEventArgs> selectionChangedRaiser;
        public HPhotoSelector() : base()
        {
            selectionChangedRaiser = new AsyncEventRaiser<PhotoSelectionChangedEventArgs>(this);
        }
        public event AsyncEventHandler<PhotoSelectionChangedEventArgs> OnSelectionChanged { add => selectionChangedRaiser.AddHandler(value); remove => selectionChangedRaiser.ZapHandler(value); }

        public Func<CancellationToken, Task<Stream>> DefaultPhotoProvider
        {
            get => state.DefaultPhotoProvider;
            set => CurrentState = state.And(x => x.DefaultPhotoProvider = value);
        }

        State state = new State();
        State CurrentState
        {
            get => state;
            set { ViewData = value.And(x => state = x); RefreshUI(isViewDataIgnored: true); }
        }

        protected override View ConstructLabeledContent()
        {
            return
                new Grid
                {
                    Margin = new Thickness(0, SizingUnit / 4),
                    RowDefinitions = [
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                        new RowDefinition(new GridLength(1, GridUnitType.Auto)),
                    ],
                }
                .And(lay =>
                {
                    lay.Add(BuildPreview(), row: 0);
                    lay.Add(BuildOptions().And(x => x.Margin = new Thickness(0, SizingUnit / 2, 0, 0)), row: 1);
                })
                .And(x =>
                {
                    CurrentState = state;
                })
                ;
        }

        View BuildOptions()
        {
            return
                new Grid
                {
                    ColumnDefinitions = [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                        new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                    ],
                }
                .And(lay =>
                {

                    lay.Add(
                        new HButton
                        {
                            Text = "Set Photo",
                        }
                        .And(btn => btn.Clicked += async (s, e) =>
                        {
                            using (Disable(btn))
                            {
                                List<FileResult> selection = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
                                {
                                    Title = "Select Photo",
                                    SelectionLimit = 1,
                                });

                                CurrentState = state.And(x =>
                                {
                                    x.SelectedPhoto = selection?.SingleOrDefault();
                                });

                                await selectionChangedRaiser.Raise(state.SelectedPhoto);
                            }
                        })
                    , column: 0);

                    lay.Add(
                        new HButton
                        {
                            Text = "Clear Photo",
                            Margin = new Thickness(SizingUnit / 2, 0, 0, 0),
                        }
                        .And(btn => btn.Clicked += async (s, e) =>
                        {
                            using (Disable(btn))
                            {
                                CurrentState = state.And(x =>
                                {
                                    x.SelectedPhoto = null;
                                });

                                await selectionChangedRaiser.Raise(state.SelectedPhoto);
                            }
                        })
                    , column: 1);

                })
                ;
        }

        View BuildPreview()
        {
            return
                new Grid
                {

                }
                .And(lay =>
                {
                    lay.Add(
                        BuildNoSelection()
                        .Bind(this, null, x => x.IsVisible = state?.IsPhotoSelected != true && state?.DefaultPhotoProvider is null)
                    );

                    lay.Add(
                        BuildPhotoPreview()
                        .Bind(this, null, x => x.IsVisible = state?.IsPhotoSelected == true || state?.DefaultPhotoProvider is not null)
                    );
                })
                ;
        }

        View BuildPhotoPreview()
        {
            return new Border
            {
                Content = new Image
                {
                    Source = null,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    Aspect = Aspect.AspectFill,
                }
                .Bind(this, null, x => x.Source = (state?.IsPhotoSelected != true && state?.DefaultPhotoProvider is null) ? null : new StreamImageSource
                {
                    Stream = LoadImageStream,
                })
                ,
                Stroke = Branding.TextColor.ToMaui(),
                StrokeShape = new RoundRectangle { CornerRadius = Branding.SizingUnitInPixels / 1.5 },
                StrokeThickness = 2,
                MinimumWidthRequest = SizingUnit * 8,
                MinimumHeightRequest = SizingUnit * 4.5,
                MaximumHeightRequest = SizingUnit * 9,
            }
            ;
        }

        View BuildNoSelection()
        {
            return
                new Border
                {
                    //VerticalOptions = LayoutOptions.Fill,
                    //HorizontalOptions = LayoutOptions.Fill,
                    Stroke = Branding.TextColor.ToMaui(),
                    StrokeShape = new RoundRectangle { CornerRadius = Branding.SizingUnitInPixels / 1.5 },
                    StrokeThickness = 2,
                    Padding = SizingUnit,
                    MinimumWidthRequest = SizingUnit * 8,
                    MinimumHeightRequest = SizingUnit * 4.5,
                    MaximumHeightRequest = SizingUnit * 9,
                    Content = new HLabel
                    {
                        Text = "No Photo Selected",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Branding.TextColor.ToMaui(),
                    },
                };
        }

        async Task<Stream> LoadImageStream(CancellationToken cancellationToken)
        {
            if (state?.IsPhotoSelected != true && state?.DefaultPhotoProvider is null)
                return Stream.Null;

            if (state?.IsPhotoSelected == true)
                return await state.SelectedPhoto.OpenReadAsync();

            return (await state.DefaultPhotoProvider.Invoke(cancellationToken)) ?? Stream.Null;
        }




        class State
        {
            public FileResult SelectedPhoto { get; set; }
            public bool IsPhotoSelected => SelectedPhoto != null;
            public Func<CancellationToken, Task<Stream>> DefaultPhotoProvider { get; set; }
        }

        public class PhotoSelectionChangedEventArgs : EventArgs
        {
            public PhotoSelectionChangedEventArgs(FileResult selectedPhoto)
            {
                SelectedPhoto = selectedPhoto;
            }

            public FileResult SelectedPhoto { get; }

            public static implicit operator PhotoSelectionChangedEventArgs(FileResult selectedPhoto) => new PhotoSelectionChangedEventArgs(selectedPhoto);
        }
    }
}
