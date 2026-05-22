using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class CostCalculationsWindow : Window
    {
        private User currentUser;

        public CostCalculationsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM cost_calculations ORDER BY product_name, calc_type";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<CostCalculation> calculations = new List<CostCalculation>();
                foreach (DataRow row in dataTable.Rows)
                {
                    calculations.Add(new CostCalculation
                    {
                        CalcId = Convert.ToInt32(row["calc_id"]),
                        ProductName = row["product_name"].ToString() ?? "",
                        MaterialName = row["material_name"]?.ToString() ?? "",
                        Quantity = row["quantity"] != DBNull.Value ? Convert.ToDecimal(row["quantity"]) : (decimal?)null,
                        Unit = row["unit"]?.ToString() ?? "",
                        Price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : (decimal?)null,
                        Cost = Convert.ToDecimal(row["cost"]),
                        CalcType = row["calc_type"].ToString() ?? ""
                    });
                }

                CalculationsDataGrid.ItemsSource = calculations;
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
