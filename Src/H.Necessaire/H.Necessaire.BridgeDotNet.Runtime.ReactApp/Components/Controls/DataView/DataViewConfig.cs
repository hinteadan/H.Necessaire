using Bridge;
using Bridge.React;
using System.Collections.Generic;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DataViewConfig
    {
        public Union<ReactElement, string> Label { get; set; }
        public Union<ReactElement, string> Description { get; set; }
        public int? MaxValueDisplayLength { get; set; }
        public int? SpacingSize { get; set; } = AppBase.Branding.SizingUnitInPixels;
        
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
        public string[] Path { get; set; } = null;
        public IReadOnlyDictionary<string, Union<ReactElement, string>> PropertyLabels { get; set; }
        public IReadOnlyDictionary<string, Union<ReactElement, string>> PropertyDescriptions { get; set; }
    }
}
