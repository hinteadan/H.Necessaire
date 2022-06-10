using System;

namespace H.Necessaire
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class NoteAttribute : Attribute
    {
        public NoteAttribute(params Note[] notes)
        {
            Notes = notes;
        }

        public Note[] Notes { get; }
    }
}
