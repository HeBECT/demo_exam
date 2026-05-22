using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ZakazWindow : Window
    {
        private int? _editId = null;

        public ZakazWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM zakaz_pokupatelya", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _editId                = Convert.ToInt32(row["id"]);
                txtNomer.Text          = row["nomer"].ToString();
                txtData.Text           = Convert.ToDateTime(row["data"]).ToString("yyyy-MM-dd");
                txtIspolnitel.Text     = row["ispolnitel"].ToString();
                txtZakazchik.Text      = row["zakazchik"].ToString();
                txtProdukciya.Text     = row["produkciya"].ToString();
                txtKolichestvo.Text    = row["kolichestvo"].ToString();
                txtEdinica.Text        = row["edinica"].ToString();
                txtCena.Text           = row["cena"].ToString();
                txtSumma.Text          = row["summa"].ToString();
                btnAdd.Content         = "Сохранить";
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
                        "INSERT INTO zakaz_pokupatelya " +
                        "(nomer, data, ispolnitel, zakazchik, produkciya, kolichestvo, edinica, cena, summa) " +
                        "VALUES (@nomer,@data,@ispolnitel,@zakazchik,@produkciya,@kolichestvo,@edinica,@cena,@summa)",
                        conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE zakaz_pokupatelya SET nomer=@nomer, data=@data, ispolnitel=@ispolnitel, " +
                        "zakazchik=@zakazchik, produkciya=@produkciya, kolichestvo=@kolichestvo, " +
                        "edinica=@edinica, cena=@cena, summa=@summa WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@nomer",       int.Parse(txtNomer.Text));
                cmd.Parameters.AddWithValue("@data",        DateTime.Parse(txtData.Text));
                cmd.Parameters.AddWithValue("@ispolnitel",  txtIspolnitel.Text);
                cmd.Parameters.AddWithValue("@zakazchik",   txtZakazchik.Text);
                cmd.Parameters.AddWithValue("@produkciya",  txtProdukciya.Text);
                cmd.Parameters.AddWithValue("@kolichestvo", decimal.Parse(txtKolichestvo.Text));
                cmd.Parameters.AddWithValue("@edinica",     txtEdinica.Text);
                cmd.Parameters.AddWithValue("@cena",        decimal.Parse(txtCena.Text));
                cmd.Parameters.AddWithValue("@summa",       decimal.Parse(txtSumma.Text));
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
            txtNomer.Text = txtData.Text = txtIspolnitel.Text = txtZakazchik.Text = "";
            txtProdukciya.Text = txtKolichestvo.Text = txtEdinica.Text = txtCena.Text = txtSumma.Text = "";
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
