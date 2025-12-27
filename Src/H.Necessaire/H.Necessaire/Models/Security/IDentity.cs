namespace H.Necessaire
{
    public interface IDentity : IGuidIdentity
    {
        string IDTag { get; }
        string DisplayName { get; }
        Note[] Notes { get; set; }
    }
}
