using System.Linq;

namespace H.Necessaire
{
    public abstract class SortFilterBase : ISortFilter
    {
        protected abstract string[] ValidSortNames { get; }

        public SortFilter[] SortFilters { get; set; }

        public OperationResult ValidateSortFilters()
        {
            if (!SortFilters?.Any(x => x != null) ?? true)
                return OperationResult.Win();

            if (SortFilters.Where(x => x != null).Any(x => x.By.NotIn(ValidSortNames)))
                return OperationResult.Fail($"Some of the sort properties are invalid. These are the valid sortable properties: {string.Join(", ", ValidSortNames)}.");

            return OperationResult.Win();
        }
    }
}
