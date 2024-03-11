using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public interface ImADataViewComponent<T>
    {
        ReactElement New(T data, DataViewConfig config);
    }
}
