namespace H.Necessaire
{
    public abstract class SortFilterBase : ISortFilter
    {
        protected abstract string[] ValidSortNames { get; }

        public SortFilter[] SortFilters { get; set; }

        public OperationResult ValidateSortFilters() => this.ValidateSortFilters(ValidSortNames);
    }
}
