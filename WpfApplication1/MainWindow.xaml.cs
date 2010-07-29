using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private int maximumIngridientId = 0;
        //private XmlDataProvider ingridients;
        //private XmlDataProvider barIngridients;
        public MainWindow()
        {
                InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            //base.OnSourceInitialized(e);
            //ingridients = (XmlDataProvider)this.Resources["ingridientsSource"];
            //ingridients.Document.NodeChanged += new XmlNodeChangedEventHandler(Document_NodeChanged);
            //ingridients.Document.NodeRemoved += new XmlNodeChangedEventHandler(Document_NodeChanged);
            //this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            //foreach (XmlNode ingridientNode in ingridients.Document.LastChild)
            //{
            //    int id = int.Parse(ingridientNode.Attributes["ID"].Value);
            //    if (id > maximumIngridientId)
            //        maximumIngridientId = id;
            //}

            //barIngridients = (XmlDataProvider)this.Resources["barIngridientsSource"];
            //if (barIngridients.Document == null)
            //{
            //    var document = new XmlDocument();
            //    document.AppendChild(document.CreateElement("Ingridients"));
            //    var source = barIngridients.Source;
            //    barIngridients.Document = document;
            //    barIngridients.Source = source;
            //    barIngridients.Document.Save(source.AbsolutePath);
            //}
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ingridientsGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        void Document_NodeChanged(object sender, XmlNodeChangedEventArgs e)
        {
            //string path = System.IO.Path.GetFullPath(ingridients.Source.OriginalString);
            //ingridients.Document.Save(path);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //XmlNode selected = ingridientsGrid.SelectedItem as XmlNode;
            //if (selected != null && ingridients.Document.LastChild.ChildNodes.Count > 1)
            //    ingridients.Document.LastChild.RemoveChild(selected);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //XmlNode node = ingridients.Document.LastChild.LastChild.CloneNode(false);
            //foreach (XmlAttribute attribute in node.Attributes)
            //    if (attribute.Name == "ID")
            //        attribute.Value = GetNextIngridientId().ToString();
            //    else
            //        attribute.Value = null;
            //ingridients.Document.LastChild.AppendChild(node);
        }

        //private int GetNextIngridientId()
        //{
        //    //return ++maximumIngridientId;
        //}

        private void ingridients_DataChanged(object sender, EventArgs e)
        {
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //string Id = ((XmlElement)((CheckBox)e.Source).DataContext).Attributes["ID"].Value;
            //var element = barIngridients.Document.CreateElement("Ingridient");
            //var attribute = barIngridients.Document.CreateAttribute("ID");
            //attribute.Value = Id;
            //element.Attributes.Append(attribute);
            //barIngridients.Document.LastChild.AppendChild(element);
            //SaveBar();
        }

        private void SaveBar()
        {
        //    string path = System.IO.Path.GetFullPath(barIngridients.Source.OriginalString);
        //    barIngridients.Document.Save(path);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
        //    string Id = ((XmlElement)((CheckBox)e.Source).DataContext).Attributes["ID"].Value;

        //    var xPath = string.Format("/Ingridients/Ingridient[@ID={0}]", Id);
        //    foreach (XmlNode existingElement in barIngridients.Document.SelectNodes(xPath))
        //    {
        //        barIngridients.Document.LastChild.RemoveChild(existingElement);
        //    }

        //    SaveBar();
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            //var listView = (ListView)e.Source;
            //var xPath = string.Format("/Ingridients/Ingridient");
            //foreach (XmlNode existingElement in barIngridients.Document.SelectNodes(xPath))
            //{
            //    var Id = existingElement.Attributes["ID"].Value;
            //    foreach (XmlElement item in listView.Items)
            //    {
            //        if (Id == item.Attributes["ID"].Value)
            //        {
                        
            //        }
            //    }
            //}

        }

    }
}
