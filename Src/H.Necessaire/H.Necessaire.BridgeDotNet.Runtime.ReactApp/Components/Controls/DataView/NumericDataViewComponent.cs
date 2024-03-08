using Bridge;
using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class NumericDataViewComponent<TNumber> : DataViewComponentBase<TNumber, NumericDataViewProps<TNumber>, NumericDataViewState<TNumber>>
        where TNumber : struct, IComparable, IFormattable, IComparable<TNumber>, IEquatable<TNumber>
    {
        protected override async Task OnPropsChange(NumericDataViewProps<TNumber> props)
        {
            await base.OnPropsChange(props);
            await DoAndSetStateAsync(state =>
            {
                state.NumberOfDecimals = props?.NumberOfDecimals ?? 2;
                if (state.NumberOfDecimals < 0)
                    state.NumberOfDecimals = 0;
            });
        }

        public NumericDataViewComponent(NumericDataViewProps<TNumber> props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected override Union<ReactElement, string> RenderData() => Convert.ToDecimal(state.Data).Print(state.NumberOfDecimals).EllipsizeIfNecessary(maxLength: props?.MaxLength ?? defaultMaxLength);
        protected override string RenderTooltipText() => Convert.ToDecimal(state.Data).ToString();
    }

    public class NumericDataViewState<TNumber> : DataViewComponentState<TNumber>
        where TNumber : struct, IComparable, IFormattable, IComparable<TNumber>, IEquatable<TNumber>
    {
        public int NumberOfDecimals { get; set; } = 2;
    }

    public class NumericDataViewProps<TNumber> : DataViewComponentProps<TNumber>
        where TNumber : struct, IComparable, IFormattable, IComparable<TNumber>, IEquatable<TNumber>
    {
        public int NumberOfDecimals { get; set; } = 2;
    }
}
