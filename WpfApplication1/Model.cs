using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace WpfApplication1
{
    class Model : IDisposable
    {
        private Dictionary<int, Ingredient> m_ingridientsDictionary = new Dictionary<int, Ingredient>();
        private XmlSerializer m_ingredientsSerializer = new XmlSerializer(typeof(Ingredient[]));
        private string m_ingredientsPath;

        public Model()
        {
            m_ingredientsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ingredients.xml");
            Ingredients = new ObservableCollection<Ingredient>(LoadFromDisk());
            Ingredients.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Ingredients_CollectionChanged);
            Ingredients_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        
        void Ingredients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Ingredient item in e.NewItems)
                        m_ingridientsDictionary[item.Id] = item;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (Ingredient item in e.OldItems)
                        m_ingridientsDictionary.Remove(item.Id);
                    break;
                default:
                    foreach (Ingredient item in this.Ingredients)
                        m_ingridientsDictionary[item.Id] = item;
                    break;
            }

            SaveToDisk();
        }

        private void SaveToDisk()
        {
            using (var file = new XmlTextWriter(m_ingredientsPath, Encoding.UTF8))
            {
                file.Formatting = Formatting.Indented;
                m_ingredientsSerializer.Serialize(file, this.Ingredients.ToArray());
                file.Close();
            }
        }

        private IEnumerable<Ingredient> LoadFromDisk()
        {
            using (var file = new XmlTextReader(m_ingredientsPath))
            {
                return (Ingredient[])m_ingredientsSerializer.Deserialize(file);
            }
        }

        public ObservableCollection<Ingredient> Ingredients { get; set; }

        internal string IngredientIdToName(int id)
        {
            Ingredient ingredient;
            if (m_ingridientsDictionary.TryGetValue(id, out ingredient))
                return ingredient.Name;

            return null;
        }

        public void Dispose()
        {
            SaveToDisk();
        }
    }


    public class Ingredient
    {
        private static object s_syncRoot = new object();
        private static int s_maxIngredientsId = 0;
        private int m_id = 0;

        public Ingredient()
        {
            Name = "<new ingredient>";
            AssignId();
        }

        public void AssignId()
        {
            lock (s_syncRoot)
                m_id = ++s_maxIngredientsId;
        }

        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                lock (s_syncRoot)
                {
                    if (value > s_maxIngredientsId)
                        s_maxIngredientsId = value;
                    m_id = value;
                }
            }
        }
        public string Name { get; set; }
        public int? Percent { get; set; }
    }
}
