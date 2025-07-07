using H.Necessaire.Runtime.MAUI.Components.Abstracts;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HKeyValEditor : HMauiLabelAndDescriptionComponentBase
    {
        bool isOnKeyValChangedDisabled = false;
        public event EventHandler<HKeyValChangedEventArgs> OnKeyValChanged;

        private HTextField keyTextField;
        private HTextField valueTextField;

        string key = null;
        public string Key
        {
            get => key;
            set
            {
                bool pre = isOnKeyValChangedDisabled;
                using (new ScopedRunner(_ => isOnKeyValChangedDisabled = true, _ => isOnKeyValChangedDisabled = pre))
                {
                    if (value == key)
                        return;

                    key = value;
                    if (keyTextField is not null)
                        keyTextField.Text = key;
                }

                RaiseOnKeyValChanged();
            }
        }
        string val = null;
        private ColumnDefinition keyColumn;

        public string Val
        {
            get => val;
            set
            {
                bool pre = isOnKeyValChangedDisabled;
                using (new ScopedRunner(_ => isOnKeyValChangedDisabled = true, _ => isOnKeyValChangedDisabled = pre))
                {
                    if (value == val)
                        return;

                    val = value;
                    if (valueTextField is not null)
                        valueTextField.Text = val;
                }

                RaiseOnKeyValChanged();
            }
        }

        public Note KeyVal
        {
            get => new Note(key, val);
            set
            {
                bool pre = isOnKeyValChangedDisabled;
                using (new ScopedRunner(_ => isOnKeyValChangedDisabled = true, _ => isOnKeyValChangedDisabled = pre))
                {
                    if (value.ID == key && value.Value == val)
                        return;

                    Key = value.ID;
                    Val = value.Value;
                }

                RaiseOnKeyValChanged();
            }
        }

        public string LabelForKey
        {
            get => keyTextField.Label;
            set => keyTextField.Label = value;
        }
        public string DescriptionForKey
        {
            get => keyTextField.Description;
            set => keyTextField.Description = value;
        }
        public string PlaceholderForKey
        {
            get => keyTextField.Placeholder;
            set => keyTextField.Placeholder = value;
        }

        public string LabelForValue
        {
            get => valueTextField.Label;
            set => valueTextField.Label = value;
        }
        public string DescriptionForValue
        {
            get => valueTextField.Description;
            set => valueTextField.Description = value;
        }
        public string PlaceholderForValue
        {
            get => valueTextField.Placeholder;
            set => valueTextField.Placeholder = value;
        }

        public bool IsKeyEditable
        {
            get => keyTextField.IsVisible;
            set
            {
                keyTextField.IsVisible = value;
                keyColumn.Width = value ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Auto);
            }
        }

        protected override View ConstructLabeledContent()
        {
            return
                new Grid
                {

                    ColumnDefinitions = [
                        new ColumnDefinition(new GridLength(1, GridUnitType.Star)).RefTo(out keyColumn),
                        new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                    ],

                }
                .And(lay =>
                {

                    lay.Add(
                        new HTextField
                        {
                            Text = key,
                            Margin = new Thickness(0, 0, SizingUnit, 0),
                        }
                        .RefTo(out keyTextField)
                        .And(x => x.TextChanged += (s, e) => IfNotBinding(_ =>
                        {
                            key = e.NewTextValue;
                            RaiseOnKeyValChanged();
                        }))
                    , column: 0);

                    lay.Add(
                        new HTextField
                        {
                            Text = val,
                        }
                        .RefTo(out valueTextField)
                        .And(x => x.TextChanged += (s, e) => IfNotBinding(_ =>
                        {
                            val = e.NewTextValue;
                            RaiseOnKeyValChanged();
                        }))
                    , column: 1);

                });
        }

        void RaiseOnKeyValChanged()
        {
            if (isOnKeyValChangedDisabled)
                return;

            IfNotBinding(_ => OnKeyValChanged?.Invoke(this, new HKeyValChangedEventArgs(key, val)));
        }
    }

    public class HKeyValChangedEventArgs : EventArgs
    {
        public HKeyValChangedEventArgs(string key, string value)
        {
            Key = key;
            Value = value;
            KeyVal = new Note(key, value);
        }
        public HKeyValChangedEventArgs(Note note)
        {
            Key = note.ID;
            Value = note.Value;
            KeyVal = note;
        }


        public string Key { get; }
        public string Value { get; }
        public Note KeyVal { get; }
    }
}
