namespace H.Necessaire.Analytics
{
    public class ConsumerNetworkTracePageFilter : ConsumerNetworkTraceFilter, IPageFilter
    {
        public PageFilter PageFilter { get; set; }
    }
}
