//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Data;
//using System.ComponentModel;
//using System.Collections.ObjectModel;
//using System.Globalization;
//using System.Windows.Markup;
//using System.Collections;
//using System.Collections.Specialized;

//namespace WpfApplication1
//{
//public class CollectionViewSource : DependencyObject, ISupportInitialize, IWeakEventListener
//{
//    // Fields
//    private CultureInfo _culture;
//    private DataSourceProvider _dataProvider;
//    private int _deferLevel;
//    private FilterStub _filterStub;
//    private ObservableCollection<GroupDescription> _groupBy;
//    private bool _hasMultipleInheritanceContexts;
//    private DependencyObject _inheritanceContext;
//    private bool _isInitializing;
//    private bool _isViewInitialized;
//    private DependencyProperty _propertyForInheritanceContext;
//    private SortDescriptionCollection _sort = new SortDescriptionCollection();
//    private int _version;
//    public static readonly DependencyProperty CollectionViewTypeProperty = DependencyProperty.Register("CollectionViewType", typeof(Type), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CollectionViewSource.OnCollectionViewTypeChanged)), new ValidateValueCallback(CollectionViewSource.IsCollectionViewTypeValid));
//    internal static readonly CollectionViewSource DefaultSource = new CollectionViewSource();
//    private static readonly UncommonField<FilterEventHandler> FilterHandlersField = new UncommonField<FilterEventHandler>();
//    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CollectionViewSource.OnSourceChanged)), new ValidateValueCallback(CollectionViewSource.IsSourceValid));
//    public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
//    private static readonly DependencyPropertyKey ViewPropertyKey = DependencyProperty.RegisterReadOnly("View", typeof(ICollectionView), typeof(CollectionViewSource), new FrameworkPropertyMetadata(null));

//    // Events
//    public event FilterEventHandler Filter
//    {
//        add
//        {
//            FilterEventHandler a = FilterHandlersField.GetValue(this);
//            if (a != null)
//            {
//                a = (FilterEventHandler) Delegate.Combine(a, value);
//            }
//            else
//            {
//                a = value;
//            }
//            FilterHandlersField.SetValue(this, a);
//            this.OnForwardedPropertyChanged();
//        }
//        remove
//        {
//            FilterEventHandler source = FilterHandlersField.GetValue(this);
//            if (source != null)
//            {
//                source = (FilterEventHandler) Delegate.Remove(source, value);
//                if (source == null)
//                {
//                    FilterHandlersField.ClearValue(this);
//                }
//                else
//                {
//                    FilterHandlersField.SetValue(this, source);
//                }
//            }
//            this.OnForwardedPropertyChanged();
//        }
//    }

//    // Methods
//    public CollectionViewSource()
//    {
//        this._sort.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
//        this._groupBy = new ObservableCollection<GroupDescription>();
//        this._groupBy.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
//    }

//    internal override void AddInheritanceContext(DependencyObject context, DependencyProperty property)
//    {
//        if (!this._hasMultipleInheritanceContexts && (this._inheritanceContext == null))
//        {
//            this._propertyForInheritanceContext = property;
//        }
//        else
//        {
//            this._propertyForInheritanceContext = null;
//        }
//        InheritanceContextHelper.AddInheritanceContext(context, this, ref this._hasMultipleInheritanceContexts, ref this._inheritanceContext);
//    }

//    private void ApplyPropertiesToView(ICollectionView view)
//    {
//        if ((view != null) && (this._deferLevel <= 0))
//        {
//            using (view.DeferRefresh())
//            {
//                int num;
//                int count;
//                Predicate<object> filterWrapper;
//                if (this.Culture != null)
//                {
//                    view.Culture = this.Culture;
//                }
//                if (view.CanSort)
//                {
//                    view.SortDescriptions.Clear();
//                    num = 0;
//                    count = this.SortDescriptions.Count;
//                    while (num < count)
//                    {
//                        view.SortDescriptions.Add(this.SortDescriptions[num]);
//                        num++;
//                    }
//                }
//                else if (this.SortDescriptions.Count > 0)
//                {
//                    throw new InvalidOperationException(SR.Get("CannotSortView", new object[] { view }));
//                }
//                if (FilterHandlersField.GetValue(this) != null)
//                {
//                    filterWrapper = this.FilterWrapper;
//                }
//                else
//                {
//                    filterWrapper = null;
//                }
//                if (view.CanFilter)
//                {
//                    view.Filter = filterWrapper;
//                }
//                else if (filterWrapper != null)
//                {
//                    throw new InvalidOperationException(SR.Get("CannotFilterView", new object[] { view }));
//                }
//                if (view.CanGroup)
//                {
//                    view.GroupDescriptions.Clear();
//                    num = 0;
//                    count = this.GroupDescriptions.Count;
//                    while (num < count)
//                    {
//                        view.GroupDescriptions.Add(this.GroupDescriptions[num]);
//                        num++;
//                    }
//                }
//                else if (this.GroupDescriptions.Count > 0)
//                {
//                    throw new InvalidOperationException(SR.Get("CannotGroupView", new object[] { view }));
//                }
//            }
//        }
//    }

//    private void BeginDefer()
//    {
//        this._deferLevel++;
//    }

//    public IDisposable DeferRefresh()
//    {
//        return new DeferHelper(this);
//    }

//    private void EndDefer()
//    {
//        if (--this._deferLevel == 0)
//        {
//            this.EnsureView();
//        }
//    }

//    private void EnsureView()
//    {
//        this.EnsureView(this.Source, this.CollectionViewType);
//    }

//    private void EnsureView(object source, Type collectionViewType)
//    {
//        if (!this._isInitializing && (this._deferLevel <= 0))
//        {
//            DataSourceProvider provider = source as DataSourceProvider;
//            if (provider != this._dataProvider)
//            {
//                if (this._dataProvider != null)
//                {
//                    DataChangedEventManager.RemoveListener(this._dataProvider, this);
//                }
//                this._dataProvider = provider;
//                if (this._dataProvider != null)
//                {
//                    DataChangedEventManager.AddListener(this._dataProvider, this);
//                    this._dataProvider.InitialLoad();
//                }
//            }
//            if (provider != null)
//            {
//                source = provider.Data;
//            }
//            ICollectionView view = null;
//            if (source != null)
//            {
//                ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, this, collectionViewType, true);
//                if (record != null)
//                {
//                    view = record.View;
//                    this._isViewInitialized = record.IsInitialized;
//                    if (this._version != record.Version)
//                    {
//                        this.ApplyPropertiesToView(view);
//                        record.Version = this._version;
//                    }
//                }
//            }
//            base.SetValue(ViewPropertyKey, view);
//        }
//    }

//    internal static CollectionView GetDefaultCollectionView(object source, bool createView)
//    {
//        if (!IsValidSourceForView(source))
//        {
//            return null;
//        }
//        ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, DefaultSource, null, createView);
//        if (record == null)
//        {
//            return null;
//        }
//        return (CollectionView) record.View;
//    }

//    internal static CollectionView GetDefaultCollectionView(object source, DependencyObject d)
//    {
//        CollectionView defaultCollectionView = GetDefaultCollectionView(source, true);
//        if ((defaultCollectionView != null) && (defaultCollectionView.Culture == null))
//        {
//            XmlLanguage language = (d != null) ? ((XmlLanguage) d.GetValue(FrameworkElement.LanguageProperty)) : null;
//            if (language == null)
//            {
//                return defaultCollectionView;
//            }
//            try
//            {
//                defaultCollectionView.Culture = language.GetSpecificCulture();
//            }
//            catch (InvalidOperationException)
//            {
//            }
//        }
//        return defaultCollectionView;
//    }

//    public static ICollectionView GetDefaultView(object source)
//    {
//        return GetOriginalView(GetDefaultCollectionView(source, true));
//    }

//    private static ICollectionView GetOriginalView(ICollectionView view)
//    {
//        for (CollectionViewProxy proxy = view as CollectionViewProxy; proxy != null; proxy = view as CollectionViewProxy)
//        {
//            view = proxy.ProxiedView;
//        }
//        return view;
//    }

//    private static bool IsCollectionViewTypeValid(object o)
//    {
//        Type c = (Type) o;
//        if (c != null)
//        {
//            return typeof(ICollectionView).IsAssignableFrom(c);
//        }
//        return true;
//    }

//    public static bool IsDefaultView(ICollectionView view)
//    {
//        if (view != null)
//        {
//            object sourceCollection = view.SourceCollection;
//            return (GetOriginalView(view) == LazyGetDefaultView(sourceCollection));
//        }
//        return true;
//    }

//    internal bool IsShareableInTemplate()
//    {
//        return false;
//    }

//    private static bool IsSourceValid(object o)
//    {
//        if (((o != null) && !(o is IEnumerable)) && (!(o is IListSource) && !(o is DataSourceProvider)))
//        {
//            return false;
//        }
//        return !(o is ICollectionView);
//    }

//    private static bool IsValidSourceForView(object o)
//    {
//        if ((o != null) && !(o is IEnumerable))
//        {
//            return (o is IListSource);
//        }
//        return true;
//    }

//    private static ICollectionView LazyGetDefaultView(object source)
//    {
//        return GetOriginalView(GetDefaultCollectionView(source, false));
//    }

//    protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
//    {
//    }

//    private static void OnCollectionViewTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        CollectionViewSource source = (CollectionViewSource) d;
//        Type oldValue = (Type) e.OldValue;
//        Type newValue = (Type) e.NewValue;
//        if (!source._isInitializing)
//        {
//            throw new InvalidOperationException(SR.Get("CollectionViewTypeIsInitOnly"));
//        }
//        source.OnCollectionViewTypeChanged(oldValue, newValue);
//        source.EnsureView();
//    }

//    private void OnForwardedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//    {
//        this.OnForwardedPropertyChanged();
//    }

//    private void OnForwardedPropertyChanged()
//    {
//        this._version++;
//        this.ApplyPropertiesToView(this.View);
//    }

//    protected virtual void OnSourceChanged(object oldSource, object newSource)
//    {
//    }

//    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        CollectionViewSource source = (CollectionViewSource) d;
//        source.OnSourceChanged(e.OldValue, e.NewValue);
//        source.EnsureView();
//    }

//    protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
//    {
//        if (managerType == typeof(DataChangedEventManager))
//        {
//            this.EnsureView();
//            return true;
//        }
//        return false;
//    }

//    internal override void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
//    {
//        InheritanceContextHelper.RemoveInheritanceContext(context, this, ref this._hasMultipleInheritanceContexts, ref this._inheritanceContext);
//        this._propertyForInheritanceContext = null;
//    }

//    void ISupportInitialize.BeginInit()
//    {
//        this._isInitializing = true;
//    }

//    void ISupportInitialize.EndInit()
//    {
//        this._isInitializing = false;
//        this.EnsureView();
//    }

//    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//    bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
//    {
//        return this.ReceiveWeakEvent(managerType, sender, e);
//    }

//    private bool WrapFilter(object item)
//    {
//        FilterEventArgs e = new FilterEventArgs(item);
//        FilterEventHandler handler = FilterHandlersField.GetValue(this);
//        if (handler != null)
//        {
//            handler(this, e);
//        }
//        return e.Accepted;
//    }

//    // Properties
//    internal CollectionView CollectionView
//    {
//        get
//        {
//            ICollectionView view = (ICollectionView) base.GetValue(ViewProperty);
//            if ((view != null) && !this._isViewInitialized)
//            {
//                object source = this.Source;
//                DataSourceProvider provider = source as DataSourceProvider;
//                if (provider != null)
//                {
//                    source = provider.Data;
//                }
//                if (source != null)
//                {
//                    ViewRecord record = DataBindEngine.CurrentDataBindEngine.GetViewRecord(source, this, this.CollectionViewType, true);
//                    if (record != null)
//                    {
//                        record.InitializeView();
//                        this._isViewInitialized = true;
//                    }
//                }
//            }
//            return (CollectionView) view;
//        }
//    }

//    public Type CollectionViewType
//    {
//        get
//        {
//            return (Type) base.GetValue(CollectionViewTypeProperty);
//        }
//        set
//        {
//            base.SetValue(CollectionViewTypeProperty, value);
//        }
//    }

//    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
//    public CultureInfo Culture
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._culture;
//        }
//        set
//        {
//            this._culture = value;
//            this.OnForwardedPropertyChanged();
//        }
//    }

//    internal override int EffectiveValuesInitialSize
//    {
//        get
//        {
//            return 3;
//        }
//    }

//    private Predicate<object> FilterWrapper
//    {
//        get
//        {
//            if (this._filterStub == null)
//            {
//                this._filterStub = new FilterStub(this);
//            }
//            return this._filterStub.FilterWrapper;
//        }
//    }

//    public ObservableCollection<GroupDescription> GroupDescriptions
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._groupBy;
//        }
//    }

//    internal override bool HasMultipleInheritanceContexts
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._hasMultipleInheritanceContexts;
//        }
//    }

//    internal override DependencyObject InheritanceContext
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._inheritanceContext;
//        }
//    }

//    internal DependencyProperty PropertyForInheritanceContext
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._propertyForInheritanceContext;
//        }
//    }

//    public SortDescriptionCollection SortDescriptions
//    {
//        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//        get
//        {
//            return this._sort;
//        }
//    }

//    public object Source
//    {
//        get
//        {
//            return base.GetValue(SourceProperty);
//        }
//        set
//        {
//            base.SetValue(SourceProperty, value);
//        }
//    }

//    [ReadOnly(true)]
//    public ICollectionView View
//    {
//        get
//        {
//            return GetOriginalView(this.CollectionView);
//        }
//    }

//    // Nested Types
//    private class DeferHelper : IDisposable
//    {
//        // Fields
//        private CollectionViewSource _target;

//        // Methods
//        public DeferHelper(CollectionViewSource target)
//        {
//            this._target = target;
//            this._target.BeginDefer();
//        }

//        public void Dispose()
//        {
//            if (this._target != null)
//            {
//                CollectionViewSource source = this._target;
//                this._target = null;
//                source.EndDefer();
//            }
//            GC.SuppressFinalize(this);
//        }
//    }

//    private class FilterStub
//    {
//        // Fields
//        private Predicate<object> _filterWrapper;
//        private WeakReference _parent;

//        // Methods
//        public FilterStub(CollectionViewSource parent)
//        {
//            this._parent = new WeakReference(parent);
//            this._filterWrapper = new Predicate<object>(this.WrapFilter);
//        }

//        private bool WrapFilter(object item)
//        {
//            CollectionViewSource target = (CollectionViewSource) this._parent.Target;
//            if (target != null)
//            {
//                return target.WrapFilter(item);
//            }
//            return true;
//        }

//        // Properties
//        public Predicate<object> FilterWrapper
//        {
//            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
//            get
//            {
//                return this._filterWrapper;
//            }
//        }
//    }
//}

 
//Collapse Methods
 
//}
