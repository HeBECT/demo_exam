using System;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class AddTransactionWindow : Window
    {
        private User currentUser;

        public AddTransactionWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату проводки", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(DebitAccountTextBox.Text))
            {
                MessageBox.Show("Введите счет дебета", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CreditAccountTextBox.Text))
            {
                MessageBox.Show("Введите счет кредита", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Введите корректную сумму", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string date = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                string debitAccount = DebitAccountTextBox.Text.Trim();
                string creditAccount = CreditAccountTextBox.Text.Trim();
                string description = DescriptionTextBox.Text.Trim().Replace("'", "''");

                string query = $@"INSERT INTO transactions 
                    (transaction_date, debit_account, credit_account, amount, description, created_by)
                    VALUES ('{date}', '{debitAccount}', '{creditAccount}', 
                    {amount.ToString().Replace(",", ".")}, '{description}', {currentUser.UserId})";

                DatabaseHelper.ExecuteNonQuery(query);
                MessageBox.Show("Проводка сохранена", "Успех", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
