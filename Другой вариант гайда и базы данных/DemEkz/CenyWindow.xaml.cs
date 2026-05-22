using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class CenyWindow : Window
    {
        private int? _editId = null;

        public CenyWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM ceny", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _editId          = Convert.ToInt32(row["id"]);
                txtNazvanie.Text = row["nazvanie"].ToString();
                txtCena.Text     = row["cena"].ToString();
                btnAdd.Content   = "Сохранить";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd;
                if (_editId == null)
                {
                    cmd = new MySqlCommand(
                        "INSERT INTO ceny (nazvanie, cena) VALUES (@nazvanie, @cena)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE ceny SET nazvanie=@nazvanie, cena=@cena WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@nazvanie", txtNazvanie.Text);
                cmd.Parameters.AddWithValue("@cena",     decimal.Parse(txtCena.Text));
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show(_editId == null ? "Запись добавлена!" : "Запись обновлена!");
            ClearForm();
            LoadData();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _editId = null;
            txtNazvanie.Text = txtCena.Text = "";
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
