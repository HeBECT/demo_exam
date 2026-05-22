using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ProizvodstvoWindow : Window
    {
        private int? _editId = null;

        public ProizvodstvoWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM proizvodstvo", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is DataRowView row)
            {
                _editId                       = Convert.ToInt32(row["id"]);
                txtNomer.Text                 = row["nomer"].ToString();
                txtData.Text                  = Convert.ToDateTime(row["data"]).ToString("yyyy-MM-dd");
                txtNazvanieProdukcii.Text     = row["nazvanie_produkcii"].ToString();
                txtKodProdukcii.Text          = row["kod_produkcii"].ToString();
                txtKolProdukcii.Text          = row["kolichestvo_produkcii"].ToString();
                txtEdProdukcii.Text           = row["edinica_produkcii"].ToString();
                txtNazvanieMateriala.Text     = row["nazvanie_materiala"].ToString();
                txtKodMateriala.Text          = row["kod_materiala"].ToString();
                txtKolMateriala.Text          = row["kolichestvo_materiala"].ToString();
                txtEdMateriala.Text           = row["edinica_materiala"].ToString();
                btnAdd.Content                = "Сохранить";
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
                        "INSERT INTO proizvodstvo " +
                        "(nomer,data,nazvanie_produkcii,kod_produkcii,kolichestvo_produkcii,edinica_produkcii," +
                        "nazvanie_materiala,kod_materiala,kolichestvo_materiala,edinica_materiala) " +
                        "VALUES (@nomer,@data,@np,@kp,@kolp,@edp,@nm,@km,@kolm,@edm)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(
                        "UPDATE proizvodstvo SET nomer=@nomer,data=@data," +
                        "nazvanie_produkcii=@np,kod_produkcii=@kp,kolichestvo_produkcii=@kolp,edinica_produkcii=@edp," +
                        "nazvanie_materiala=@nm,kod_materiala=@km,kolichestvo_materiala=@kolm,edinica_materiala=@edm " +
                        "WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", _editId);
                }
                cmd.Parameters.AddWithValue("@nomer", int.Parse(txtNomer.Text));
                cmd.Parameters.AddWithValue("@data",  DateTime.Parse(txtData.Text));
                cmd.Parameters.AddWithValue("@np",    txtNazvanieProdukcii.Text);
                cmd.Parameters.AddWithValue("@kp",    txtKodProdukcii.Text);
                cmd.Parameters.AddWithValue("@kolp",  decimal.Parse(txtKolProdukcii.Text));
                cmd.Parameters.AddWithValue("@edp",   txtEdProdukcii.Text);
                cmd.Parameters.AddWithValue("@nm",    txtNazvanieMateriala.Text);
                cmd.Parameters.AddWithValue("@km",    txtKodMateriala.Text);
                cmd.Parameters.AddWithValue("@kolm",  decimal.Parse(txtKolMateriala.Text));
                cmd.Parameters.AddWithValue("@edm",   txtEdMateriala.Text);
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
            txtNomer.Text = txtData.Text = "";
            txtNazvanieProdukcii.Text = txtKodProdukcii.Text = txtKolProdukcii.Text = txtEdProdukcii.Text = "";
            txtNazvanieMateriala.Text = txtKodMateriala.Text = txtKolMateriala.Text = txtEdMateriala.Text = "";
            btnAdd.Content = "Добавить";
            dgData.SelectedItem = null;
        }
    }
}
