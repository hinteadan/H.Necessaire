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

        protected virtual ReactElement RenderProperty(PropertyInfo propertyInfo, int index)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            MarginTop = index == 0 ? 0 : state.DataViewConfig?.SpacingSize ?? Branding.SizingUnitInPixels,
                            JustifyContent = JustifyContent.Center,
                        }
                        .FlexNode(isVerticalFlow: true),
                        ClassName = $"{GetDataTypeName()}-Property-{propertyInfo.Name}",
                    }
                    ,
                    DataViewComponentFactory.BuildViewerFor(
                        propertyInfo.PropertyType,
                        state.Data == null ? null : propertyInfo.GetValue(state.Data),
                        cfg =>
                        {
                            cfg.Label = GetPropertyLabel(propertyInfo);
                            cfg.Description = GetPropertyDescription(propertyInfo);
                            cfg.MaxValueDisplayLength = state.DataViewConfig?.MaxValueDisplayLength ?? cfg.MaxValueDisplayLength;
                            cfg.Numeric = state.DataViewConfig?.Numeric ?? cfg.Numeric;
                            cfg.Object = state.DataViewConfig?.Object ?? cfg.Object;
                        }
                    )
                );
        }

        private Union<ReactElement, string> GetPropertyLabel(PropertyInfo propertyInfo)
        {
            string propertyFullName = GetPropertyFullName(propertyInfo);

            Union<ReactElement, string> labelFromConfigMapping = null;
            if (state.DataViewConfig?.Object?.PropertyLabels?.TryGetValue(propertyFullName, out labelFromConfigMapping) == true)
                return labelFromConfigMapping;

            return propertyInfo.Name.ToDisplayLabel();
        }

        private Union<ReactElement, string> GetPropertyDescription(PropertyInfo propertyInfo)
        {
            string propertyFullName = GetPropertyFullName(propertyInfo);

            Union<ReactElement, string> descriptionFromConfigMapping = null;
            if (state.DataViewConfig?.Object?.PropertyDescriptions?.TryGetValue(propertyFullName, out descriptionFromConfigMapping) == true)
                return descriptionFromConfigMapping;

            return null;
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

        private string GetPropertyFullName(PropertyInfo propertyInfo)
        {
            string propertyFullName = propertyInfo.Name;
            if (state.DataViewConfig?.Object?.Path?.Any() == true)
                propertyFullName = $"{string.Join(".", state.DataViewConfig.Object.Path)}.{propertyFullName}";
            return propertyFullName;
        }
    }

    public class ObjectDataViewState<TData> : DataViewComponentState<TData>
    {
    }
}
