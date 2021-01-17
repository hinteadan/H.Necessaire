namespace H.Necessaire
{
    public interface ISortFilter
    {
        SortFilter[] SortFilters { get; }

        OperationResult ValidateSortFilters();
    }
}
