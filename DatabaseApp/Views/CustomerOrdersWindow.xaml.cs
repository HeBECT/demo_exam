using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class CustomerOrdersWindow : Window
    {
        private User currentUser;

        public CustomerOrdersWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM customer_orders ORDER BY order_date DESC";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<CustomerOrder> orders = new List<CustomerOrder>();
                foreach (DataRow row in dataTable.Rows)
                {
                    orders.Add(new CustomerOrder
                    {
                        OrderId = Convert.ToInt32(row["order_id"]),
                        OrderNumber = row["order_number"].ToString() ?? "",
                        OrderDate = Convert.ToDateTime(row["order_date"]),
                        Executor = row["executor"].ToString() ?? "",
                        CustomerId = row["customer_id"].ToString() ?? "",
                        TotalAmount = Convert.ToDecimal(row["total_amount"])
                    });
                }

                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is CustomerOrder selected)
            {
                MessageBox.Show($"Заказ №{selected.OrderNumber}\n" +
                    $"Дата: {selected.OrderDate:dd.MM.yyyy}\n" +
                    $"Исполнитель: {selected.Executor}\n" +
                    $"Заказчик: {selected.CustomerId}\n" +
                    $"Сумма: {selected.TotalAmount:N2} руб.",
                    "Информация о заказе", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите заказ для просмотра", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
