using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class ProductionWindow : Window
    {
        private User currentUser;

        public ProductionWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM production ORDER BY production_date DESC";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Production> productions = new List<Production>();
                foreach (DataRow row in dataTable.Rows)
                {
                    productions.Add(new Production
                    {
                        ProductionId = Convert.ToInt32(row["production_id"]),
                        ProductionNumber = row["production_number"].ToString() ?? "",
                        ProductionDate = Convert.ToDateTime(row["production_date"]),
                        ProductName = row["product_name"].ToString() ?? "",
                        ProductCode = row["product_code"].ToString() ?? "",
                        Quantity = Convert.ToDecimal(row["quantity"]),
                        Unit = row["unit"].ToString() ?? ""
                    });
                }

                ProductionDataGrid.ItemsSource = productions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductionDataGrid.SelectedItem is Production selected)
            {
                MessageBox.Show($"Производство №{selected.ProductionNumber}\n" +
                    $"Дата: {selected.ProductionDate:dd.MM.yyyy}\n" +
                    $"Продукция: {selected.ProductName}\n" +
                    $"Код: {selected.ProductCode}\n" +
                    $"Количество: {selected.Quantity} {selected.Unit}",
                    "Информация о производстве", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите документ для просмотра", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
