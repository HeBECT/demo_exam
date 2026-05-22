using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ZakazchikiWindow : Window
    {
        private int? _editId = null;

        public ZakazchikiWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM zakazchiki", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _editId           = Convert.ToInt32(row["id"]);
                txtKod.Text       = row["kod"].ToString();
                txtName.Text      = row["name"].ToString();
                txtInn.Text       = row["inn"].ToString();
                txtAdres.Text     = row["adres"].ToString();
                txtPhone.Text     = row["phone"].ToString();
                chkSalesman.IsChecked = row["salesman"].ToString() == "1";
                chkBuyer.IsChecked    = row["buyer"].ToString()    == "1";
                btnAdd.Content    = "Сохранить";
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
                        "INSERT INTO zakazchiki (kod, name, inn, adres, phone, salesman, buyer) " +
                        "VALUES (@kod, @name, @inn, @adres, @phone, @salesman, @buyer)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE zakazchiki SET kod=@kod, name=@name, inn=@inn, " +
                        "adres=@adres, phone=@phone, salesman=@salesman, buyer=@buyer " +
                        "WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@kod",      txtKod.Text);
                cmd.Parameters.AddWithValue("@name",     txtName.Text);
                cmd.Parameters.AddWithValue("@inn",      txtInn.Text);
                cmd.Parameters.AddWithValue("@adres",    txtAdres.Text);
                cmd.Parameters.AddWithValue("@phone",    txtPhone.Text);
                cmd.Parameters.AddWithValue("@salesman", chkSalesman.IsChecked == true ? 1 : 0);
                cmd.Parameters.AddWithValue("@buyer",    chkBuyer.IsChecked    == true ? 1 : 0);
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
            txtKod.Text = txtName.Text = txtInn.Text = txtAdres.Text = txtPhone.Text = "";
            chkSalesman.IsChecked = chkBuyer.IsChecked = false;
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
