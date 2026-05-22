using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class SpecificationsWindow : Window
    {
        private User currentUser;

        public SpecificationsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM specifications ORDER BY spec_name";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Specification> specifications = new List<Specification>();
                foreach (DataRow row in dataTable.Rows)
                {
                    specifications.Add(new Specification
                    {
                        SpecId = Convert.ToInt32(row["spec_id"]),
                        SpecName = row["spec_name"].ToString() ?? "",
                        ProductName = row["product_name"].ToString() ?? "",
                        Quantity = Convert.ToDecimal(row["quantity"]),
                        Unit = row["unit"].ToString() ?? "",
                        Manufacturer = row["manufacturer"].ToString() ?? ""
                    });
                }

                SpecificationsDataGrid.ItemsSource = specifications;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления спецификаций будет доступна в следующей версии", 
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция редактирования спецификаций будет доступна в следующей версии", 
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpecificationsDataGrid.SelectedItem is Specification selected)
            {
                var result = MessageBox.Show($"Удалить спецификацию '{selected.SpecName}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM specifications WHERE spec_id = {selected.SpecId}";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Спецификация удалена", "Успех", 
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
                MessageBox.Show("Выберите спецификацию для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
