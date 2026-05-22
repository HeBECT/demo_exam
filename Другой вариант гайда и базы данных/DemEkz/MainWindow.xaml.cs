using System.Windows;

namespace DemEkz
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnZakazchiki_Click(object sender, RoutedEventArgs e)
        {
            new ZakazchikiWindow().Show();
        }

        private void BtnZakaz_Click(object sender, RoutedEventArgs e)
        {
            new ZakazWindow().Show();
        }

        private void BtnProizvodstvo_Click(object sender, RoutedEventArgs e)
        {
            new ProizvodstvoWindow().Show();
        }

        private void BtnSpecifikaciya_Click(object sender, RoutedEventArgs e)
        {
            new SpecifikaciyaWindow().Show();
        }

        private void BtnCeny_Click(object sender, RoutedEventArgs e)
        {
            new CenyWindow().Show();
        }

        private void BtnRaschet_Click(object sender, RoutedEventArgs e)
        {
            new RaschetWindow().Show();
        }
    }
}
