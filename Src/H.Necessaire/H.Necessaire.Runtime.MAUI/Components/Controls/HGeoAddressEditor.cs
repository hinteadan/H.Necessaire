using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HGeoAddressEditor : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HGeoAddressChangedEventArgs> OnGeoAddressChanged;

        GeoAddress geoAddress = new GeoAddress();
        private HKeyValEditor continentEditor;
        private HKeyValEditor countryEditor;
        private HKeyValEditor stateEditor;
        private HKeyValEditor countyEditor;
        private HKeyValEditor cityEditor;
        private HKeyValEditor cityAreaEditor;
        private HTextField zipCodeEditor;
        private HTextField streetAddressEditor;
        private HNoteCollectionEditor addressNotesEditor;

        public GeoAddress GeoAddress
        {
            get => geoAddress;
            set => ViewData = (value ?? new GeoAddress()).RefTo(out geoAddress);
        }

        public bool IsContinentEnabled
        {
            get => continentEditor.IsVisible;
            set => continentEditor.IsVisible = value.AndIf(!value, x => geoAddress.Continent = null);
        }

        public bool IsCountryEnabled
        {
            get => countryEditor.IsVisible;
            set => countryEditor.IsVisible = value.AndIf(!value, x => geoAddress.Country = null);
        }

        public bool IsStateEnabled
        {
            get => stateEditor.IsVisible;
            set => stateEditor.IsVisible = value.AndIf(!value, x => geoAddress.State = null);
        }

        public bool IsCountyEnabled
        {
            get => countyEditor.IsVisible;
            set => countyEditor.IsVisible = value.AndIf(!value, x => geoAddress.County = null);
        }

        public bool IsCityEnabled
        {
            get => cityEditor.IsVisible;
            set => cityEditor.IsVisible = value.AndIf(!value, x => geoAddress.City = null);
        }

        public bool IsCityAreaEnabled
        {
            get => cityAreaEditor.IsVisible;
            set => cityAreaEditor.IsVisible = value.AndIf(!value, x => geoAddress.CityArea = null);
        }

        public bool IsZipCodeEnabled
        {
            get => zipCodeEditor.IsVisible;
            set => zipCodeEditor.IsVisible = value.AndIf(!value, x => geoAddress.ZipCode = null);
        }

        public bool IsAddressNotesEnabled
        {
            get => addressNotesEditor.IsVisible;
            set => addressNotesEditor.IsVisible = value.AndIf(!value, x => geoAddress.Notes = null);
        }

        public string LabelForStreetAddress
        {
            get => streetAddressEditor.Label;
            set => streetAddressEditor.Label = value;
        }

        bool areCodesEditable = true;
        public bool AreCodesEditable
        {
            get => areCodesEditable;
            set
            {
                if (value == areCodesEditable)
                    return;

                areCodesEditable = value;

                continentEditor.IsKeyEditable = value;
                countryEditor.IsKeyEditable = value;
                stateEditor.IsKeyEditable = value;
                countyEditor.IsKeyEditable = value;
                cityEditor.IsKeyEditable = value;
                cityAreaEditor.IsKeyEditable = value;
            }
        }

        protected override View ConstructLabeledContent()
        {
            return new VerticalStackLayout
            {

            }
            .And(lay =>
            {

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "Continent",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out continentEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.Continent, x => x.KeyVal = geoAddress.Continent)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ => { geoAddress.Continent = e.KeyVal; RaiseOnGeoAddressChanged(); }))
                );

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "Country",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out countryEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.Country, x => x.KeyVal = geoAddress.Country)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.Country = e.KeyVal; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "State",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out stateEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.State, x => x.KeyVal = geoAddress.State)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.State = e.KeyVal; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "County",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out countyEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.County, x => x.KeyVal = geoAddress.County)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.County = e.KeyVal; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "City",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out cityEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.City, x => x.KeyVal = geoAddress.City)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.City = e.KeyVal; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HKeyValEditor
                    {
                        Label = "City Area",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                        IsVisible = false,
                    }
                    .RefTo(out cityAreaEditor)
                    .Bind(this, x => x.KeyVal = geoAddress.CityArea, x => x.KeyVal = geoAddress.CityArea)
                    .And(x => x.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.CityArea = e.KeyVal; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HTextField
                    {
                        Label = "Zip Code",
                        Margin = new Thickness(0, 0, 0, SizingUnit / 2),
                    }
                    .RefTo(out zipCodeEditor)
                    .Bind(this, x => x.Text = geoAddress.ZipCode, x => x.Text = geoAddress.ZipCode)
                    .And(x => x.TextChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.ZipCode = e.NewTextValue; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HTextField
                    {
                        Label = "Street Address",
                    }
                    .RefTo(out streetAddressEditor)
                    .Bind(this, x => x.Text = geoAddress.StreetAddress, x => x.Text = geoAddress.StreetAddress)
                    .And(x => x.TextChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.StreetAddress = e.NewTextValue; RaiseOnGeoAddressChanged();
                    }))
                );

                lay.Add(
                    new HNoteCollectionEditor
                    {
                        Margin = new Thickness(0, SizingUnit / 2, 0, 0),
                        Label = "Address Notes",
                        RaiseNoteCollectionChangedOnNoteChange = true,
                    }
                    .RefTo(out addressNotesEditor)
                    .Bind(this, x => x.Notes = geoAddress.Notes, x => x.Notes = geoAddress.Notes)
                    .And(x => x.OnNoteCollectionChanged += (s, e) => IfNotBinding(_ =>
                    {
                        geoAddress.Notes = e.NewNotes; RaiseOnGeoAddressChanged();
                    }))
                );

            });
        }

        void RaiseOnGeoAddressChanged()
        {
            IfNotBinding(_ => OnGeoAddressChanged?.Invoke(this, new HGeoAddressChangedEventArgs(GeoAddress)));
        }
    }

    public class HGeoAddressChangedEventArgs : EventArgs
    {
        public HGeoAddressChangedEventArgs(GeoAddress newGeoAddress)
        {
            NewGeoAddress = newGeoAddress;
        }

        public GeoAddress NewGeoAddress { get; }
    }
}