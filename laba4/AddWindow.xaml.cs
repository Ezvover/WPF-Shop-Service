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
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        Goods goods = new Goods();
        public AddWindow()
        {
            InitializeComponent();
        }

        List<Goods> goodsList = new List<Goods>();
        int freeId = 0;

        public void ToClass()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream fs = new FileStream("LastGood.xml", FileMode.OpenOrCreate))
            {
                goodsList.Add(xmlSerializer.Deserialize(fs) as Goods);
            }

            var sortedItems = goodsList.OrderBy(item => item.Id);

            foreach (var item in sortedItems)
            {
                freeId = int.Parse(item.Id);
            }

            freeId++;

            goods.Id = freeId.ToString();
            goods.Name = NameTextBox.Text;
            goods.Desc = DescTextBox.Text;
            goods.Category = CategoryTextBox.Text;
            goods.Rate = int.Parse(RateTextBox.Text);
            goods.Price = double.Parse(PriceTextBox.Text);
            goods.Amount = int.Parse(AmountTextBox.Text);
            goods.Other = OtherTextBox.Text;



        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ToClass();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"Good{freeId}.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(stream, goods);
            }
        }
    }
}
