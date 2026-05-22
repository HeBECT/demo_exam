using System;
using System.Windows;
using System.Windows.Controls;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class AddEditCounterpartyWindow : Window
    {
        private User currentUser;
        private Counterparty? counterparty;
        private bool isEditMode;

        public AddEditCounterpartyWindow(User user, Counterparty? counterpartyToEdit = null)
        {
            InitializeComponent();
            currentUser = user;
            counterparty = counterpartyToEdit;
            isEditMode = counterparty != null;

            if (isEditMode && counterparty != null)
            {
                Title = "Редактирование контрагента";
                LoadCounterpartyData();
            }
            else
            {
                Title = "Добавление контрагента";
                TypeComboBox.SelectedIndex = 0;
            }
        }

        private void LoadCounterpartyData()
        {
            if (counterparty != null)
            {
                NameTextBox.Text = counterparty.Name;
                InnTextBox.Text = counterparty.Inn;
                KppTextBox.Text = counterparty.Kpp;
                AddressTextBox.Text = counterparty.Address;
                PhoneTextBox.Text = counterparty.Phone;
                EmailTextBox.Text = counterparty.Email;
                ContactPersonTextBox.Text = counterparty.ContactPerson;

                switch (counterparty.CounterpartyType)
                {
                    case "supplier":
                        TypeComboBox.SelectedIndex = 0;
                        break;
                    case "customer":
                        TypeComboBox.SelectedIndex = 1;
                        break;
                    case "both":
                        TypeComboBox.SelectedIndex = 2;
                        break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите наименование контрагента", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InnTextBox.Text))
            {
                MessageBox.Show("Введите ИНН контрагента", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string name = NameTextBox.Text.Trim().Replace("'", "''");
                string inn = InnTextBox.Text.Trim();
                string kpp = KppTextBox.Text.Trim();
                string address = AddressTextBox.Text.Trim().Replace("'", "''");
                string phone = PhoneTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string contactPerson = ContactPersonTextBox.Text.Trim().Replace("'", "''");
                string type = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag.ToString() ?? "supplier";

                string query;
                if (isEditMode && counterparty != null)
                {
                    query = $@"UPDATE counterparties SET 
                        name = '{name}',
                        inn = '{inn}',
                        kpp = '{kpp}',
                        address = '{address}',
                        phone = '{phone}',
                        email = '{email}',
                        contact_person = '{contactPerson}',
                        counterparty_type = '{type}'
                        WHERE counterparty_id = {counterparty.CounterpartyId}";
                }
                else
                {
                    query = $@"INSERT INTO counterparties 
                        (name, inn, kpp, address, phone, email, contact_person, counterparty_type)
                        VALUES ('{name}', '{inn}', '{kpp}', '{address}', '{phone}', 
                        '{email}', '{contactPerson}', '{type}')";
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
