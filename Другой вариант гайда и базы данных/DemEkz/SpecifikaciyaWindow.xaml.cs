using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class SpecifikaciyaWindow : Window
    {
        private int? _editId = null;

        public SpecifikaciyaWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM specifikaciya", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _editId                  = Convert.ToInt32(row["id"]);
                txtNazvanie.Text         = row["nazvanie"].ToString();
                txtProdukt.Text          = row["produkt"].ToString();
                txtKolichestvo.Text      = row["kolichestvo"].ToString();
                txtIzgotovitel.Text      = row["izgotovitel"].ToString();
                txtMaterial.Text         = row["material"].ToString();
                txtEdinica.Text          = row["edinica"].ToString();
                txtKolMateriala.Text     = row["kolichestvo_materiala"].ToString();
                btnAdd.Content           = "Сохранить";
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
                        "INSERT INTO specifikaciya " +
                        "(nazvanie,produkt,kolichestvo,izgotovitel,material,edinica,kolichestvo_materiala) " +
                        "VALUES (@nazvanie,@produkt,@kol,@izgotovitel,@material,@edinica,@kolmat)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE specifikaciya SET nazvanie=@nazvanie, produkt=@produkt, kolichestvo=@kol, " +
                        "izgotovitel=@izgotovitel, material=@material, edinica=@edinica, " +
                        "kolichestvo_materiala=@kolmat WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@nazvanie",    txtNazvanie.Text);
                cmd.Parameters.AddWithValue("@produkt",     txtProdukt.Text);
                cmd.Parameters.AddWithValue("@kol",         decimal.Parse(txtKolichestvo.Text));
                cmd.Parameters.AddWithValue("@izgotovitel", txtIzgotovitel.Text);
                cmd.Parameters.AddWithValue("@material",    txtMaterial.Text);
                cmd.Parameters.AddWithValue("@edinica",     txtEdinica.Text);
                cmd.Parameters.AddWithValue("@kolmat",      decimal.Parse(txtKolMateriala.Text));
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
            txtNazvanie.Text = txtProdukt.Text = txtKolichestvo.Text = txtIzgotovitel.Text = "";
            txtMaterial.Text = txtEdinica.Text = txtKolMateriala.Text = "";
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
