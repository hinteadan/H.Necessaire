using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DataViewConfig
    {
        public Union<ReactElement, string> Label { get; set; }
        public Union<ReactElement, string> Description { get; set; }
        public int? MaxValueDisplayLength { get; set; }
        
        public NumericDataViewConfig Numeric { get; set; } = new NumericDataViewConfig();
        public ObjectDataViewConfig Object { get; set; } = new ObjectDataViewConfig();
    }

    public class NumericDataViewConfig
    {
        public int NumberOfDecimals { get; set; } = 2;
    }

    public class ObjectDataViewConfig
    {
        public string[] PropertyNamesToIgnore { get; set; } = null;
    }
}
