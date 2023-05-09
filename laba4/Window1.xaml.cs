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

            try
            {
                textBox.Text = $"ID/ИД: {goods.Id}\nName/Имя: {goods.Name}\nDesc/Описание: {goods.Desc}\nCategory/Категория: {goods.Category}\nRate/Рейтинг: {goods.Rate}\nPrice/Цена: {goods.Price}\nAmount/Цена: {goods.Amount}\nOther/Прочее: {goods.Other}";
            }
            catch (Exception ex)
            {

            }
            Image image = FindName("Image") as Image;
            try
            {
                image.Source = new BitmapImage(new Uri($"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\{goods.Name}.jpg", UriKind.RelativeOrAbsolute));
            }
            catch
            {
                image.Source = new BitmapImage(new Uri($"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\img0.jpg", UriKind.RelativeOrAbsolute));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("TempGood.xml");
            this.Close();
        }
    }
}
