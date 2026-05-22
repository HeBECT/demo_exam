using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class TransactionsWindow : Window
    {
        private User currentUser;

        public TransactionsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM transactions ORDER BY transaction_date DESC";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Transaction> transactions = new List<Transaction>();
                foreach (DataRow row in dataTable.Rows)
                {
                    transactions.Add(new Transaction
                    {
                        TransactionId = Convert.ToInt32(row["transaction_id"]),
                        TransactionDate = Convert.ToDateTime(row["transaction_date"]),
                        DebitAccount = row["debit_account"].ToString() ?? "",
                        CreditAccount = row["credit_account"].ToString() ?? "",
                        Amount = Convert.ToDecimal(row["amount"]),
                        Description = row["description"].ToString() ?? ""
                    });
                }

                TransactionsDataGrid.ItemsSource = transactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTransactionWindow window = new AddTransactionWindow(currentUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsDataGrid.SelectedItem is Transaction selected)
            {
                var result = MessageBox.Show($"Удалить проводку?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM transactions WHERE transaction_id = {selected.TransactionId}";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Проводка удалена", "Успех", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите проводку для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
