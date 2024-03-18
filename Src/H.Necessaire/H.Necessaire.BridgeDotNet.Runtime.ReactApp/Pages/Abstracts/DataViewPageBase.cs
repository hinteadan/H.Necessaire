﻿using Bridge;
using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class DataViewPageBase<TData, TDataID, TProps, TState>
        : PageBase<TProps, TState>
        where TData : IDentityType<TDataID>
        where TState : DataViewPageState<TData>, new()
        where TProps : DataViewPageProps, new()
    {
        #region Construct
        static readonly string[] dataIDParamNames = new string[] { "id", "ID", "Id", "dataID", "DataID", "dataId", "data-id" };
        ImAStorageService<TDataID, TData> dataStorageService;
        protected override void EnsureDependencies()
        {
            base.EnsureDependencies();
            dataStorageService = Get<ImAStorageService<TDataID, TData>>();
        }
        protected DataViewPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override async Task RunAtStartup()
        {
            await base.RunAtStartup();

            OperationResult<TDataID> dataIDResult = GetDataID();
            if (!dataIDResult.IsSuccessful)
            {
                await DoAndSetStateAsync(state =>
                {
                    state.OperationResult = dataIDResult;
                });
                return;
            }

            using (new ScopedRunner(
                onStart: () => DoAndSetState(state => state.IsLoadingData = true),
                onStop: () => DoAndSetState(state => state.IsLoadingData = false)
                ))
            {
                await DoAndSetStateAsync(async state =>
                {
                    state.DataViewConfig = (await GetDataViewConfig()) ?? new DataViewConfig();
                    OperationResult<TData> dataLoadResult = await LoadData(dataIDResult.Payload);
                    if (!dataLoadResult.IsSuccessful)
                    {
                        state.OperationResult = dataLoadResult;
                        return;
                    }

                    state.Data = dataLoadResult.Payload;
                    state.DataType = typeof(TData);
                    if (state.DataType == typeof(object))
                        state.DataType = state.Data?.GetType();
                });
            }
        }
        #endregion


        public override ReactElement Render()
        {
            return
                RenderPageChrome(
                    RenderPageLayout(
                        RenderPreDataView()
                        ,
                        (
                            state.IsLoadingData 
                            ? RenderLoadingIndicator() 
                            : RenderDataViewer()
                        )
                        ,
                        RenderPostDataView()
                        ,
                        (
                            state.OperationResult?.IsSuccessful == false
                            ? RenderOperationResult()
                            : null
                        )
                    )
                );
        }

        protected virtual ReactElement RenderDataViewer()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = "DataViewer-Chrome",
                    }
                    ,
                    RenderViewer()
                );
        }

        protected virtual ReactElement RenderViewer() => DataViewComponentFactory.BuildViewerFor<TData>(state.Data, x => x.CopyFrom(state.DataViewConfig));

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
                        LayoutMode = FormLayoutMode.OnePerRow,
                    }
                    ,
                    children
                );
        }        

        protected virtual ReactElement RenderLoadingIndicator()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = "LoadingIndicator-Chrome",
                    }
                    ,
                    new TimeoutProgressIndicator(new TimeoutProgressIndicator.Props
                    {
                        Label = $"Loading {state.DataType?.Name ?? "[Unknown Data Type]"}...",
                    })
                );
        }

        protected virtual ReactElement RenderOperationResult()
        {
            if (state.OperationResult == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = "OperationResult-Chrome",
                    }
                    ,
                    new OperationResultCard(new OperationResultCard.Props { 
                        OperationResult = state.OperationResult,
                        Width = "100%",
                    })
                );
        }

        protected virtual ReactElement RenderPreDataView() 
        {
            string id = state.Data?.ID?.ToString()?.Trim();
            if (id.IsEmpty())
                return null;

            return new Title($"{state.DataType?.Name ?? "[Unknown Data Type]"} - {id}");
        }

        protected virtual ReactElement RenderPostDataView()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                    }
                    ,
                    new Button(
                        new Button.Props {
                            OnClick = () => FlySafe(() => Navi.GoBack()),
                        }
                        ,
                        "⬅️ Back"
                    )
                );
        }


        protected virtual Task<DataViewConfig> GetDataViewConfig() => new DataViewConfig().AsTask();
        protected virtual OperationResult<TDataID> GetDataID()
        {
            if (props == null)
                return OperationResult.Fail("No page properties defined, therefore no Data ID to load.").WithoutPayload<TDataID>();

            bool failedToGetDataID = false;
            TDataID dataID = props.NavigationParams.GetValue<TDataID>(orFail: () => failedToGetDataID = true);
            if (failedToGetDataID == false)
                return dataID.ToWinResult();

            foreach (string dataIDParamName in dataIDParamNames)
            {
                failedToGetDataID = false;
                dataID = props.NavigationParams.GetValueFor<TDataID>(dataIDParamName, orFail: () => failedToGetDataID = true);
                if (failedToGetDataID == false)
                    return dataID.ToWinResult();
            }

            return
                OperationResult.Fail("Cannot find the Data ID in the page navigation params").WithoutPayload<TDataID>();
        }
        protected virtual async Task<OperationResult<TData>> LoadData(TDataID dataID)
        {
            if (dataStorageService == null)
            {
                return OperationResult.Fail($"There's no default storage service defined for {state.DataType?.Name ?? "[Unknown Data Type]"}").WithoutPayload<TData>();
            }

            return await dataStorageService.LoadByID(dataID);
        }
    }

    public class DataViewPageState<TData> : PageStateBase
    {
        public TData Data { get; set; }
        public Type DataType { get; set; } = typeof(TData);
        public DataViewConfig DataViewConfig { get; set; } = new DataViewConfig();

        public bool IsLoadingData { get; set; } = false;
        public OperationResult OperationResult { get; set; }
    }

    public class DataViewPageProps : PagePropsBase
    {
    }



    public abstract class GuidIDDataViewPageBase<TData, TProps, TState>
       : DataViewPageBase<TData, Guid, TProps, TState>
       where TData : IDentityType<Guid>
       where TState : DataViewPageState<TData>, new()
       where TProps : DataViewPageProps, new()
    {
        protected GuidIDDataViewPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }

    public abstract class StringIDDataViewPageBase<TData, TProps, TState>
       : DataViewPageBase<TData, string, TProps, TState>
       where TData : IDentityType<string>
       where TState : DataViewPageState<TData>, new()
       where TProps : DataViewPageProps, new()
    {
        protected StringIDDataViewPageBase(TProps props, params Union<ReactElement, string>[] children) : base(props, children) { }
    }
}