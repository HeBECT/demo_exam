using System;
using System.Windows;
using System.Windows.Controls;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class AddDocumentWindow : Window
    {
        private User currentUser;

        public AddDocumentWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            DatePicker.SelectedDate = DateTime.Now;
            TypeComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                MessageBox.Show("Введите номер документа", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату документа", "Внимание", 
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
                string number = NumberTextBox.Text.Trim();
                string date = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                string type = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag.ToString() ?? "other";
                string description = DescriptionTextBox.Text.Trim().Replace("'", "''");

                string query = $@"INSERT INTO documents 
                    (document_number, document_date, document_type, total_amount, description, created_by)
                    VALUES ('{number}', '{date}', '{type}', 
                    {amount.ToString().Replace(",", ".")}, '{description}', {currentUser.UserId})";

                DatabaseHelper.ExecuteNonQuery(query);
                MessageBox.Show("Документ сохранен", "Успех", 
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
