using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
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
                            MarginLeft = (state.DataViewConfig?.Object?.CurrentDepth ?? 0) == 0 ? 0 : (state.DataViewConfig?.SpacingSize ?? Branding.SizingUnitInPixels),
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
            if (state.DataViewConfig?.Object?.UseDefaultDataViewer == true)
                return base.RenderDataValue();

            if ((state.DataViewConfig?.Object?.CurrentDepth ?? 0) >= (state.DataViewConfig?.Object?.MaxDepth ?? 0))
                return base.RenderDataValue();

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
                            MarginTop = (index == 0 && state.DataViewConfig?.Label == null && state.DataViewConfig?.Description == null) ? 0 : state.DataViewConfig?.SpacingSize ?? Branding.SizingUnitInPixels,
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
                            cfg.CopyFrom(state.DataViewConfig);

                            cfg.Label = GetPropertyLabel(propertyInfo);
                            cfg.Description = GetPropertyDescription(propertyInfo);

                            cfg.Object = (cfg.Object ?? new ObjectDataViewConfig()).And(x => {
                                x.CurrentDepth += 1;
                                x.Path = x.Path.Push(propertyInfo.Name);
                            });
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
            PropertyInfo[] result = propertiesPerType.GetOrAdd(GetDataType(), BuildPropertyInfos());
            if (result == null)
                return null;

            if (state.DataViewConfig?.Object?.PropertyNamesToIgnore?.Any() == true)
            {
                result
                    = result
                    .Where(p => p.Name.NotIn(state.DataViewConfig.Object.PropertyNamesToIgnore))
                    .ToArrayNullIfEmpty();
                    ;
            }

            return result;
        }

        private PropertyInfo[] BuildPropertyInfos()
        {
            IEnumerable<PropertyInfo> query
                = GetDataType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetGetMethod() != null)
                ;

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
