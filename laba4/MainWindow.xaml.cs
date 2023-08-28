using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Goods> goodsList = new List<Goods>();
        List<string> strList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            ToGrid();
          
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
            MainGrid.IsReadOnly = true;

            for (int i = 0; i < goodsList.Count; i++)
            {
                strList.Add(goodsList[i].Category);
            }
            var strList2 = strList.Distinct();
            FilterBOx.ItemsSource = strList2;

          
        }

        public void ToGrid()
        {
            goodsList.Clear();
            MainGrid.ItemsSource = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string selectQuery = "SELECT id, name, [desc], category, rate, price, amount, other FROM Goods";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Goods goods = new Goods();
                            goods.Id = Convert.ToInt32(reader["id"]);
                            goods.Name = reader["name"].ToString();
                            goods.Desc = reader["desc"].ToString();
                            goods.Category = reader["category"].ToString();
                            goods.Rate = Convert.ToInt32(reader["rate"]);
                            goods.Price = Convert.ToDouble(reader["price"]);
                            goods.Amount = Convert.ToInt32(reader["amount"]);

                            string imagePath = reader["other"].ToString();
                            if (File.Exists(imagePath))
                            {
                                goods.Other = new Uri(imagePath).ToString();
                            }
                            goodsList.Add(goods);
                        }
                    }
                }

                connection.CloseAsync();
                MainGrid.ItemsSource = goodsList;
            }

            strList.Clear();
            for (int i = 0; i < goodsList.Count; i++)
            {
                strList.Add(goodsList[i].Category);
            }
            var strList2 = strList.Distinct();
            FilterBOx.ItemsSource = strList2;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // delete button
        {
            if (MainGrid.SelectedItem!= null) 
            {
                var selectedObject = MainGrid.SelectedItem as Goods;
                MainGrid.ItemsSource = null;
                for (int i = 0; i < goodsList.Count; i++)
                {
                    if (goodsList[i].Equals(selectedObject))
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
                        string deleteGoodsQuery = "DELETE FROM Goods WHERE id = @id";
                        string deleteCatalogQuery = "DELETE FROM Category WHERE id = @id";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction();
                            try
                            {
                                using (SqlCommand command = new SqlCommand(deleteCatalogQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@id", goodsList[i].Id);
                                    command.ExecuteNonQuery();
                                }

                                using (SqlCommand command = new SqlCommand(deleteGoodsQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@id", goodsList[i].Id);
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

                        goodsList.Remove(goodsList[i]);
                    }
                }
                MainGrid.ItemsSource = goodsList;
                MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) // sortByPrice
        {
            try
            {
                int minPrice = int.Parse(SearchText.Text);
                int maxPrice = int.Parse(SearchText2.Text);

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
                string query = $"SELECT id, name, [desc], category, rate, price, amount, other FROM Goods WHERE price >= {minPrice} AND price <= {maxPrice}";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Goods> tempList = new List<Goods>();

                            while (reader.Read())
                            {
                                Goods goods = new Goods();
                                goods.Id = Convert.ToInt32(reader["id"]);
                                goods.Name = reader["name"].ToString();
                                goods.Desc = reader["desc"].ToString();
                                goods.Category = reader["category"].ToString();
                                goods.Rate = Convert.ToInt32(reader["rate"]);
                                goods.Price = Convert.ToDouble(reader["price"]);
                                goods.Amount = Convert.ToInt32(reader["amount"]);
                                goods.Other = reader["other"].ToString();

                                tempList.Add(goods);
                            }

                            MainGrid.ItemsSource = tempList;
                        }
                    }
                    connection.CloseAsync();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
        }


        private void Button_Click_1(object sender, RoutedEventArgs e) // CategorySort
        {
            if (FilterBOx.SelectedItem!= null)
            {
                List<Goods> tempList = new List<Goods>();
                MainGrid.ItemsSource = null;

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // процедура
                    try
                    {
                        string storedProcedure = @"
                    CREATE PROCEDURE GetGoodsByCategory
                        @Category nvarchar(50)
                    AS
                    BEGIN
                        SELECT id, name, [desc], category, rate, price, amount, other FROM Goods WHERE category = @Category;
                    END";
                        using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch
                    {

                    }

                    using (SqlCommand command = new SqlCommand("GetGoodsByCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Category", FilterBOx.Text));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Goods goods = new Goods();
                                goods.Id = reader.GetInt32(0);
                                goods.Name = reader.GetString(1);
                                goods.Desc = reader.GetString(2);
                                goods.Category = reader.GetString(3);
                                goods.Rate = reader.GetInt32(4);
                                goods.Price = reader.GetInt32(5);
                                goods.Amount = reader.GetInt32(6);
                                goods.Other = reader.GetString(7);
                                tempList.Add(goods);
                            }
                        }
                    }
                }
                MainGrid.ItemsSource = tempList;
                MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) // add window
        {
            AddWindow add = new AddWindow();
            add.Show();
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ToGrid();
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e) // PartSearch
        {
            string searchText = SearchText3.Text;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
            string query = $"SELECT id, name, [desc], category, rate, price, amount, other FROM Goods WHERE " +
                $"name LIKE '%{searchText}%' OR " +
                $"[desc] LIKE '%{searchText}%' OR " +
                $"other LIKE '%{searchText}%' OR " +
                $"category LIKE '%{searchText}%' OR " +
                $"CONVERT(NVARCHAR, price) LIKE '%{searchText}%' OR " +
                $"CONVERT(NVARCHAR, amount) LIKE '%{searchText}%'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Goods> tempList2 = new List<Goods>();

                        while (reader.Read())
                        {
                            Goods goods = new Goods();
                            goods.Id = Convert.ToInt32(reader["id"]);
                            goods.Name = reader["name"].ToString();
                            goods.Desc = reader["desc"].ToString();
                            goods.Category = reader["category"].ToString();
                            goods.Rate = Convert.ToInt32(reader["rate"]);
                            goods.Price = Convert.ToInt32(reader["price"]);
                            goods.Amount = Convert.ToInt32(reader["amount"]);
                            goods.Other = reader["other"].ToString();

                            tempList2.Add(goods);
                        }

                        MainGrid.ItemsSource = tempList2;
                    }
                }
                connection.CloseAsync();
            }
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
        }


        private void Button_Click_3(object sender, RoutedEventArgs e) // edit
        {
            try
            {
                File.Delete("TempEditGood.xml");
            }
            catch
            {

            }
            if (MainGrid.SelectedItem!= null) 
            {
                var selectedObject = MainGrid.SelectedItem as Goods;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
                using (FileStream stream = new FileStream($"TempEditGood.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(stream, selectedObject);
                }

                EditWindow editWindow = new EditWindow();
                editWindow.Show();
            }
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                int index = MainGrid.SelectedIndex;
                if (index < MainGrid.Items.Count - 1)
                {
                    MainGrid.SelectedIndex = index + 1;
                    MainGrid.ScrollIntoView(MainGrid.SelectedItem);
                }
            }
        }
    }
}
