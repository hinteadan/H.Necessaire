namespace H.Necessaire
{
    public class SortFilter
    {
        public string By { get; set; }
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        public enum SortDirection
        {
            Ascending = 0,
            Descending = 1,
        }
    }
}
