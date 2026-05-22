using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class CounterpartiesWindow : Window
    {
        private User currentUser;

        public CounterpartiesWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM counterparties ORDER BY name";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Counterparty> counterparties = new List<Counterparty>();
                foreach (DataRow row in dataTable.Rows)
                {
                    counterparties.Add(new Counterparty
                    {
                        CounterpartyId = Convert.ToInt32(row["counterparty_id"]),
                        Name = row["name"].ToString() ?? "",
                        Inn = row["inn"].ToString() ?? "",
                        Kpp = row["kpp"].ToString() ?? "",
                        Address = row["address"].ToString() ?? "",
                        Phone = row["phone"].ToString() ?? "",
                        Email = row["email"].ToString() ?? "",
                        ContactPerson = row["contact_person"].ToString() ?? "",
                        CounterpartyType = row["counterparty_type"].ToString() ?? ""
                    });
                }

                CounterpartiesDataGrid.ItemsSource = counterparties;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditCounterpartyWindow window = new AddEditCounterpartyWindow(currentUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CounterpartiesDataGrid.SelectedItem is Counterparty selected)
            {
                AddEditCounterpartyWindow window = new AddEditCounterpartyWindow(currentUser, selected);
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите контрагента для редактирования", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CounterpartiesDataGrid.SelectedItem is Counterparty selected)
            {
                var result = MessageBox.Show($"Удалить контрагента '{selected.Name}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM counterparties WHERE counterparty_id = {selected.CounterpartyId}";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Контрагент удален", "Успех", 
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
                MessageBox.Show("Выберите контрагента для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
