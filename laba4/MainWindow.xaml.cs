using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        // продумать логику автогенерации id
        List<Goods> goodsList = new List<Goods>();
        public MainWindow()
        {
            InitializeComponent();
            goodsList.Add(new Goods("0", "milk", "good", "eat", 5.0, 30, 24, "other"));
            goodsList.Add(new Goods("1", "apple", "bad", "eat", 3.0, 20, 16, "abc"));
            MainGrid.ItemsSource = goodsList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            /*XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream("goods.xml", FileMode.OpenOrCreate))
            {
                foreach (Goods item in goodsList) 
                {
                    xmlSerializer.Serialize(stream, item);
                }
            }*/



            Goods g = MainGrid.SelectedItem as Goods;
            EditWindow edit = new EditWindow(g.Id);
            edit.ShowDialog();


        }
    }
}
