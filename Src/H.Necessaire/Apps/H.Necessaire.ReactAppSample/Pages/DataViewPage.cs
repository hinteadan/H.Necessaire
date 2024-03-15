using Bridge;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.ReactAppSample.Pages
{
    public class DataViewPage : GuidIDDataViewPageBase<TestData, DataViewPage.Props, DataViewPage.State>
    {
        public DataViewPage(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        protected override async Task<OperationResult<TestData>> LoadData(Guid dataID)
        {
            await Task.Delay(TimeSpan.FromSeconds(2.17));
            return new TestData().ToWinResult();
        }




        public class State : DataViewPageState<TestData> { }

        public class Props : DataViewPageProps { }
    }

    public class TestData : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = "Hin";
        public string LastName { get; set; } = "Tee";
    }
}
