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

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow(string id)
        {
            InitializeComponent();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            List<Goods> goodsList = new List<Goods>();
            using (FileStream fs = new FileStream("goods.xml", FileMode.OpenOrCreate))
            {
                goodsList.Add(xmlSerializer.Deserialize(fs) as Goods);
            }
        }
    }
}
