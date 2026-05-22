using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class CustomersWindow : Window
    {
        private User currentUser;

        public CustomersWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM customers ORDER BY name";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Customer> customers = new List<Customer>();
                foreach (DataRow row in dataTable.Rows)
                {
                    customers.Add(new Customer
                    {
                        CustomerId = row["customer_id"].ToString() ?? "",
                        Name = row["name"].ToString() ?? "",
                        Inn = row["inn"].ToString() ?? "",
                        Address = row["address"].ToString() ?? "",
                        Phone = row["phone"].ToString() ?? "",
                        IsSalesman = Convert.ToInt32(row["is_salesman"]) == 1,
                        IsBuyer = Convert.ToInt32(row["is_buyer"]) == 1
                    });
                }

                CustomersDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditCustomerWindow window = new AddEditCustomerWindow(currentUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selected)
            {
                AddEditCustomerWindow window = new AddEditCustomerWindow(currentUser, selected);
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите заказчика для редактирования", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selected)
            {
                var result = MessageBox.Show($"Удалить заказчика '{selected.Name}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM customers WHERE customer_id = '{selected.CustomerId}'";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Заказчик удален", "Успех", 
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
                MessageBox.Show("Выберите заказчика для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
