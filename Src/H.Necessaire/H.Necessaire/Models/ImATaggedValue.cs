namespace H.Necessaire
{
    public interface ImATaggedValue : IStringIdentity
    {
        string Name { get; }
        string Description { get; }
        Note[] Notes { get; }
    }

    public interface ImATaggedValue<TValue> : ImATaggedValue
    {
        TValue Value { get; }
    }
}
