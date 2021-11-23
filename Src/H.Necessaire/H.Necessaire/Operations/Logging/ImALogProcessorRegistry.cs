namespace H.Necessaire
{
    public interface ImALogProcessorRegistry
    {
        ImALogProcessorRegistry Register(ImALogProcessor logProcessor);
        ImALogProcessor[] GetAllKnownProcessors();
    }
}
