using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WpfApplication1
{
    class Bar : DependencyObject, INotifyPropertyChanged, IWeakEventListener
    {
        public Bar()
        {
        }

        private static void OnIngredientsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var bar = (Bar)o;
            var oldCollection = args.NewValue as INotifyCollectionChanged;
            if (oldCollection != null)
                CollectionChangedEventManager.RemoveListener(oldCollection, bar);

            var newCollection = args.NewValue as INotifyCollectionChanged;
            if (newCollection != null)
                CollectionChangedEventManager.AddListener(newCollection, bar);

            bar.ProcessIntridientsChange();
        }

        /// <summary>
        /// IWeakEventListener handler
        /// </summary>
        /// <param name="managerType"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = e as CollectionChangeEventArgs;
            ProcessIntridientsChange();
            return true;
        }

        private void ProcessIntridientsChange()
        {
            var ingredients = this.Ingridients as IEnumerable<XmlNode>;
            if (ingredients != null)
            {
                List<BarIngridient> result = new List<BarIngridient>();
                foreach (XmlNode ingridient in ingredients)
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
    }

    public class BarIngridient
    {
        public int Id { get; set; }
        public bool IsInBar { get; set; }
    }
}
