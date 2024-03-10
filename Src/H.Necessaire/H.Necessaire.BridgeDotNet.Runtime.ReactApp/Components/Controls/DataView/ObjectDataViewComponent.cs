using Bridge;
using Bridge.React;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ObjectDataViewComponent<TData> : DataViewComponentBase<TData, ObjectDataViewProps<TData>, ObjectDataViewState<TData>>
    {
        public ObjectDataViewComponent(ObjectDataViewProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected override async Task OnPropsChange(ObjectDataViewProps<TData> props)
        {
            await base.OnPropsChange(props);
            await DoAndSetStateAsync(state =>
            {
                state.PropertyNamesToIgnore = props?.PropertyNamesToIgnore;
            });
        }

    }

    public class ObjectDataViewState<TData> : DataViewComponentState<TData>
    {
        public string[] PropertyNamesToIgnore { get; set; }
    }

    public class ObjectDataViewProps<TData> : DataViewComponentProps<TData>
    {
        public string[] PropertyNamesToIgnore { get; set; }
    }
}
