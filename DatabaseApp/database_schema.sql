-- База данных для бухгалтерского учета
-- Создание базы данных
CREATE DATABASE IF NOT EXISTS accounting_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE accounting_db;

-- Таблица пользователей системы
CREATE TABLE IF NOT EXISTS users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('admin', 'user') NOT NULL DEFAULT 'user',
    full_name VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP NULL
);

-- Таблица организаций
CREATE TABLE IF NOT EXISTS organizations (
    org_id INT AUTO_INCREMENT PRIMARY KEY,
    org_name VARCHAR(200) NOT NULL,
    inn VARCHAR(12) NOT NULL UNIQUE,
    kpp VARCHAR(9),
    address TEXT,
    phone VARCHAR(20),
    email VARCHAR(100),
    director VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица контрагентов
CREATE TABLE IF NOT EXISTS counterparties (
    counterparty_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    inn VARCHAR(12) NOT NULL,
    kpp VARCHAR(9),
    address TEXT,
    phone VARCHAR(20),
    email VARCHAR(100),
    contact_person VARCHAR(100),
    counterparty_type ENUM('supplier', 'customer', 'both') NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица счетов бухгалтерского учета
CREATE TABLE IF NOT EXISTS accounts (
    account_id INT AUTO_INCREMENT PRIMARY KEY,
    account_number VARCHAR(10) NOT NULL UNIQUE,
    account_name VARCHAR(200) NOT NULL,
    account_type ENUM('active', 'passive', 'active-passive') NOT NULL,
    parent_account VARCHAR(10),
    is_synthetic BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица документов
CREATE TABLE IF NOT EXISTS documents (
    document_id INT AUTO_INCREMENT PRIMARY KEY,
    document_number VARCHAR(50) NOT NULL,
    document_date DATE NOT NULL,
    document_type ENUM('invoice', 'payment', 'receipt', 'expense', 'other') NOT NULL,
    counterparty_id INT,
    total_amount DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    description TEXT,
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (counterparty_id) REFERENCES counterparties(counterparty_id) ON DELETE SET NULL,
    FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_document_date (document_date),
    INDEX idx_document_number (document_number)
);

-- Таблица проводок
CREATE TABLE IF NOT EXISTS transactions (
    transaction_id INT AUTO_INCREMENT PRIMARY KEY,
    transaction_date DATE NOT NULL,
    document_id INT,
    debit_account VARCHAR(10) NOT NULL,
    credit_account VARCHAR(10) NOT NULL,
    amount DECIMAL(15, 2) NOT NULL,
    description TEXT,
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (document_id) REFERENCES documents(document_id) ON DELETE CASCADE,
    FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_transaction_date (transaction_date),
    INDEX idx_debit_account (debit_account),
    INDEX idx_credit_account (credit_account)
);

-- Таблица товаров и услуг
CREATE TABLE IF NOT EXISTS products (
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    product_code VARCHAR(50) UNIQUE,
    product_name VARCHAR(200) NOT NULL,
    unit VARCHAR(20) NOT NULL DEFAULT 'шт',
    price DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    vat_rate DECIMAL(5, 2) NOT NULL DEFAULT 20.00,
    product_type ENUM('goods', 'service') NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблица складских операций
CREATE TABLE IF NOT EXISTS inventory_operations (
    operation_id INT AUTO_INCREMENT PRIMARY KEY,
    operation_date DATE NOT NULL,
    operation_type ENUM('receipt', 'shipment', 'writeoff', 'inventory') NOT NULL,
    document_id INT,
    product_id INT NOT NULL,
    quantity DECIMAL(15, 3) NOT NULL,
    price DECIMAL(15, 2) NOT NULL,
    total_amount DECIMAL(15, 2) NOT NULL,
    warehouse VARCHAR(100),
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (document_id) REFERENCES documents(document_id) ON DELETE SET NULL,
    FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE RESTRICT,
    FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_operation_date (operation_date),
    INDEX idx_product_id (product_id)
);

-- Таблица банковских счетов
CREATE TABLE IF NOT EXISTS bank_accounts (
    bank_account_id INT AUTO_INCREMENT PRIMARY KEY,
    account_number VARCHAR(20) NOT NULL UNIQUE,
    bank_name VARCHAR(200) NOT NULL,
    bik VARCHAR(9) NOT NULL,
    correspondent_account VARCHAR(20),
    currency VARCHAR(3) NOT NULL DEFAULT 'RUB',
    org_id INT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (org_id) REFERENCES organizations(org_id) ON DELETE CASCADE
);

-- Таблица кассовых операций
CREATE TABLE IF NOT EXISTS cash_operations (
    cash_operation_id INT AUTO_INCREMENT PRIMARY KEY,
    operation_date DATE NOT NULL,
    operation_type ENUM('receipt', 'expense') NOT NULL,
    document_id INT,
    amount DECIMAL(15, 2) NOT NULL,
    counterparty_id INT,
    purpose TEXT,
    created_by INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (document_id) REFERENCES documents(document_id) ON DELETE SET NULL,
    FOREIGN KEY (counterparty_id) REFERENCES counterparties(counterparty_id) ON DELETE SET NULL,
    FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_operation_date (operation_date)
);

-- Вставка начальных данных
-- Пользователи (пароли: admin123 и user123)
INSERT INTO users (username, password_hash, role, full_name) VALUES
('admin', 'admin123', 'admin', 'Администратор системы'),
('user', 'user123', 'user', 'Пользователь системы');

-- Основные счета бухгалтерского учета
INSERT INTO accounts (account_number, account_name, account_type, is_synthetic) VALUES
('01', 'Основные средства', 'active', TRUE),
('10', 'Материалы', 'active', TRUE),
('20', 'Основное производство', 'active', TRUE),
('41', 'Товары', 'active', TRUE),
('50', 'Касса', 'active', TRUE),
('51', 'Расчетные счета', 'active', TRUE),
('60', 'Расчеты с поставщиками и подрядчиками', 'active-passive', TRUE),
('62', 'Расчеты с покупателями и заказчиками', 'active-passive', TRUE),
('68', 'Расчеты по налогам и сборам', 'passive', TRUE),
('69', 'Расчеты по социальному страхованию', 'passive', TRUE),
('70', 'Расчеты с персоналом по оплате труда', 'passive', TRUE),
('71', 'Расчеты с подотчетными лицами', 'active-passive', TRUE),
('80', 'Уставный капитал', 'passive', TRUE),
('90', 'Продажи', 'active-passive', TRUE),
('91', 'Прочие доходы и расходы', 'active-passive', TRUE),
('99', 'Прибыли и убытки', 'active-passive', TRUE);

-- Пример организации
INSERT INTO organizations (org_name, inn, kpp, address, phone, director) VALUES
('ООО "Пример"', '7701234567', '770101001', 'г. Москва, ул. Примерная, д. 1', '+7 (495) 123-45-67', 'Иванов И.И.');

-- Примеры контрагентов
INSERT INTO counterparties (name, inn, kpp, address, phone, counterparty_type) VALUES
('ООО "Поставщик 1"', '7702345678', '770201001', 'г. Москва, ул. Поставщиков, д. 10', '+7 (495) 234-56-78', 'supplier'),
('ООО "Покупатель 1"', '7703456789', '770301001', 'г. Москва, ул. Покупателей, д. 20', '+7 (495) 345-67-89', 'customer');

-- Примеры товаров
INSERT INTO products (product_code, product_name, unit, price, vat_rate, product_type) VALUES
('TOV001', 'Товар 1', 'шт', 1000.00, 20.00, 'goods'),
('TOV002', 'Товар 2', 'шт', 2000.00, 20.00, 'goods'),
('USL001', 'Услуга 1', 'час', 500.00, 20.00, 'service');
