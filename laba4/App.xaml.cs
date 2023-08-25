using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace laba4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string connectionString = @"Data Source=.;Initial Catalog=master;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    string createDatabaseQuery = "CREATE DATABASE GoodsLab";
                    using (SqlCommand command = new SqlCommand(createDatabaseQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1801) // Database already exists
                    {
                        // Do nothing
                    }
                    else
                    {
                        throw;
                    }
                }
                connection.ChangeDatabase("GoodsLab");
                try
                {
                    string createTableQuery = @"
        CREATE TABLE Goods
        (
            id int PRIMARY KEY,
            name nvarchar(50),
            [desc] nvarchar(50),
            category nvarchar(50),
            rate int,
            price int,
            amount int,
            other nvarchar(max),
            picture image
        );";
                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2714) // Table already exists
                    {
                        // Do nothing
                    }
                    else
                    {
                        throw;
                    }
                }
                try
                {
                    string createTableQuery = @"
        CREATE TABLE Category
        (
        id int PRIMARY KEY FOREIGN KEY REFERENCES Goods(id),
        category nvarchar(50),
        );";
                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2714) // Table already exists
                    {
                        // Do nothing
                    }
                    else
                    {
                        throw;
                    }
                }
                try
                {
                    string createProcedureQuery = @"
        CREATE PROCEDURE usp_GetGoods
        AS
        BEGIN
            SELECT id FROM Goods;
        END;";
                    using (SqlCommand command = new SqlCommand(createProcedureQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
}
