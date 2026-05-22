using System;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp
{
    public partial class LoginWindow : Window
    {
        public User? CurrentUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            
            // Инициализация базы данных происходит автоматически при первом обращении
            try
            {
                DatabaseHelper.TestConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Введите имя пользователя и пароль";
                return;
            }

            try
            {
                // Проверка учетных данных
                string query = $"SELECT * FROM users WHERE username = '{username}' AND password_hash = '{password}'";
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    CurrentUser = new User
                    {
                        UserId = Convert.ToInt32(row["user_id"]),
                        Username = row["username"].ToString() ?? "",
                        Role = row["role"].ToString() ?? "",
                        FullName = row["full_name"].ToString() ?? ""
                    };

                    // Обновление времени последнего входа
                    string updateQuery = $"UPDATE users SET last_login = datetime('now') WHERE user_id = {CurrentUser.UserId}";
                    DatabaseHelper.ExecuteNonQuery(updateQuery);

                    // Открытие главного окна
                    MainWindow mainWindow = new MainWindow(CurrentUser);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorTextBlock.Text = "Неверное имя пользователя или пароль";
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }
    }
}
