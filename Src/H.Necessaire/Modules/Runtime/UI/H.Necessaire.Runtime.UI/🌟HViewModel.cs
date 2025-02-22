using H.Necessaire.Runtime.UI.Abstractions;
using H.Necessaire.Runtime.UI.Concrete;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UI
{
    public class HViewModel : EphemeralTypeBase, IStringIdentity
    {
        static readonly TimeSpan propertyChangeQueueDebouncedInterval = TimeSpan.FromSeconds(.15);
        readonly ConcurrentQueue<HViewModelPropertyChangeInfo> propertyChangeQueue = new ConcurrentQueue<HViewModelPropertyChangeInfo>();
        readonly Dictionary<string, HViewModelProperty> propertiesIndex = new Dictionary<string, HViewModelProperty>();
        Debouncer propertyChangeQueueDebouncedProcessor;
        object dataSource;
        readonly Type dataType;
        readonly AsyncEventRaiser<HViewModelChangedEventArgs> viewModelChangeEventRaiser;
        ImAnHUILabelProvider labelProvider = DefaultHUILabelProvider.Instance;
        public HViewModel(Type dataType, string id, string title, string description, params Note[] notes)
        {
            DoNotExpire();

            this.dataType = dataType;
            ID = id;
            Title = title;
            Description = description;
            Notes = notes.NullIfEmpty();

            viewModelChangeEventRaiser = new AsyncEventRaiser<HViewModelChangedEventArgs>(this);

            propertyChangeQueueDebouncedProcessor = new Debouncer(ProcessPropertyChangeQueue, propertyChangeQueueDebouncedInterval);

            Properties = BuildProperties();
        }

        public HViewModel(Type dataType, string title, string description, params Note[] notes) : this(dataType, dataType.TypeName(), title, description, notes) { }
        public HViewModel(Type dataType, string title, params Note[] notes) : this(dataType, title, null, notes) { }
        public HViewModel(Type dataType, params Note[] notes) : this(dataType, dataType?.Name?.ToDisplayLabel(), notes) { }
        public HViewModel(Type dataType) : this(dataType, notes: null) { }

        public event AsyncEventHandler<HViewModelChangedEventArgs> OnViewModelChanged { add => viewModelChangeEventRaiser.OnEvent += value; remove => viewModelChangeEventRaiser.OnEvent -= value; }

        public string ID { get; }

        public string Title { get; }

        public string Description { get; }

        public Note[] Notes { get; }


        public HViewModelProperty[] Properties { get; }
        public HViewModelProperty this[string propertyName] => propertiesIndex[propertyName];

        public object Data { get => dataSource; set => WithData(value); }

        public HViewModel WithData(object data)
        {
            if (data is null)
            {
                Clear();
                return this;
            }

            if (data.GetType() != dataType)
            {
                throw new InvalidOperationException("Data type doesn't match the view model data source type");
            }

            dataSource = data;

            Refresh();

            return this;
        }

        public HViewModel WithLabelProvider(ImAnHUILabelProvider labelProvider)
        {
            if (labelProvider is null)
                return this;

            this.labelProvider = labelProvider;

            VisitProperties(property => property.Label = this.labelProvider.GetLabelFor(property.DataSourceProperty, this));

            return this;
        }

        public void Refresh()
        {
            VisitProperties(property => property.WithData(dataSource));
        }

        public void Clear()
        {
            dataSource = null;
            VisitProperties(property => property.Clear());
        }

        public T As<T>()
        {
            if (dataSource is T castedData)
            {
                return castedData;
            }

            return default;
        }

        internal async void QueuePropertyChanged(HViewModelProperty viewModelProperty, object preValue, object newValue)
        {
            propertyChangeQueue.Enqueue(new HViewModelPropertyChangeInfo(viewModelProperty, preValue, newValue));

            await propertyChangeQueueDebouncedProcessor.Invoke();
        }

        HViewModelProperty[] BuildProperties()
        {
            if (dataType is null)
                return null;

            PropertyInfo[] propsToConsider = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return
                propsToConsider
                .OrderBy(p => p.GetPriority())
                .Select(
                    p => !p.CanRead
                    ? null
                    : new HViewModelProperty(p, this).And(h => h.Label = labelProvider.GetLabelFor(p, this))
                )
                .ToNoNullsArray()
                .And(props =>
                {
                    propertiesIndex.Clear();

                    if (props.IsEmpty())
                        return;

                    foreach (HViewModelProperty prop in props)
                    {
                        propertiesIndex.Add(prop.ID, prop);
                    }
                })
                ;
        }

        void VisitProperties(Action<HViewModelProperty> visitorAction)
        {
            if (Properties.IsEmpty())
                return;

            foreach (HViewModelProperty property in Properties)
            {
                visitorAction.Invoke(property);
            }
        }

        async Task ProcessPropertyChangeQueue()
        {
            if (propertyChangeQueue.IsEmpty)
                return;

            List<HViewModelPropertyChangeInfo> updatedProps = new List<HViewModelPropertyChangeInfo>(propertyChangeQueue.Count);

            while (propertyChangeQueue.TryDequeue(out HViewModelPropertyChangeInfo modelPropertyChangeInfo))
            {
                updatedProps.Add(modelPropertyChangeInfo);
            }

            updatedProps
                = updatedProps
                .GroupBy(x => x.Property)
                .Select(x => x.Last())
                .ToList()
                ;

            HViewModelChangedEventArgs args = new HViewModelChangedEventArgs(updatedProps.ToArray());

            await viewModelChangeEventRaiser.Raise(args);
        }


        public static implicit operator HViewModel(Type dataType) => new HViewModel(dataType);
        public static implicit operator HViewModel((Type dataType, string title) x) => new HViewModel(x.dataType, title: x.title, description: null);
    }

    public class HViewModel<DataType> : HViewModel
    {
        public HViewModel() : base(typeof(DataType))
        {
        }

        public HViewModel(params Note[] notes) : base(typeof(DataType), notes)
        {
        }

        public HViewModel(string title, params Note[] notes) : base(typeof(DataType), title, notes)
        {
        }

        public HViewModel(string title, string description, params Note[] notes) : base(typeof(DataType), title, description, notes)
        {
        }

        public HViewModel(string id, string title, string description, params Note[] notes) : base(typeof(DataType), id, title, description, notes)
        {
        }

        public HViewModel<DataType> WithData(DataType data)
        {
            base.WithData(data);

            return this;
        }

        public HViewModelProperty Property<TProperty>(Expression<Func<DataType, TProperty>> selectorExpression)
        {
            if (!(selectorExpression.Body is MemberExpression memberExpression))
                return null;

            string propName = memberExpression.Member.Name;

            return this[propName];
        }

        public static implicit operator DataType(HViewModel<DataType> model) => model.As<DataType>();
        public static implicit operator HViewModel<DataType>(DataType data) => data.ToHViewModel();
        public static implicit operator HViewModel<DataType>((DataType data, string title) x) => x.data.ToHViewModel(title: x.title, description: null);
    }

}
