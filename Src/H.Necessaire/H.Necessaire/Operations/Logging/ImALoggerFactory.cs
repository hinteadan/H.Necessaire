namespace H.Necessaire
{
    public interface ImALoggerFactory
    {
        ImALogger BuildLogger(string component, string application = "H.Necessaire");
        ImALogger BuildLogger<T>(string application = "H.Necessaire");
    }
}
