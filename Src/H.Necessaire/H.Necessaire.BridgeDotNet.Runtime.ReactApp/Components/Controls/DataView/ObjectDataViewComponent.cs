using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ObjectDataViewComponent<TData> : DataViewComponentBase<TData, DataViewComponentProps<TData>, ObjectDataViewState<TData>>
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> propertiesPerType = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public ObjectDataViewComponent(DataViewComponentProps<TData> props, params Union<ReactElement, string>[] children) : base(props, children) { }
        protected override ReactElement New(DataViewComponentProps<TData> props) => new ObjectDataViewComponent<TData>(props);


        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {

                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{GetDataTypeName()}-ObjectView-Chrome",
                    }
                    ,
                    RenderLabelIfNecessary()
                    ,
                    RenderDescriptionIfNecessary()
                    ,
                    (!HasValue() ? RenderNoDataView() : RenderDataView())
                );
        }

        protected override ReactElement RenderDataValue()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            JustifyContent = JustifyContent.Center,
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{GetDataTypeName()}-Properties",
                    }
                    ,
                    GetDataTypeProperties().Select(RenderProperty)
                );
        }

        protected virtual ReactElement RenderProperty(PropertyInfo propertyInfo)
        {
            string label = propertyInfo.Name;//Convert to proper name via config mapping or camelCase parsing
            string description = null;//Fetch from config
            object value = state.Data == null ? null : propertyInfo.GetValue(state.Data);

            return DataViewComponentFactory.BuildViewerFor(
                propertyInfo.PropertyType, 
                value, 
                cfg => { 
                    cfg.Label = label;
                    cfg.Description = description;
                    cfg.MaxValueDisplayLength = state.DataViewConfig?.MaxValueDisplayLength ?? cfg.MaxValueDisplayLength;
                    cfg.Numeric = state.DataViewConfig?.Numeric ?? cfg.Numeric;
                    cfg.Object = state.DataViewConfig?.Object ?? cfg.Object;
                }
            );
        }

        protected override bool HasValue() => GetDataTypeProperties()?.Any() == true && base.HasValue();

        protected virtual PropertyInfo[] GetDataTypeProperties()
        {
            PropertyInfo[] result = propertiesPerType.GetOrAdd(typeof(TData), BuildPropertyInfos());
            return result;
        }

        private PropertyInfo[] BuildPropertyInfos()
        {
            IEnumerable<PropertyInfo> query
                = typeof(TData)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetGetMethod() != null)
                ;

            if(state.DataViewConfig?.Object?.PropertyNamesToIgnore?.Any() == true)
            {
                query
                    = query
                    .Where(p => p.Name.NotIn(state.DataViewConfig.Object.PropertyNamesToIgnore))
                    ;
            }

            return query.ToArrayNullIfEmpty();
        }
    }

    public class ObjectDataViewState<TData> : DataViewComponentState<TData>
    {
    }
}
