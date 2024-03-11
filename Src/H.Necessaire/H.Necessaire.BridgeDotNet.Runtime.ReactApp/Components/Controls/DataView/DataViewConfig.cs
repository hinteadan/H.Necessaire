using Bridge;
using Bridge.React;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ArrayDataViewConfig Array { get; set; } = new ArrayDataViewConfig();

        public DataViewConfig Clone()
        {
            return
                new DataViewConfig
                {
                    Label = Label,
                    Description = Description,
                    MaxValueDisplayLength = MaxValueDisplayLength,
                    SpacingSize = SpacingSize,
                    Numeric = Numeric?.Clone(),
                    Object = Object?.Clone(),
                    Array = Array?.Clone(),
                };
        }

        public DataViewConfig CopyFrom(DataViewConfig other)
        {
            if (other == null)
                return this;

            Label = other.Label;
            Description = other.Description;
            MaxValueDisplayLength = other.MaxValueDisplayLength;
            SpacingSize = other.SpacingSize;
            Numeric = other.Numeric?.Clone();
            Object = other.Object?.Clone();
            Array = other.Array?.Clone();

            return this;
        }
    }

    public class NumericDataViewConfig
    {
        public int NumberOfDecimals { get; set; } = 2;

        public NumericDataViewConfig Clone()
        {
            return
                new NumericDataViewConfig
                {
                    NumberOfDecimals = NumberOfDecimals,
                };
        }
    }

    public class ObjectDataViewConfig
    {
        public bool UseDefaultDataViewer { get; set; } = false;
        public string[] PropertyNamesToIgnore { get; set; } = null;
        public string[] Path { get; set; } = null;
        public int CurrentDepth { get; set; } = 0;
        public int MaxDepth { get; set; } = 5;
        public IReadOnlyDictionary<string, Union<ReactElement, string>> PropertyLabels { get; set; }
        public IReadOnlyDictionary<string, Union<ReactElement, string>> PropertyDescriptions { get; set; }

        public ObjectDataViewConfig Clone()
        {
            return
                new ObjectDataViewConfig
                {
                    UseDefaultDataViewer = UseDefaultDataViewer,
                    PropertyNamesToIgnore = PropertyNamesToIgnore?.ToArray(),
                    Path = Path?.ToArray(),
                    CurrentDepth = CurrentDepth,
                    MaxDepth = MaxDepth,
                    PropertyLabels = PropertyLabels?.ToDictionary(x => x.Key, x => x.Value),
                    PropertyDescriptions = PropertyDescriptions?.ToDictionary(x => x.Key, x => x.Value),
                };
        }
    }

    public class ArrayDataViewConfig
    {
        public Func<int, Union<ReactElement, string>> LabelPrinter { get; set; }
        public Func<int, Union<ReactElement, string>> DescriptionPrinter { get; set; }

        public ArrayDataViewConfig Clone()
        {
            return
                new ArrayDataViewConfig
                {
                    LabelPrinter = LabelPrinter,
                    DescriptionPrinter = DescriptionPrinter,
                };
        }
    }
}
