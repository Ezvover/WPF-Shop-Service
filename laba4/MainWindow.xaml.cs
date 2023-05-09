using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            Deserializatioin();
            MainGrid.ItemsSource = goodsList;
            List<string> strList = new List<string>();
            for (int i = 0; i < goodsList.Count; i++)
            {
                strList.Add(goodsList[i].Category);
            }
            var strList2 = strList.Distinct();
            FilterBOx.ItemsSource = strList2;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));

            using (FileStream stream = new FileStream("LastGood.xml", FileMode.OpenOrCreate))
            {

                xmlSerializer.Serialize(stream, goodsList.Last());

            }

            Serialization();
            Mouse.OverrideCursor = ((FrameworkElement)this.Resources["KinectCursor"]).Cursor;

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //Заполняем меню смены языка:
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }

        }

        public void Serialization()
        {
            var rootFolder = AppDomain.CurrentDomain.BaseDirectory;
            var filesToDelete = Directory.GetFiles(rootFolder, "Good*.xml");

            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            for (int i = 0; i < goodsList.Count; i++)
            {
                using (FileStream stream = new FileStream($"Good{i}.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(stream, goodsList[i]);
                }
            }
        }

        public void Deserializatioin()
        {
            var rootFolder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(rootFolder, "Good*.xml");

            foreach (var file in files)
            {
                var serializer = new XmlSerializer(typeof(Goods));
                using (var stream = File.OpenRead(file))
                {
                    var good = (Goods)serializer.Deserialize(stream);
                    goodsList.Add(good);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) // delete button
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
        private void MainGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) // move to window 1
        {
            var selectedObject = MainGrid.SelectedItem as Goods;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"TempGood.xml", FileMode.OpenOrCreate))
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool test = false;
            for (int i = 0; i < goodsList.Count; i++)
            {
                if ((goodsList[i].Amount < 0) || (goodsList[i].Price < 0) || (int.Parse(goodsList[i].Id) < 0))
                {
                    test = true;
                    break;
                }
                
            }
            if (test == true)
            {
                MessageBox.Show("Одно из значений введено неправильно");
                MainGrid.ItemsSource = null;
                goodsList.Clear();
                Deserializatioin();
                MainGrid.ItemsSource = goodsList;
            }
            else
            {
                Serialization();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.ItemsSource = null;
            goodsList.Clear();
            Deserializatioin();
            MainGrid.ItemsSource = goodsList;
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e)
        {
            List<Goods> tempList2 = new List<Goods>();
            for(int i = 0; i < goodsList.Count; i++)
            {
                if ((goodsList[i].Id.Contains(SearchText3.Text)) || (goodsList[i].Name.Contains(SearchText3.Text)) || (goodsList[i].Desc.Contains(SearchText3.Text)) || (goodsList[i].Other.Contains(SearchText3.Text)) || (goodsList[i].Category.Contains(SearchText3.Text)) || ((goodsList[i].Price).ToString().Contains(SearchText3.Text)) || ((goodsList[i].Category.Contains(SearchText3.Text)) || ((goodsList[i].Price).ToString().Contains(SearchText3.Text)) || (goodsList[i].Amount.ToString().Contains(SearchText3.Text))))
                {
                    tempList2.Add(goodsList[i]);
                }
            }
            MainGrid.ItemsSource = tempList2;
        }
    }
}
