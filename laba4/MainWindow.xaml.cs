﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            ToGrid();
            MainGrid.IsReadOnly = true;
            List<string> strList = new List<string>();
            for (int i = 0; i < goodsList.Count; i++)
            {
                strList.Add(goodsList[i].Category);
            }
            var strList2 = strList.Distinct();
            FilterBOx.ItemsSource = strList2;


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


        public void ToGrid()
        {
            goodsList.Clear();
            MainGrid.ItemsSource = null;
            string connectionString = @"Data Source=.;Initial Catalog=GoodsLab;Integrated Security=True";
            string selectQuery = "SELECT id, name, [desc], category, rate, price, amount, other FROM Goods";
            string updateQuery = "UPDATE Category SET category = @category WHERE id = @id";
            string insertQuery = "INSERT INTO Category (id, category) VALUES (@id, @category)";

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
                            goods.Other = reader["other"].ToString();

                            goodsList.Add(goods);
                        }
                    }
                }

                foreach (Goods goods in goodsList)
                {
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", goods.Id);
                        command.Parameters.AddWithValue("@category", goods.Category);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@id", goods.Id);
                                insertCommand.Parameters.AddWithValue("@category", goods.Category);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                connection.CloseAsync();
                MainGrid.ItemsSource = goodsList;
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
                    string connectionString = @"Data Source=.;Initial Catalog=GoodsLab;Integrated Security=True";
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
            int minPrice = int.Parse(SearchText.Text);
            int maxPrice = int.Parse(SearchText2.Text);

            string connectionString = @"Data Source=.;Initial Catalog=GoodsLab;Integrated Security=True";
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


        private void Button_Click_1(object sender, RoutedEventArgs e) // search
        {
            List<Goods> tempList = new List<Goods>();
            MainGrid.ItemsSource = null;

            string connectionString = @"Data Source=.;Initial Catalog=GoodsLab;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
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
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) // add window
        {
            AddWindow add = new AddWindow();
            add.Show();
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ToGrid();
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchText3.Text;

            string connectionString = @"Data Source=.;Initial Catalog=GoodsLab;Integrated Security=True";
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
        }


        private void Button_Click_3(object sender, RoutedEventArgs e) // edit
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

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                int index = MainGrid.SelectedIndex;
                if (index > 0)
                {
                    MainGrid.SelectedIndex = index - 1;
                    MainGrid.ScrollIntoView(MainGrid.SelectedItem);
                }
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

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid_MouseDoubleClick(sender, null);
        }

    }
}
