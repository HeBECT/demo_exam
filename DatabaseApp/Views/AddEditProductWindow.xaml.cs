using System;
using System.Windows;
using System.Windows.Controls;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class AddEditProductWindow : Window
    {
        private User currentUser;
        private Product? product;
        private bool isEditMode;

        public AddEditProductWindow(User user, Product? productToEdit = null)
        {
            InitializeComponent();
            currentUser = user;
            product = productToEdit;
            isEditMode = product != null;

            if (isEditMode && product != null)
            {
                Title = "Редактирование";
                LoadProductData();
            }
            else
            {
                Title = "Добавление";
                TypeComboBox.SelectedIndex = 0;
                UnitTextBox.Text = "шт";
            }
        }

        private void LoadProductData()
        {
            if (product != null)
            {
                NameTextBox.Text = product.Name;
                UnitTextBox.Text = product.Unit;
                PriceTextBox.Text = product.Price.ToString();
                TypeComboBox.SelectedIndex = product.ProductType == "product" ? 0 : 1;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите наименование", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string name = NameTextBox.Text.Trim().Replace("'", "''");
                string unit = UnitTextBox.Text.Trim();
                string type = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag.ToString() ?? "product";

                string query;
                if (isEditMode && product != null)
                {
                    query = $@"UPDATE products SET 
                        name = '{name}',
                        unit = '{unit}',
                        price = {price.ToString().Replace(",", ".")},
                        product_type = '{type}'
                        WHERE product_id = {product.ProductId}";
                }
                else
                {
                    query = $@"INSERT INTO products (name, unit, price, product_type)
                        VALUES ('{name}', '{unit}', {price.ToString().Replace(",", ".")}, '{type}')";
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
