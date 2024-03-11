using Bridge;
using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class NumericDataViewComponent<TNumber> : DataViewComponentBase<TNumber, DataViewComponentProps<TNumber>, NumericDataViewState<TNumber>>
        where TNumber : struct, IComparable, IFormattable, IComparable<TNumber>, IEquatable<TNumber>
    {
        public NumericDataViewComponent(DataViewComponentProps<TNumber> props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected override Union<ReactElement, string> RenderData()
            => Convert.ToDecimal(state.Data).Print(GetNumberOfDecimals()).EllipsizeIfNecessary(maxLength: state.DataViewConfig?.MaxValueDisplayLength ?? defaultMaxLength);
        protected override string RenderTooltipText() => Convert.ToDecimal(state.Data).ToString();

        private int GetNumberOfDecimals()
        {
            int numberOfDecimals = state.DataViewConfig?.Numeric?.NumberOfDecimals ?? 2;
            if (numberOfDecimals < 0)
                numberOfDecimals = 0;

            return numberOfDecimals;
        }
    }

    public class NumericDataViewState<TNumber> : DataViewComponentState<TNumber>
        where TNumber : struct, IComparable, IFormattable, IComparable<TNumber>, IEquatable<TNumber>
    {
    }
}
