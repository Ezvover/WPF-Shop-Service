using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Goods> goodsList = new List<Goods>();
        public MainWindow()
        {
            InitializeComponent();
            goodsList.Add(new Goods("0", "milk", "good", "eat", 5.0, 30, 24, "other"));
            goodsList.Add(new Goods("1", "apple", "bad", "phone", 3.0, 20, 16, "abc"));
            MainGrid.ItemsSource = goodsList;
            List<string> strList = new List<string>();
            for (int i = 0; i < goodsList.Count; i++) 
            {
                strList.Add(goodsList[i].Category);
            }
            var strList2 = strList.Distinct();
            FilterBOx.ItemsSource = strList2;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream fs = new FileStream("addGoods.xml", FileMode.OpenOrCreate))
            {
                goodsList.Add(xmlSerializer.Deserialize(fs) as Goods);
            }

            //DeleteButton.Content = Properties.Resources.Delete;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedObject = MainGrid.SelectedItem as Goods;
            MainGrid.ItemsSource = null;
            for (int i = 0; i < goodsList.Count; i++) 
            {
                if (goodsList[i].Equals(selectedObject))
                {
                    goodsList.Remove(goodsList[i]);
                }
            }
            MainGrid.ItemsSource = goodsList;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream("goods.xml", FileMode.OpenOrCreate))
            {
                foreach (Goods item in goodsList)
                {
                    xmlSerializer.Serialize(stream, item);
                }
            }

            Goods g = MainGrid.SelectedItem as Goods;
            EditWindow edit = new EditWindow(g.Id);
            edit.ShowDialog();
        }
        
        private void MainGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedObject = MainGrid.SelectedItem as Goods;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"goodsTemp.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(stream, selectedObject);
            }
            Window1 window = new Window1();
            window.Show();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Goods> tempList = new List<Goods>();
            MainGrid.ItemsSource = null;
            for (int i = 0; i < goodsList.Count; i++)
            {
                if ((goodsList[i].Price >= int.Parse(SearchText.Text)) && (goodsList[i].Price <= int.Parse(SearchText2.Text)))
                {
                    tempList.Add(goodsList[i]);
                }
            }
            MainGrid.ItemsSource = tempList;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<Goods> tempList = new List<Goods>();
            MainGrid.ItemsSource = null;
            foreach (var c in goodsList)
            {
                if (c.Category.Equals(FilterBOx.Text))
                {
                    tempList.Add(c);
                }
            }
            MainGrid.ItemsSource = tempList;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddWindow window = new AddWindow();
            window.Show();
            
        }
    }
}
