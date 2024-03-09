namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DataViewConfig
    {
        public string Label { get; set; }
        public string Description { get; set; }
        public int? MaxValueDisplayLength { get; set; }
        
        public NumericDataViewConfig Numeric { get; set; } = new NumericDataViewConfig();
    }

    public class NumericDataViewConfig
    {
        public int NumberOfDecimals { get; set; } = 2;
    }
}
