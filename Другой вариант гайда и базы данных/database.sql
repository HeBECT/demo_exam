-- ============================================
-- СОЗДАНИЕ БАЗЫ ДАННЫХ DemEkz
-- Запускать в MySQL Workbench или HeidiSQL
-- ============================================

CREATE DATABASE IF NOT EXISTS demekz CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE demekz;

-- 1. Таблица: Заказчики
CREATE TABLE IF NOT EXISTS zakazchiki (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    kod      VARCHAR(20),
    name     VARCHAR(200),
    inn      VARCHAR(20),
    adres    VARCHAR(300),
    phone    VARCHAR(30),
    salesman TINYINT(1) DEFAULT 0,
    buyer    TINYINT(1) DEFAULT 0
);

-- 2. Таблица: Заказ покупателя
CREATE TABLE IF NOT EXISTS zakaz_pokupatelya (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    nomer       INT,
    data        DATE,
    ispolnitel  VARCHAR(200),
    zakazchik   VARCHAR(200),
    produkciya  VARCHAR(200),
    kolichestvo DECIMAL(10,2),
    edinica     VARCHAR(20),
    cena        DECIMAL(10,2),
    summa       DECIMAL(10,2)
);

-- 3. Таблица: Производство
CREATE TABLE IF NOT EXISTS proizvodstvo (
    id                    INT AUTO_INCREMENT PRIMARY KEY,
    nomer                 INT,
    data                  DATE,
    nazvanie_produkcii    VARCHAR(200),
    kod_produkcii         VARCHAR(50),
    kolichestvo_produkcii DECIMAL(10,2),
    edinica_produkcii     VARCHAR(20),
    nazvanie_materiala    VARCHAR(200),
    kod_materiala         VARCHAR(50),
    kolichestvo_materiala DECIMAL(10,2),
    edinica_materiala     VARCHAR(20)
);

-- 4. Таблица: Спецификация
CREATE TABLE IF NOT EXISTS specifikaciya (
    id                   INT AUTO_INCREMENT PRIMARY KEY,
    nazvanie             VARCHAR(200),
    produkt              VARCHAR(200),
    kolichestvo          DECIMAL(10,2),
    izgotovitel          VARCHAR(200),
    material             VARCHAR(200),
    edinica              VARCHAR(20),
    kolichestvo_materiala DECIMAL(10,2)
);

-- 5. Таблица: Цены
CREATE TABLE IF NOT EXISTS ceny (
    id      INT AUTO_INCREMENT PRIMARY KEY,
    nazvanie VARCHAR(200),
    cena     DECIMAL(10,2)
);

-- 6. Таблица: Расчет стоимости продукции
CREATE TABLE IF NOT EXISTS raschet_stoimosti (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    tip      VARCHAR(200),
    stoimost DECIMAL(10,2)
);

-- ============================================
-- ТЕСТОВЫЕ ДАННЫЕ
-- ============================================

INSERT INTO zakazchiki (kod, name, inn, adres, phone, salesman, buyer) VALUES
('000000001', 'ООО "Поставка"',          '',             'г.Пятигорск',                    '+79198634592', 1, 1),
('000000002', 'ООО "Кинотеатр Квант"',   '26320045123',  'г. Железноводск, ул. Мира, 123', '+79884581555', 1, 0),
('000000003', 'ООО "Ромашка"',           '4140784214',   'г. Омск, ул. Строителей, 294',   '+79882584546', 0, 1),
('000000008', 'ООО "Новый JDTO"',        '26320045111',  'г. Железноводск',                '+79884581555', 1, 0),
('000000009', 'ООО "Ипподром"',          '5874045632',   'г. Уфа, ул. Набережная, 37',     '+79627486389', 1, 1),
('000000010', 'ООО "Ассоль"',            '2629011278',   'г. Калуга, ул. Пушкина, 94',     '+79184572398', 0, 1);

INSERT INTO zakaz_pokupatelya (nomer, data, ispolnitel, zakazchik, produkciya, kolichestvo, edinica, cena, summa) VALUES
(2, '2025-06-06', 'ООО Молочный комбинат "Полесье"', 'ООО "Ассоль"', 'Кефир 2,5% 900г.',  12, 'шт', 80, 960),
(2, '2025-06-06', 'ООО Молочный комбинат "Полесье"', 'ООО "Ассоль"', 'Кефир 3,2% 900г.',   9, 'шт', 82, 738),
(2, '2025-06-06', 'ООО Молочный комбинат "Полесье"', 'ООО "Ассоль"', 'Молоко 2,5% 900г.', 10, 'шт', 79, 790);

INSERT INTO proizvodstvo (nomer, data, nazvanie_produkcii, kod_produkcii, kolichestvo_produkcii, edinica_produkcii, nazvanie_materiala, kod_materiala, kolichestvo_materiala, edinica_materiala) VALUES
(1, '2025-06-09', 'Сметана классическая 15% 540г.', 'НФ-00000006', 1, 'шт', 'Молоко нормализованное', 'НФ-00000004', 0.9,  'кг'),
(1, '2025-06-09', 'Сметана классическая 15% 540г.', 'НФ-00000006', 1, 'шт', 'Закваска сметанная',      'НФ-00000005', 0.07, 'кг');

INSERT INTO specifikaciya (nazvanie, produkt, kolichestvo, izgotovitel, material, edinica, kolichestvo_materiala) VALUES
('Основная Сметана 15%', 'Сметана классическая 15% 540г.', 1, 'ООО Молочный комбинат "Полесье"', 'Молоко нормализованное', 'кг', 0.9),
('Основная Сметана 15%', 'Сметана классическая 15% 540г.', 1, 'ООО Молочный комбинат "Полесье"', 'Закваска сметанная',      'кг', 0.07);

INSERT INTO ceny (nazvanie, cena) VALUES
('Закваска сметанная',            45),
('Кефир 2,5% 900г.',              80),
('Кефир 3,2% 900г.',              82),
('Молоко 2,5% 900г.',             70),
('Молоко 3,2% 900г.',             76),
('Молоко нормализованное',        34),
('Сметана классическая 15% 540г.', 89),
('Сметана классическая 20% 540г.', 92);

INSERT INTO raschet_stoimosti (tip, stoimost) VALUES
('Материалы',                       36.70),
('Сметана классическая 15% 540г.',  36.70),
('Закваска сметанная',               0.70),
('Молоко нормализованное',          36.00),
('Итого',                           36.70);
