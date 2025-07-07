using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HNoteCollectionEditor : HMauiLabelAndDescriptionComponentBase
    {
        public event EventHandler<HNoteCollectionChangedEventArgs> OnNoteCollectionChanged;
        public event EventHandler<HNoteChangedEventArgs> OnNoteChanged;

        private VerticalStackLayout notesViewStack;
        private HGlyphButton addNoteButton;
        readonly List<RefNote> notes = new List<RefNote>();
        public Note[] Notes
        {
            get => notes.Select(x => (Note)x).Where(x => !x.IsEmpty()).ToArrayNullIfEmpty();
            set
            {
                notes.Clear();

                if (value.IsEmpty())
                    return;

                notes.AddRange(value.Where(x => !x.IsEmpty()).Select(x => (RefNote)x));

                if (notesViewStack is not null)
                    RefreshUI();

                RaiseNoteCollectionChanged();
            }
        }

        public bool IsNoteRemovalUserConfirmed { get; set; } = false;
        public bool RaiseNoteCollectionChangedOnNoteChange { get; set; } = false;

        protected override View ConstructLabeledContent()
        {
            return new VerticalStackLayout
            {

            }
            .RefTo(out notesViewStack)
            .And(lay =>
            {
                lay.Add(new HGlyphButton
                {
                    Margin = new Thickness(0, SizingUnit / 2, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    Glyph = "ic_fluent_add_12_filled",
                    GlyphColor = Branding.TextColor.ToMaui(),
                    BackgroundColor = Colors.Transparent,
                    Padding = SizingUnit / 2,
                }
                .RefTo(out addNoteButton)
                .And(btn => btn.Clicked += (s, e) =>
                {
                    using (Disable(btn))
                    {
                        AddNote();
                    }
                }));

            });
        }

        View ConstructNoteEditor(RefNote note, int index)
        {
            return new Grid
            {
                Margin = new Thickness(0, index == 0 ? 0 : SizingUnit / 2, 0, 0),
                ColumnDefinitions = [
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition(new GridLength(1, GridUnitType.Auto)),
                ],
            }
            .And(lay =>
            {

                lay.Add(new HLabel
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = SizingUnit / 2,
                    Text = $"{index + 1}.",
                }, column: 0);

                lay.Add(new HKeyValEditor
                {
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(SizingUnit / 2, 0),
                    PlaceholderForKey = "ID",
                    PlaceholderForValue = "Value",
                    LabelForKey = index == 0 ? "ID" : null,
                    LabelForValue = index == 0 ? "Value" : null,
                    Key = note.ID,
                    Val = note.Value,
                }
                .And(editor => editor.OnKeyValChanged += (s, e) => IfNotBinding(_ =>
                {
                    note.ID = e.Key;
                    note.Value = e.Value;

                    RaiseNoteChanged(note);
                    if (RaiseNoteCollectionChangedOnNoteChange)
                        RaiseNoteCollectionChanged();
                }))
                , column: 1);

                lay.Add(new HGlyphButton
                {
                    VerticalOptions = LayoutOptions.Center,
                    Glyph = "ic_fluent_bin_recycle_24_filled",
                    GlyphColor = Branding.TextColor.ToMaui(),
                    BackgroundColor = Colors.Transparent,
                    Padding = SizingUnit / 2,
                }
                .And(btn => btn.Clicked += async (s, e) =>
                {
                    using (Disable(btn))
                    {
                        if (!note.IsEmpty() && IsNoteRemovalUserConfirmed)
                        {
                            bool isConfirmed = await Application.Current.Windows[0].Page.DisplayAlert("Delete note?", "Are you sure you want to delete the underlying note?", "Yes", "No");
                            if (!isConfirmed)
                                return;
                        }

                        ZapNote(note);
                    }
                })
                , column: 2);

            });

        }

        protected override void RefreshUI(bool isViewDataIgnored = false)
        {
            base.RefreshUI(isViewDataIgnored);

            notesViewStack.Clear();

            if (!notes.IsEmpty())
            {
                int index = -1;
                foreach (RefNote note in notes)
                {
                    index++;
                    notesViewStack.Add(ConstructNoteEditor(note, index));
                }
            }

            notesViewStack.Add(addNoteButton);
        }

        void AddNote()
        {
            int index = notes.Count;
            var note = new RefNote();
            notes.Add(note);
            notesViewStack.Insert(index, ConstructNoteEditor(note, index));

            RaiseNoteCollectionChanged();
        }

        void ZapNote(RefNote note)
        {
            int index = notes.IndexOf(note);
            notes.Remove(note);
            notesViewStack.RemoveAt(index);
            ReIndexNotesStackView();

            RaiseNoteCollectionChanged();
        }

        void ReIndexNotesStackView()
        {
            int index = -1;
            foreach (var child in notesViewStack.Children)
            {
                if (!(child is Grid noteEditorGrid))
                    continue;

                if (!(noteEditorGrid.Children.FirstOrDefault() is HLabel indexLabel))
                    continue;

                index++;

                indexLabel.Text = $"{index + 1}.";

            }
        }

        void RaiseNoteCollectionChanged()
        {
            IfNotBinding(_ => OnNoteCollectionChanged?.Invoke(this, new HNoteCollectionChangedEventArgs(Notes)));
        }

        void RaiseNoteChanged(RefNote note)
        {
            IfNotBinding(_ =>
            {
                int index = notes.IndexOf(note);
                OnNoteChanged?.Invoke(this, new HNoteChangedEventArgs(index, note.ID, note.Value, note));
            });
        }

        private class RefNote
        {
            public string ID { get; set; }
            public string Value { get; set; }

            public bool IsEmpty() => ID.IsEmpty() && Value.IsEmpty();

            public static implicit operator Note(RefNote refNote) => new Note(refNote?.ID, refNote?.Value);
            public static implicit operator RefNote(Note note) => new RefNote { ID = note.ID, Value = note.Value };
        }
    }

    public class HNoteChangedEventArgs : EventArgs
    {
        public HNoteChangedEventArgs(int index, string newID, string newValue, Note newNote)
        {
            Index = index;
            NewID = newID;
            NewValue = newValue;
            NewNote = newNote;
        }

        public int Index { get; }
        public string NewID { get; }
        public string NewValue { get; }
        public Note NewNote { get; }
    }

    public class HNoteCollectionChangedEventArgs : EventArgs
    {
        public HNoteCollectionChangedEventArgs(Note[] newNotes)
        {
            NewNotes = newNotes;
        }

        public Note[] NewNotes { get; }
    }
}
