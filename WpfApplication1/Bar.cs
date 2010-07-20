using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Xml;

namespace WpfApplication1
{
    class Bar : DependencyObject, INotifyPropertyChanged, IWeakEventListener
    {
        public Bar()
        {
            //CollectionViewSource
        }

        private static void OnIngredientsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {

            
            var bar = (Bar)o;

            var t = bar.ReadLocalValue(IngridientsProperty) as BindingExpression;
            var t1 = bar.GetValue(IngridientsProperty);
            var t3 = t.ParentBinding.Source as DataSourceProvider;
            if (t3 != null)
                DataChangedEventManager.AddListener(t3, bar);
           
            var provider = bar.Ingridients as DataSourceProvider;
            if (provider != null)
                DataChangedEventManager.AddListener(provider, bar);

        }

        

        private static object OnIngredientsValueChanged(DependencyObject d, object baseValue)
        {
            return null;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var provider = e.NewValue as DataSourceProvider;
            if (provider != null)
                DataChangedEventManager.AddListener(provider, this);

            base.OnPropertyChanged(e);
            var t2 = e.NewValue as IEnumerable<XmlNode>;
            if (t2 != null)
            {
                List<BarIngridient> result = new List<BarIngridient>();
                foreach (XmlNode ingridient in t2)
                {
                    string IdString = ingridient.Attributes["ID"].Value;
                    int Id;
                    if (int.TryParse(IdString, out Id))
                    {
                        BarIngridient barIngridient = new BarIngridient();
                        barIngridient.Id = Id;
                        barIngridient.IsInBar = true;
                        result.Add(barIngridient);
                    }
                }

                BarIngridients = result.ToArray();
            }

            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(BarIngridientsProperty.Name));
        }

        

        public object Ingridients
        {
            get { return (object)GetValue(IngridientsProperty); }
            set { SetValue(IngridientsProperty, value); }
        }
 
        // Using a DependencyProperty as the backing store for Intridients.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IngridientsProperty =
            DependencyProperty.Register(
                                    "Ingridients",
                                    typeof(object),
                                    typeof(Bar),
                                    new FrameworkPropertyMetadata(
                                        null,
                                        new PropertyChangedCallback(OnIngredientsChanged)));

        public BarIngridient[] BarIngridients
        {
            get { return (BarIngridient[])GetValue(BarIngridientsProperty); }
            set { SetValue(BarIngridientsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarIngridients.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarIngridientsProperty =
            DependencyProperty.Register("BarIngridients", typeof(BarIngridient[]), typeof(Bar), new UIPropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public class BarIngridient
    {
        public int Id { get; set; }
        public bool IsInBar { get; set; }
    }
}
