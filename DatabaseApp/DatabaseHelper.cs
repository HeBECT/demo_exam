using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DatabaseApp
{
    public class DatabaseHelper
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounting.db");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        static DatabaseHelper()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(dbPath);

            if (isNewDatabase)
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
                InsertInitialData();
            }
        }

        private static void CreateTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                string[] createTableQueries = new string[]
                {
                    // Таблица пользователей
                    @"CREATE TABLE IF NOT EXISTS users (
                        user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        username TEXT NOT NULL UNIQUE,
                        password_hash TEXT NOT NULL,
                        role TEXT NOT NULL DEFAULT 'user',
                        full_name TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        last_login DATETIME
                    )",

                    // Таблица заказчиков (из Заказчики.json)
                    @"CREATE TABLE IF NOT EXISTS customers (
                        customer_id TEXT PRIMARY KEY,
                        name TEXT NOT NULL,
                        inn TEXT,
                        address TEXT,
                        phone TEXT,
                        is_salesman INTEGER DEFAULT 0,
                        is_buyer INTEGER DEFAULT 0,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",

                    // Таблица продукции и материалов (из Цены.xlsx)
                    @"CREATE TABLE IF NOT EXISTS products (
                        product_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL UNIQUE,
                        price REAL NOT NULL,
                        unit TEXT DEFAULT 'шт',
                        product_type TEXT DEFAULT 'product',
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",

                    // Таблица заказов покупателей (из Заказ покупателя.xlsx)
                    @"CREATE TABLE IF NOT EXISTS customer_orders (
                        order_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        order_number TEXT NOT NULL,
                        order_date DATE NOT NULL,
                        executor TEXT NOT NULL,
                        customer_id TEXT,
                        total_amount REAL DEFAULT 0,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
                    )",

                    // Таблица позиций заказа
                    @"CREATE TABLE IF NOT EXISTS order_items (
                        item_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        order_id INTEGER NOT NULL,
                        product_name TEXT NOT NULL,
                        quantity REAL NOT NULL,
                        unit TEXT NOT NULL,
                        price REAL NOT NULL,
                        amount REAL NOT NULL,
                        FOREIGN KEY (order_id) REFERENCES customer_orders(order_id) ON DELETE CASCADE
                    )",

                    // Таблица производства (из Производство.xlsx)
                    @"CREATE TABLE IF NOT EXISTS production (
                        production_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        production_number TEXT NOT NULL,
                        production_date DATE NOT NULL,
                        product_name TEXT NOT NULL,
                        product_code TEXT,
                        quantity REAL NOT NULL,
                        unit TEXT NOT NULL,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",

                    // Таблица материалов для производства
                    @"CREATE TABLE IF NOT EXISTS production_materials (
                        material_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        production_id INTEGER NOT NULL,
                        material_name TEXT NOT NULL,
                        material_code TEXT,
                        quantity REAL NOT NULL,
                        unit TEXT NOT NULL,
                        FOREIGN KEY (production_id) REFERENCES production(production_id) ON DELETE CASCADE
                    )",

                    // Таблица спецификаций (из Спецификация.xlsx)
                    @"CREATE TABLE IF NOT EXISTS specifications (
                        spec_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        spec_name TEXT NOT NULL,
                        product_name TEXT NOT NULL,
                        quantity REAL NOT NULL,
                        unit TEXT NOT NULL,
                        manufacturer TEXT,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )",

                    // Таблица материалов спецификации
                    @"CREATE TABLE IF NOT EXISTS specification_materials (
                        spec_material_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        spec_id INTEGER NOT NULL,
                        material_name TEXT NOT NULL,
                        quantity REAL NOT NULL,
                        unit TEXT NOT NULL,
                        FOREIGN KEY (spec_id) REFERENCES specifications(spec_id) ON DELETE CASCADE
                    )",

                    // Таблица расчета стоимости (из Расчет стоимости продукции.xlsx)
                    @"CREATE TABLE IF NOT EXISTS cost_calculations (
                        calc_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        product_name TEXT NOT NULL,
                        material_name TEXT,
                        quantity REAL,
                        unit TEXT,
                        price REAL,
                        cost REAL NOT NULL,
                        calc_type TEXT NOT NULL,
                        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                    )"
                };

                using (var command = connection.CreateCommand())
                {
                    foreach (var query in createTableQueries)
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void InsertInitialData()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Вставка пользователей
                        ExecuteNonQuery(connection, @"
                            INSERT INTO users (username, password_hash, role, full_name) VALUES
                            ('admin', 'admin123', 'admin', 'Администратор системы'),
                            ('user', 'user123', 'user', 'Пользователь системы')
                        ");

                        // Вставка заказчиков из JSON
                        ExecuteNonQuery(connection, @"
                            INSERT INTO customers (customer_id, name, inn, address, phone, is_salesman, is_buyer) VALUES
                            ('000000001', 'ООО ""Поставка""', '', 'г.Пятигорск', '+79198634592', 1, 1),
                            ('000000002', 'ООО ""Кинотеатр Квант""', '26320045123', 'г. Железноводск, ул. Мира, 123', '+79884581555', 1, 0),
                            ('000000008', 'ООО ""Новый JDTO""', '26320045111', 'г. Железноводсу', '+79884581555', 1, 0),
                            ('000000003', 'ООО ""Ромашка""', '4140784214', 'г. Омск, ул. Строителей, 294', '+79882584546', 0, 1),
                            ('000000009', 'ООО ""Ипподром""', '5874045632', 'г. Уфа, ул. Набережная,  37', '+79627486389', 1, 1),
                            ('000000010', 'ООО ""Ассоль""', '2629011278', 'г. Калуга, ул. Пушкина, 94', '+79184572398', 0, 1)
                        ");

                        // Вставка продукции и материалов из Цены.xlsx
                        ExecuteNonQuery(connection, @"
                            INSERT INTO products (name, price, unit, product_type) VALUES
                            ('Закваска сметанная', 45, 'кг', 'material'),
                            ('Кефир 2,5% 900г.', 80, 'шт', 'product'),
                            ('Кефир 3,2% 900г.', 82, 'шт', 'product'),
                            ('Молоко 2,5% 900г.', 70, 'шт', 'product'),
                            ('Молоко 3,2% 900г.', 76, 'шт', 'product'),
                            ('Молоко нормализованное', 34, 'кг', 'material'),
                            ('Сметана классическая 15% 540г.', 89, 'шт', 'product'),
                            ('Сметана классическая 20% 540г.', 92, 'шт', 'product')
                        ");

                        // Вставка заказа покупателя
                        ExecuteNonQuery(connection, @"
                            INSERT INTO customer_orders (order_number, order_date, executor, customer_id, total_amount) VALUES
                            ('2', '2025-06-06', 'ООО Молочный комбинат ""Полесье""', '000000010', 2488)
                        ");

                        // Вставка позиций заказа
                        ExecuteNonQuery(connection, @"
                            INSERT INTO order_items (order_id, product_name, quantity, unit, price, amount) VALUES
                            (1, 'Кефир 2,5% 900г.', 12, 'шт', 80, 960),
                            (1, 'Кефир 3,2% 900г.', 9, 'шт', 82, 738),
                            (1, 'Молоко 2,5% 900г.', 10, 'шт', 79, 790)
                        ");

                        // Вставка производства
                        ExecuteNonQuery(connection, @"
                            INSERT INTO production (production_number, production_date, product_name, product_code, quantity, unit) VALUES
                            ('1', '2025-06-09', 'Сметана классическая 15% 540г.', 'НФ-00000006', 1, 'шт')
                        ");

                        // Вставка материалов производства
                        ExecuteNonQuery(connection, @"
                            INSERT INTO production_materials (production_id, material_name, material_code, quantity, unit) VALUES
                            (1, 'Молоко нормализованное', 'НФ-00000004', 0.9, 'кг'),
                            (1, 'Закваска сметанная', 'НФ-00000005', 0.07, 'кг')
                        ");

                        // Вставка спецификации
                        ExecuteNonQuery(connection, @"
                            INSERT INTO specifications (spec_name, product_name, quantity, unit, manufacturer) VALUES
                            ('Основная Сметана 15%', 'Сметана классическая 15% 540г.', 1, 'шт', 'ООО Молочный комбинат ""Полесье""')
                        ");

                        // Вставка материалов спецификации
                        ExecuteNonQuery(connection, @"
                            INSERT INTO specification_materials (spec_id, material_name, quantity, unit) VALUES
                            (1, 'Молоко нормализованное', 0.9, 'кг'),
                            (1, 'Закваска сметанная', 0.07, 'кг')
                        ");

                        // Вставка расчета стоимости
                        ExecuteNonQuery(connection, @"
                            INSERT INTO cost_calculations (product_name, material_name, quantity, unit, price, cost, calc_type) VALUES
                            ('Сметана классическая 15% 540г.', NULL, 1, 'шт', NULL, 36.7, 'product'),
                            ('Сметана классическая 15% 540г.', 'Закваска сметанная', 0.07, 'кг', 10, 0.7, 'material'),
                            ('Сметана классическая 15% 540г.', 'Молоко нормализованное', 0.9, 'кг', 40, 36, 'material')
                        ");

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Ошибка при вставке начальных данных: " + ex.Message);
                    }
                }
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static DataTable ExecuteQuery(string query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        // Метод для проверки учетных данных пользователя
        public static bool ValidateUser(string username, string password, out string? role)
        {
            role = null;
            string query = $"SELECT role FROM users WHERE username = '{username}' AND password_hash = '{password}'";
            
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        role = result.ToString() ?? "";
                        
                        // Обновляем время последнего входа
                        string updateQuery = $"UPDATE users SET last_login = datetime('now') WHERE username = '{username}'";
                        ExecuteNonQuery(updateQuery);
                        
                        return true;
                    }
                }
            }
            
            return false;
        }

        // Метод для проверки подключения к базе данных
        public static void TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    // Просто проверяем, что можем открыть соединение
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось подключиться к базе данных: {ex.Message}");
            }
        }
    }
}
