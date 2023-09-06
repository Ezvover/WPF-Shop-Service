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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        private Goods selectedGoods;

        int id = 0;

        Goods goods = new Goods();

        public EditWindow()
        {
            InitializeComponent();


            Goods goods2;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"TempEditGood.xml", FileMode.OpenOrCreate))
            {
                goods2 = (xmlSerializer.Deserialize(stream) as Goods);
            }
            id = goods2.Id;
            LoadDB();
            ToText();
        }

        List<Goods> goodsList = new List<Goods>();

        public void LoadDB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string query = $"SELECT id, name, [desc], category, rate, price, amount, other FROM Goods WHERE id = {id}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            goods.Id = Convert.ToInt32(reader["id"]);
                            goods.Name = reader["name"].ToString();
                            goods.Desc = reader["desc"].ToString();
                            goods.Category = reader["category"].ToString();
                            goods.Rate = Convert.ToInt32(reader["rate"]);
                            goods.Price = Convert.ToInt32(reader["price"]);
                            goods.Amount = Convert.ToInt32(reader["amount"]);
                            goods.Other = reader["other"].ToString();

                            

                            goodsList.Add(goods);
                        }
                    }
                }
                connection.CloseAsync();
            }
        }

        public void ToText()
        {

            NameTextBox.Text = goodsList[0].Name.ToString();
            DescTextBox.Text = goodsList[0].Desc.ToString();
            CategoryTextBox.Text = goodsList[0].Category.ToString();
            RateTextBox.Text = goodsList[0].Rate.ToString();
            PriceTextBox.Text = goodsList[0].Price.ToString();
            AmountTextBox.Text = goodsList[0].Amount.ToString();

          

        }

        private void UpdateDB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string updateGoodsQuery = $"UPDATE Goods SET name = @Name, [desc] = @Desc, category = @Category, rate = @Rate, price = @Price, amount = @Amount, other = @Other, picture = @Picture WHERE id = @id";
            string updateCatalogQuery = "UPDATE Category SET Category = (SELECT category FROM Goods WHERE id = @id) WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand(updateGoodsQuery, connection, transaction))
                    {
                        string imagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\{NameTextBox.Text}.jpg";
                        string defaultImagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\img0.jpg";
                        byte[] imageBytes;

                        if (File.Exists(imagePath))
                        {
                            goods.Other = imagePath;
                            imageBytes = File.ReadAllBytes(imagePath);
                        }
                        else
                        {
                            goods.Other = defaultImagePath;
                            imageBytes = File.ReadAllBytes(defaultImagePath);
                        }

                        goods.Picture = imageBytes;

                        command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                        command.Parameters.AddWithValue("@Desc", DescTextBox.Text);
                        command.Parameters.AddWithValue("@Category", CategoryTextBox.Text);
                        command.Parameters.AddWithValue("@Rate", RateTextBox.Text);
                        command.Parameters.AddWithValue("@Price", PriceTextBox.Text);
                        command.Parameters.AddWithValue("@Amount", AmountTextBox.Text);
                        command.Parameters.AddWithValue("@Other", goods.Other);
                        command.Parameters.AddWithValue("@Picture", goods.Picture);
                        command.Parameters.AddWithValue("@id", goodsList[0].Id);

                        command.ExecuteNonQuery();
                    }

                        using (SqlCommand command = new SqlCommand(updateCatalogQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", goodsList[0].Id);

                            command.ExecuteNonQuery();
                        }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(RateTextBox.Text) < 0) || (double.Parse(PriceTextBox.Text) < 0) || (int.Parse(AmountTextBox.Text) < 0) || string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CategoryTextBox.Text))
            {
                MessageBox.Show("Ошибка");
            }
            else
            {
                UpdateDB();
                File.Delete("TempEditGood.xml");
                this.Close();
            }
        }
    }
}

