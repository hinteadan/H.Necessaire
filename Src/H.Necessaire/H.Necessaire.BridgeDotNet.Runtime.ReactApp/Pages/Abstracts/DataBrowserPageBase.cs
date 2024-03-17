using Bridge;
using Bridge.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataBrowserPageBase<TData, TDataID, TFilter, TProps, TState>
        : PageBase<TProps, TState>
        where TData : IDentityType<TDataID>
        where TFilter : IPageFilter, ISortFilter
        where TState : DataBrowserPageState<TData>, new()
        where TProps : DataBrowserPageProps, new()
    {
        #region Construct
        ImAStorageBrowserService<TData, TFilter> dataBrowserService;
        protected override void EnsureDependencies()
        {
            base.EnsureDependencies();
            dataBrowserService = Get<ImAStorageBrowserService<TData, TFilter>>();
        }
        protected DataBrowserPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
        #endregion

        public override ReactElement Render()
        {
            return
                RenderPageChrome(
                    RenderPageLayout(
                        RenderPreDataView()
                        //,
                        //(
                        //    state.IsLoadingData
                        //    ? RenderLoadingIndicator()
                        //    : RenderDataViewer()
                        //)
                        ,
                        RenderPostDataView()
                        //,
                        //(
                        //    state.OperationResult?.IsSuccessful == false
                        //    ? RenderOperationResult()
                        //    : null
                        //)
                    )
                );
        }

        protected virtual ReactElement RenderPageChrome(params Union<ReactElement, string>[] children)
        {
            return
                new DefaultChrome(children);
        }

        protected virtual ReactElement RenderPageLayout(params Union<ReactElement, string>[] children)
        {
            return
                new FormLayout(
                    new FormLayout.Props
                    {
                        LayoutMode = FormLayoutMode.OnePerRowLarge,
                    }
                    ,
                    children
                );
        }

        protected virtual ReactElement RenderPreDataView() => null;

        protected virtual ReactElement RenderPostDataView() => null;
    }


    public class DataBrowserPageState<TData> : PageStateBase { }
    public class DataBrowserPageProps : PagePropsBase { }




    public abstract class GuidIDDataBrowserPageBase<TData, TProps, TState>
       : DataViewPageBase<TData, Guid, TProps, TState>
       where TData : IDentityType<Guid>
       where TState : DataViewPageState<TData>, new()
       where TProps : DataViewPageProps, new()
    {
        protected GuidIDDataBrowserPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }

    public abstract class StringIDDataBrowserPageBase<TData, TProps, TState>
       : DataViewPageBase<TData, string, TProps, TState>
       where TData : IDentityType<string>
       where TState : DataViewPageState<TData>, new()
       where TProps : DataViewPageProps, new()
    {
        protected StringIDDataBrowserPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }
}
