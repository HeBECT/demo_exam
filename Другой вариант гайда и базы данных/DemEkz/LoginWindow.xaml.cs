using System.Windows;

namespace DemEkz
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login    = txtLogin.Text;
            string password = txtPassword.Password;

            if ((login == "admin" && password == "admin123") ||
                (login == "user"  && password == "user123"))
            {
                new MainWindow().Show();
                this.Close();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль!";
            }
        }
    }
}
