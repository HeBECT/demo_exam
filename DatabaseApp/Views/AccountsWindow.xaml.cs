using System;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public class Account
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
    }

    public partial class AccountsWindow : Window
    {
        private User currentUser;

        public AccountsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM accounts ORDER BY account_number";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                var accounts = new System.Collections.Generic.List<Account>();
                foreach (DataRow row in dataTable.Rows)
                {
                    accounts.Add(new Account
                    {
                        AccountNumber = row["account_number"].ToString() ?? "",
                        AccountName = row["account_name"].ToString() ?? "",
                        AccountType = row["account_type"].ToString() ?? ""
                    });
                }

                AccountsDataGrid.ItemsSource = accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
