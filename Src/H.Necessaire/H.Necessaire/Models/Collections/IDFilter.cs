namespace H.Necessaire
{
    public class IDFilter<TId> : SortFilterBase, IPageFilter
    {
        public TId[] IDs { get; set; }

        public PageFilter PageFilter { get; set; }

        protected override string[] ValidSortNames { get; } = new string[] { "ID" };
    }
}
