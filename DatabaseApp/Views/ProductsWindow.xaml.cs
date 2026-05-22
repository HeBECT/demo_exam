using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class ProductsWindow : Window
    {
        private User currentUser;

        public ProductsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM products ORDER BY name";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Product> products = new List<Product>();
                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(row["product_id"]),
                        Name = row["name"].ToString() ?? "",
                        Unit = row["unit"].ToString() ?? "",
                        Price = Convert.ToDecimal(row["price"]),
                        ProductType = row["product_type"].ToString() ?? ""
                    });
                }

                ProductsDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditProductWindow window = new AddEditProductWindow(currentUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selected)
            {
                AddEditProductWindow window = new AddEditProductWindow(currentUser, selected);
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите позицию для редактирования", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selected)
            {
                var result = MessageBox.Show($"Удалить '{selected.Name}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM products WHERE product_id = {selected.ProductId}";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Позиция удалена", "Успех", 
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
                MessageBox.Show("Выберите позицию для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
