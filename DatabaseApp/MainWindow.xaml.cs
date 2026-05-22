using System.Windows;
using DatabaseApp.Models;
using DatabaseApp.Views;

namespace DatabaseApp
{
    public partial class MainWindow : Window
    {
        private User currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            UserInfoTextBlock.Text = $"Пользователь: {currentUser.FullName} ({currentUser.Role})";
            WelcomeTextBlock.Text = $"Добро пожаловать, {currentUser.FullName}!";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CounterpartiesButton_Click(object sender, RoutedEventArgs e)
        {
            CustomersWindow window = new CustomersWindow(currentUser);
            window.ShowDialog();
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow window = new ProductsWindow(currentUser);
            window.ShowDialog();
        }

        private void AccountsButton_Click(object sender, RoutedEventArgs e)
        {
            SpecificationsWindow window = new SpecificationsWindow(currentUser);
            window.ShowDialog();
        }

        private void DocumentsButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerOrdersWindow window = new CustomerOrdersWindow(currentUser);
            window.ShowDialog();
        }

        private void TransactionsButton_Click(object sender, RoutedEventArgs e)
        {
            ProductionWindow window = new ProductionWindow(currentUser);
            window.ShowDialog();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            CostCalculationsWindow window = new CostCalculationsWindow(currentUser);
            window.ShowDialog();
        }
    }
}
