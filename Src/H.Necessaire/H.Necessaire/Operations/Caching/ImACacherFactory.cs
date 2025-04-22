namespace H.Necessaire
{
    public interface ImACacherFactory
    {
        ImACacher<T> BuildCacher<T>(string cacherID = null);
    }
}
