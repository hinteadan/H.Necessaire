using System;

namespace H.Necessaire
{
    public abstract class IDentityBase : IDentity
    {
        public virtual Guid ID { get; set; } = Guid.NewGuid();

        public virtual string IDTag { get; set; }

        public virtual string DisplayName { get; set; }

        public Note[] Notes { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
                return DisplayName;

            if (!string.IsNullOrWhiteSpace(IDTag))
                return IDTag;

            return ID.ToString();
        }

        protected string GetNoteValueFor(string noteId)
        {
            return Notes.Get(x => x.Id == noteId).Value;
        }

        protected void SetNoteValueFor(string noteId, string value)
        {
            Notes = Notes.Set(x => x.Id == noteId, new Note(noteId, value));
        }
    }
}
