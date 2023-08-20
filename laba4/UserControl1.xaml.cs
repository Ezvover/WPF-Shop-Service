using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    /// 



    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

      


        Goods goods = new Goods();

        List<Goods> goodsList = new List<Goods>();
        int freeId = 0;

        public void CheckId()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string query = "SELECT id FROM Goods";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Goods goods = new Goods();
                            goods.Id = Convert.ToInt32(reader["id"]);

                            goodsList.Add(goods);
                        }
                    }
                }
            }
        }


        public void ToClass()
        {
            CheckId();

            if (goodsList.Count > 0) 
            {
                freeId = goodsList.Last().Id;
                freeId++;
            }
            else
            {
                freeId = 0;
            }
            goods.Id = freeId;
            goods.Name = NameTextBox.Text;
            goods.Desc = DescTextBox.Text;
            goods.Category = CategoryTextBox.Text;

            goods.Rate = int.Parse(RateTextBox.Text);

            goods.Price = double.Parse(PriceTextBox.Text);

            goods.Amount = int.Parse(AmountTextBox.Text);


            string imagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\{NameTextBox.Text}.jpg";

            if (File.Exists(imagePath))
            {
                goods.Other = imagePath;
            }
            else
            {
                goods.Other = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\img0.jpg";
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string insertQuery = "INSERT INTO Goods (id, name, [desc], category, rate, price, amount, other) VALUES (@id, @name, @desc, @category, @rate, @price, @amount, @other)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Transaction = transaction;
                    try
                    {
                        command.Parameters.AddWithValue("@id", goods.Id);
                        command.Parameters.AddWithValue("@name", goods.Name);
                        command.Parameters.AddWithValue("@desc", goods.Desc);
                        command.Parameters.AddWithValue("@category", goods.Category);
                        command.Parameters.AddWithValue("@rate", goods.Rate);
                        command.Parameters.AddWithValue("@price", goods.Price);
                        command.Parameters.AddWithValue("@amount", goods.Amount);
                        command.Parameters.AddWithValue("@other", goods.Other);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ToClass();
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

    }
}
