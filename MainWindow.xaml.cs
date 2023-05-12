// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Npgsql;

namespace DeagencyWin
{
    public sealed partial class MainWindow : Window
    {
        private NpgsqlConnection connection;
        private NpgsqlDataAdapter dataAdapter;
        private DataTable dataTable;
        public MainWindow()
        {
            this.InitializeComponent();
            ConnectToDatabase();
            PopulateComboBox();
        }

        private void ConnectToDatabase()
        {
            // Replace with your actual connection string
            const string connectionString = "Host=192.168.1.17;Database=postgres;Username=postgres;Password=9024;";
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
        }

        private void PopulateComboBox()
        {
            const string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
            dataAdapter = new NpgsqlDataAdapter(query, connection);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                tableComboBox.Items.Add(row["table_name"]);
            }
        }

        private void tableComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string tableName = tableComboBox.SelectedValue.ToString();
            var res = LoadDataAsync(tableName);
        }
        
        private async Task LoadDataAsync(string tableName)
        {
            var dataTable = new DataTable();

            string connectionString = "Host=192.168.1.17;Username=postgres;Password=9024;Database=postgres";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = $"SELECT * FROM {tableName};";
                using (var dataAdapter = new NpgsqlDataAdapter(query, connection))
                {
                    dataAdapter.Fill(dataTable);
                }
            }

            dataGrid.ItemsSource = dataTable.DefaultView;
        }

    }
}
