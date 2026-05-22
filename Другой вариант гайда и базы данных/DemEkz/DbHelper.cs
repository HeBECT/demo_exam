using MySql.Data.MySqlClient;

namespace DemEkz
{
    public static class DbHelper
    {
        // ВАЖНО: замените Password=root на ваш пароль MySQL
        public static string ConnectionString =
            "Server=localhost;Database=demekz;User Id=root;Password=1234;charset=utf8;SslMode=None;AllowPublicKeyRetrieval=True;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
