using System;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class AddEditCustomerWindow : Window
    {
        private User currentUser;
        private Customer? customer;
        private bool isEditMode;

        public AddEditCustomerWindow(User user, Customer? existingCustomer = null)
        {
            InitializeComponent();
            currentUser = user;
            customer = existingCustomer;
            isEditMode = customer != null;

            if (isEditMode && customer != null)
            {
                Title = "Редактирование заказчика";
                LoadCustomerData();
                CustomerIdTextBox.IsReadOnly = true;
            }
            else
            {
                Title = "Добавление заказчика";
            }
        }

        private void LoadCustomerData()
        {
            if (customer != null)
            {
                CustomerIdTextBox.Text = customer.CustomerId;
                NameTextBox.Text = customer.Name;
                InnTextBox.Text = customer.Inn;
                AddressTextBox.Text = customer.Address;
                PhoneTextBox.Text = customer.Phone;
                IsSalesmanCheckBox.IsChecked = customer.IsSalesman;
                IsBuyerCheckBox.IsChecked = customer.IsBuyer;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerIdTextBox.Text) || 
                string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля (ID и Название)", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string query;
                int isSalesman = IsSalesmanCheckBox.IsChecked == true ? 1 : 0;
                int isBuyer = IsBuyerCheckBox.IsChecked == true ? 1 : 0;

                if (isEditMode)
                {
                    query = $@"UPDATE customers SET 
                        name = '{NameTextBox.Text.Replace("'", "''")}',
                        inn = '{InnTextBox.Text.Replace("'", "''")}',
                        address = '{AddressTextBox.Text.Replace("'", "''")}',
                        phone = '{PhoneTextBox.Text.Replace("'", "''")}',
                        is_salesman = {isSalesman},
                        is_buyer = {isBuyer}
                        WHERE customer_id = '{CustomerIdTextBox.Text}'";
                }
                else
                {
                    query = $@"INSERT INTO customers (customer_id, name, inn, address, phone, is_salesman, is_buyer) 
                        VALUES ('{CustomerIdTextBox.Text.Replace("'", "''")}', 
                                '{NameTextBox.Text.Replace("'", "''")}', 
                                '{InnTextBox.Text.Replace("'", "''")}', 
                                '{AddressTextBox.Text.Replace("'", "''")}', 
                                '{PhoneTextBox.Text.Replace("'", "''")}', 
                                {isSalesman}, 
                                {isBuyer})";
                }

                DatabaseHelper.ExecuteNonQuery(query);
                MessageBox.Show("Данные сохранены", "Успех", 
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
