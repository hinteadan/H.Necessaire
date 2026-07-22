namespace H.Necessaire
{
    /// <remarks>NOT Thread-Safe</remarks>
    public class ReleaseVersion : ImATaggedValue
    {
        public const string IsHumanReviewedNoteID = "IsHumanReviewed";

        public ReleaseVersion() { }
        private ReleaseVersion(ImATaggedValue<Version> taggedValue) : this()
        {
            if (taggedValue is null) throw new OperationResultException("The provided tagged value is null");
            if (taggedValue.Value is null) throw new OperationResultException("The provided tagged value version is null");

            ID = taggedValue.ID;
            Name = taggedValue.Name;
            Description = taggedValue.Description;
            Notes = taggedValue.Notes;
            Version = taggedValue.Value;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Note[] Notes { get; set; }
        public Version Version { get; set; }

        public bool IsHumanReviewed => !Notes.Get(n => n.ID.Is(IsHumanReviewedNoteID)).IsEmpty();

        public ReleaseVersion Note(params Note[] notes)
        {
            Notes = Notes.Push(notes);
            return this;
        }
        public ReleaseVersion Describe(string description)
        {
            Description = description;
            return this;
        }

        public ReleaseVersion MarkAsHumanReviewed()
        {
            Notes = Notes.Set(IsHumanReviewedNoteID, true.ToString());
            return this;
        }

        public override string ToString()
        {
            if (Name.IsEmpty())
                return Version?.ToString();

            return $"{Name} ({Description.IfEmpty("~ No Description ~")})";
        }

        public static implicit operator Version(ReleaseVersion releaseVersion) => releaseVersion.Version;
        public static implicit operator ReleaseVersion(Version version) => new ReleaseVersion { ID = version.Commit, Name = version.Number.Semantic, Version = version };
        public static implicit operator ReleaseVersion(TaggedValue<Version> taggedValue) => new ReleaseVersion(taggedValue);
    }
}
