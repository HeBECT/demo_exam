using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class RaschetWindow : Window
    {
        private int? _editId = null;

        public RaschetWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM raschet_stoimosti", conn);
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
                txtTip.Text      = row["tip"].ToString();
                txtStoimost.Text = row["stoimost"].ToString();
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
                        "INSERT INTO raschet_stoimosti (tip, stoimost) VALUES (@tip, @stoimost)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE raschet_stoimosti SET tip=@tip, stoimost=@stoimost WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@tip",      txtTip.Text);
                cmd.Parameters.AddWithValue("@stoimost", decimal.Parse(txtStoimost.Text));
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
            txtTip.Text = txtStoimost.Text = "";
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
