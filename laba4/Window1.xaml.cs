using System;
using System.Collections;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Goods goods;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"TempGood.xml", FileMode.OpenOrCreate))
            {
                goods = (xmlSerializer.Deserialize(stream) as Goods);
            }

            textBox.Text = $"ID: {goods.Id}\nName: {goods.Name}\nDesc: {goods.Desc}\nCategory: {goods.Category}\nRate: {goods.Rate}\nPrice: {goods.Price}\nAmount: {goods.Amount}\nOther: {goods.Other}";
            Image image = FindName("Image") as Image;
            try
            {
                image.Source = new BitmapImage(new Uri($"C:\\Users\\Vover\\Desktop\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\{goods.Name}.jpg", UriKind.RelativeOrAbsolute));
            }
            catch
            {
                image.Source = new BitmapImage(new Uri($"C:\\Users\\Vover\\Desktop\\task\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\img0.jpg", UriKind.RelativeOrAbsolute));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
