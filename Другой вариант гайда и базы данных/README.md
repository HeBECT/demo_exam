# Демо-экзамен: WPF + MySQL — Полная инструкция

> Эта инструкция написана пошагово для повторения на экзамене с нуля, без каких-либо готовых файлов.

---

## ЧАСТЬ 1: СОЗДАНИЕ БАЗЫ ДАННЫХ В MySQL

### 1.1 Что нужно установить

- **MySQL Server** (скачать с mysql.com → MySQL Community Server)
- **MySQL Workbench** (входит в MySQL Installer) — визуальный редактор

### 1.2 Создание базы данных через MySQL Workbench

1. Открыть **MySQL Workbench**
2. Нажать на соединение (обычно `Local instance MySQL80` или `root@localhost`)
3. Ввести пароль root (тот, что задали при установке)
4. В меню выбрать **File → New Query Tab** (или нажать `Ctrl+T`)
5. В поле запроса написать код ниже и нажать кнопку **молния (Execute)** (`Ctrl+Shift+Enter`)

### 1.3 SQL-скрипт (весь код для создания БД)

Вставить этот код в MySQL Workbench и выполнить:

```sql
CREATE DATABASE IF NOT EXISTS demekz CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE demekz;

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

CREATE TABLE IF NOT EXISTS specifikaciya (
    id                    INT AUTO_INCREMENT PRIMARY KEY,
    nazvanie              VARCHAR(200),
    produkt               VARCHAR(200),
    kolichestvo           DECIMAL(10,2),
    izgotovitel           VARCHAR(200),
    material              VARCHAR(200),
    edinica               VARCHAR(20),
    kolichestvo_materiala DECIMAL(10,2)
);

CREATE TABLE IF NOT EXISTS ceny (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    nazvanie VARCHAR(200),
    cena     DECIMAL(10,2)
);

CREATE TABLE IF NOT EXISTS raschet_stoimosti (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    tip      VARCHAR(200),
    stoimost DECIMAL(10,2)
);
```

После выполнения слева в панели **Schemas** появится база `demekz` с 6 таблицами.

---

## ЧАСТЬ 2: СОЗДАНИЕ WPF ПРОЕКТА В VISUAL STUDIO

### 2.1 Создание нового проекта

1. Открыть **Visual Studio 2022**
2. Нажать **"Создать проект"**
3. В поиске написать `WPF`
4. Выбрать **"Приложение WPF (.NET Framework)"**
   - Если нет .NET Framework — выбрать **"Приложение WPF"** (.NET 6/7/8)
5. Нажать **Далее**
6. **Имя проекта:** `DemEkz`
7. **Расположение:** выбрать удобную папку
8. **.NET Framework:** выбрать `.NET Framework 4.8` (или .NET 8 если Framework нет)
9. Нажать **Создать**

> Visual Studio автоматически создаст: App.xaml, App.xaml.cs, MainWindow.xaml, MainWindow.xaml.cs

### 2.2 Установка пакета MySQL

Нужно установить библиотеку для работы с MySQL:

1. В меню выбрать **Инструменты → Диспетчер пакетов NuGet → Консоль диспетчера пакетов**
2. В появившейся консоли внизу написать и нажать Enter:

```
Install-Package MySql.Data -Version 8.3.0
```

3. Подождать пока установится (в выводе появится `Successfully installed`)

**Или через графический интерфейс:**
1. Правой кнопкой на проект в Обозревателе решений → **Управление пакетами NuGet**
2. Вкладка **Обзор** → в поиске написать `MySql.Data`
3. Выбрать первый результат → нажать **Установить**

---

## ЧАСТЬ 3: СОЗДАНИЕ ФАЙЛОВ ПРОЕКТА

### ВАЖНО: Порядок создания файлов

Создавать файлы в таком порядке:
1. DbHelper.cs (класс подключения к БД)
2. LoginWindow.xaml + LoginWindow.xaml.cs
3. Изменить MainWindow.xaml + MainWindow.xaml.cs
4. 6 окон таблиц (каждое: .xaml + .xaml.cs)
5. Изменить App.xaml (стартовое окно)

---

### 3.1 Создание DbHelper.cs (класс подключения к БД)

1. В **Обозревателе решений** кликнуть **правой кнопкой** на проект (DemEkz)
2. Выбрать **Добавить → Новый элемент**
3. Выбрать **Класс (Class)**
4. Имя файла: `DbHelper.cs`
5. Нажать **Добавить**
6. Удалить весь код в файле и вставить:

```csharp
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public static class DbHelper
    {
        // ЗАМЕНИТЕ Password=root на ваш пароль MySQL!
        public static string ConnectionString =
            "Server=localhost;Database=demekz;User Id=root;Password=root;CharSet=utf8mb4;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
```

> **Важно:** Если ваш пароль MySQL не `root` — замените `Password=root` на ваш пароль.

---

### 3.2 Создание LoginWindow (окно входа)

#### Шаг 1: Создать XAML файл окна

1. Правой кнопкой на проект → **Добавить → Новый элемент**
2. Выбрать **Окно (WPF)** (Window (WPF))
3. Имя: `LoginWindow.xaml`
4. Нажать **Добавить**

#### Шаг 2: Вставить код в LoginWindow.xaml

Открыть файл `LoginWindow.xaml`, удалить всё содержимое и вставить:

```xml
<Window x:Class="DemEkz.LoginWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Вход в систему" Height="220" Width="320"
        WindowStartupLocation="CenterScreen" ResizeMode="NoResize">
    <Grid Margin="15">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="10"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="10"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="10"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="10"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="80"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <Label Grid.Row="0" Grid.Column="0" Content="Логин:" VerticalAlignment="Center"/>
        <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtLogin" Height="25"/>

        <Label Grid.Row="2" Grid.Column="0" Content="Пароль:" VerticalAlignment="Center"/>
        <PasswordBox Grid.Row="2" Grid.Column="1" x:Name="txtPassword" Height="25"/>

        <TextBlock Grid.Row="4" Grid.ColumnSpan="2" x:Name="lblError"
                   Foreground="Red" TextWrapping="Wrap"/>

        <Button Grid.Row="6" Grid.ColumnSpan="2" Content="Войти"
                Click="BtnLogin_Click" Height="30"/>
    </Grid>
</Window>
```

#### Шаг 3: Вставить код в LoginWindow.xaml.cs

Открыть `LoginWindow.xaml.cs` (нажать F7 или раскрыть узел LoginWindow.xaml в Обозревателе), удалить всё и вставить:

```csharp
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
```

---

### 3.3 Изменение MainWindow (главное меню)

Открыть `MainWindow.xaml`, удалить всё и вставить:

```xml
<Window x:Class="DemEkz.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Главное меню" Height="380" Width="300"
        WindowStartupLocation="CenterScreen" ResizeMode="NoResize">
    <Grid Margin="15">
        <StackPanel>
            <Label Content="Выберите таблицу для просмотра / добавления:"
                   FontSize="12" Margin="0,0,0,10" FontWeight="Bold"/>
            <Button Content="Заказчики"        Click="BtnZakazchiki_Click"    Margin="0,5" Height="35"/>
            <Button Content="Заказ покупателя" Click="BtnZakaz_Click"         Margin="0,5" Height="35"/>
            <Button Content="Производство"     Click="BtnProizvodstvo_Click"  Margin="0,5" Height="35"/>
            <Button Content="Спецификация"     Click="BtnSpecifikaciya_Click" Margin="0,5" Height="35"/>
            <Button Content="Цены"             Click="BtnCeny_Click"          Margin="0,5" Height="35"/>
            <Button Content="Расчет стоимости" Click="BtnRaschet_Click"       Margin="0,5" Height="35"/>
        </StackPanel>
    </Grid>
</Window>
```

Открыть `MainWindow.xaml.cs`, удалить всё и вставить:

```csharp
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
```

---

### 3.4 Изменение App.xaml (стартовое окно)

Открыть `App.xaml` и изменить строчку `StartupUri`:
- Найти: `StartupUri="MainWindow.xaml"`
- Заменить на: `StartupUri="LoginWindow.xaml"`

Итоговый App.xaml:

```xml
<Application x:Class="DemEkz.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="LoginWindow.xaml">
    <Application.Resources>
    </Application.Resources>
</Application>
```

---

## ЧАСТЬ 4: СОЗДАНИЕ ОКОН ДЛЯ 6 ТАБЛИЦ

### Шаблон создания каждого окна:

**Для каждой таблицы** нужно:
1. Правой кнопкой на проект → Добавить → Новый элемент → **Окно (WPF)**
2. Задать имя файла
3. Вставить XAML-код
4. Вставить C#-код в .cs файл

---

### 4.1 Окно: Заказчики (ZakazchikiWindow)

**Создать файл:** `ZakazchikiWindow.xaml`

**Код ZakazchikiWindow.xaml:**

```xml
<Window x:Class="DemEkz.ZakazchikiWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Заказчики" Height="620" Width="860"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="120"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="120"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Код:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtKod" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Наименование:"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtName" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="0" Content="ИНН:"/>
                <TextBox Grid.Row="1" Grid.Column="1" x:Name="txtInn" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="2" Content="Адрес:"/>
                <TextBox Grid.Row="1" Grid.Column="3" x:Name="txtAdres" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="0" Content="Телефон:"/>
                <TextBox Grid.Row="2" Grid.Column="1" x:Name="txtPhone" Margin="3" Height="24"/>

                <StackPanel Grid.Row="2" Grid.Column="2" Grid.ColumnSpan="2"
                            Orientation="Horizontal" Margin="3">
                    <CheckBox x:Name="chkSalesman" Content="Поставщик" Margin="0,0,15,0" VerticalAlignment="Center"/>
                    <CheckBox x:Name="chkBuyer"    Content="Покупатель" VerticalAlignment="Center"/>
                </StackPanel>

                <Button Grid.Row="3" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код ZakazchikiWindow.xaml.cs:**

```csharp
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ZakazchikiWindow : Window
    {
        public ZakazchikiWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM zakazchiki", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO zakazchiki (kod, name, inn, adres, phone, salesman, buyer) " +
                    "VALUES (@kod, @name, @inn, @adres, @phone, @salesman, @buyer)", conn);

                cmd.Parameters.AddWithValue("@kod",      txtKod.Text);
                cmd.Parameters.AddWithValue("@name",     txtName.Text);
                cmd.Parameters.AddWithValue("@inn",      txtInn.Text);
                cmd.Parameters.AddWithValue("@adres",    txtAdres.Text);
                cmd.Parameters.AddWithValue("@phone",    txtPhone.Text);
                cmd.Parameters.AddWithValue("@salesman", chkSalesman.IsChecked == true ? 1 : 0);
                cmd.Parameters.AddWithValue("@buyer",    chkBuyer.IsChecked    == true ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

### 4.2 Окно: Заказ покупателя (ZakazWindow)

**Создать файл:** `ZakazWindow.xaml`

**Код ZakazWindow.xaml:**

```xml
<Window x:Class="DemEkz.ZakazWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Заказ покупателя" Height="660" Width="950"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="120"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="120"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Номер:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtNomer" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Дата (гг-мм-дд):"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtData" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="0" Content="Исполнитель:"/>
                <TextBox Grid.Row="1" Grid.Column="1" x:Name="txtIspolnitel" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="2" Content="Заказчик:"/>
                <TextBox Grid.Row="1" Grid.Column="3" x:Name="txtZakazchik" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="0" Content="Продукция:"/>
                <TextBox Grid.Row="2" Grid.Column="1" x:Name="txtProdukciya" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="2" Content="Количество:"/>
                <TextBox Grid.Row="2" Grid.Column="3" x:Name="txtKolichestvo" Margin="3" Height="24"/>

                <Label Grid.Row="3" Grid.Column="0" Content="Ед. изм.:"/>
                <TextBox Grid.Row="3" Grid.Column="1" x:Name="txtEdinica" Margin="3" Height="24"/>

                <Label Grid.Row="3" Grid.Column="2" Content="Цена:"/>
                <TextBox Grid.Row="3" Grid.Column="3" x:Name="txtCena" Margin="3" Height="24"/>

                <Label Grid.Row="4" Grid.Column="0" Content="Сумма:"/>
                <TextBox Grid.Row="4" Grid.Column="1" x:Name="txtSumma" Margin="3" Height="24"/>

                <Button Grid.Row="5" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код ZakazWindow.xaml.cs:**

```csharp
using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ZakazWindow : Window
    {
        public ZakazWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM zakaz_pokupatelya", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO zakaz_pokupatelya " +
                    "(nomer, data, ispolnitel, zakazchik, produkciya, kolichestvo, edinica, cena, summa) " +
                    "VALUES (@nomer, @data, @ispolnitel, @zakazchik, @produkciya, @kolichestvo, @edinica, @cena, @summa)",
                    conn);

                cmd.Parameters.AddWithValue("@nomer",      int.Parse(txtNomer.Text));
                cmd.Parameters.AddWithValue("@data",       DateTime.Parse(txtData.Text));
                cmd.Parameters.AddWithValue("@ispolnitel", txtIspolnitel.Text);
                cmd.Parameters.AddWithValue("@zakazchik",  txtZakazchik.Text);
                cmd.Parameters.AddWithValue("@produkciya", txtProdukciya.Text);
                cmd.Parameters.AddWithValue("@kolichestvo",decimal.Parse(txtKolichestvo.Text));
                cmd.Parameters.AddWithValue("@edinica",    txtEdinica.Text);
                cmd.Parameters.AddWithValue("@cena",       decimal.Parse(txtCena.Text));
                cmd.Parameters.AddWithValue("@summa",      decimal.Parse(txtSumma.Text));
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

### 4.3 Окно: Производство (ProizvodstvoWindow)

**Создать файл:** `ProizvodstvoWindow.xaml`

**Код ProizvodstvoWindow.xaml:**

```xml
<Window x:Class="DemEkz.ProizvodstvoWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Производство" Height="700" Width="1000"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="160"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="160"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Номер:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtNomer" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Дата (гг-мм-дд):"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtData" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="0" Content="Наим. продукции:"/>
                <TextBox Grid.Row="1" Grid.Column="1" x:Name="txtNazvanieProdukcii" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="2" Content="Код продукции:"/>
                <TextBox Grid.Row="1" Grid.Column="3" x:Name="txtKodProdukcii" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="0" Content="Кол-во продукции:"/>
                <TextBox Grid.Row="2" Grid.Column="1" x:Name="txtKolProdukcii" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="2" Content="Ед. изм. продукции:"/>
                <TextBox Grid.Row="2" Grid.Column="3" x:Name="txtEdProdukcii" Margin="3" Height="24"/>

                <Label Grid.Row="3" Grid.Column="0" Content="Наим. материала:"/>
                <TextBox Grid.Row="3" Grid.Column="1" x:Name="txtNazvanieMateriala" Margin="3" Height="24"/>

                <Label Grid.Row="3" Grid.Column="2" Content="Код материала:"/>
                <TextBox Grid.Row="3" Grid.Column="3" x:Name="txtKodMateriala" Margin="3" Height="24"/>

                <Label Grid.Row="4" Grid.Column="0" Content="Кол-во материала:"/>
                <TextBox Grid.Row="4" Grid.Column="1" x:Name="txtKolMateriala" Margin="3" Height="24"/>

                <Label Grid.Row="4" Grid.Column="2" Content="Ед. изм. материала:"/>
                <TextBox Grid.Row="4" Grid.Column="3" x:Name="txtEdMateriala" Margin="3" Height="24"/>

                <Button Grid.Row="5" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код ProizvodstvoWindow.xaml.cs:**

```csharp
using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class ProizvodstvoWindow : Window
    {
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO proizvodstvo " +
                    "(nomer, data, nazvanie_produkcii, kod_produkcii, kolichestvo_produkcii, edinica_produkcii, " +
                    " nazvanie_materiala, kod_materiala, kolichestvo_materiala, edinica_materiala) " +
                    "VALUES (@nomer, @data, @np, @kp, @kolp, @edp, @nm, @km, @kolm, @edm)",
                    conn);

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
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

### 4.4 Окно: Спецификация (SpecifikaciyaWindow)

**Создать файл:** `SpecifikaciyaWindow.xaml`

**Код SpecifikaciyaWindow.xaml:**

```xml
<Window x:Class="DemEkz.SpecifikaciyaWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Спецификация" Height="650" Width="900"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="140"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="140"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Название:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtNazvanie" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Продукт:"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtProdukt" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="0" Content="Количество:"/>
                <TextBox Grid.Row="1" Grid.Column="1" x:Name="txtKolichestvo" Margin="3" Height="24"/>

                <Label Grid.Row="1" Grid.Column="2" Content="Изготовитель:"/>
                <TextBox Grid.Row="1" Grid.Column="3" x:Name="txtIzgotovitel" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="0" Content="Материал:"/>
                <TextBox Grid.Row="2" Grid.Column="1" x:Name="txtMaterial" Margin="3" Height="24"/>

                <Label Grid.Row="2" Grid.Column="2" Content="Ед. изм.:"/>
                <TextBox Grid.Row="2" Grid.Column="3" x:Name="txtEdinica" Margin="3" Height="24"/>

                <Label Grid.Row="3" Grid.Column="0" Content="Кол-во материала:"/>
                <TextBox Grid.Row="3" Grid.Column="1" x:Name="txtKolMateriala" Margin="3" Height="24"/>

                <Button Grid.Row="4" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код SpecifikaciyaWindow.xaml.cs:**

```csharp
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class SpecifikaciyaWindow : Window
    {
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO specifikaciya " +
                    "(nazvanie, produkt, kolichestvo, izgotovitel, material, edinica, kolichestvo_materiala) " +
                    "VALUES (@nazvanie, @produkt, @kol, @izgotovitel, @material, @edinica, @kolmat)",
                    conn);

                cmd.Parameters.AddWithValue("@nazvanie",    txtNazvanie.Text);
                cmd.Parameters.AddWithValue("@produkt",     txtProdukt.Text);
                cmd.Parameters.AddWithValue("@kol",         decimal.Parse(txtKolichestvo.Text));
                cmd.Parameters.AddWithValue("@izgotovitel", txtIzgotovitel.Text);
                cmd.Parameters.AddWithValue("@material",    txtMaterial.Text);
                cmd.Parameters.AddWithValue("@edinica",     txtEdinica.Text);
                cmd.Parameters.AddWithValue("@kolmat",      decimal.Parse(txtKolMateriala.Text));
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

### 4.5 Окно: Цены (CenyWindow)

**Создать файл:** `CenyWindow.xaml`

**Код CenyWindow.xaml:**

```xml
<Window x:Class="DemEkz.CenyWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Цены" Height="500" Width="600"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="140"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="140"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Наименование:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtNazvanie" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Цена:"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtCena" Margin="3" Height="24"/>

                <Button Grid.Row="1" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код CenyWindow.xaml.cs:**

```csharp
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class CenyWindow : Window
    {
        public CenyWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM ceny", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO ceny (nazvanie, cena) VALUES (@nazvanie, @cena)", conn);

                cmd.Parameters.AddWithValue("@nazvanie", txtNazvanie.Text);
                cmd.Parameters.AddWithValue("@cena",     decimal.Parse(txtCena.Text));
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

### 4.6 Окно: Расчет стоимости (RaschetWindow)

**Создать файл:** `RaschetWindow.xaml`

**Код RaschetWindow.xaml:**

```xml
<Window x:Class="DemEkz.RaschetWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Расчет стоимости продукции" Height="500" Width="600"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <DataGrid Grid.Row="0" x:Name="dgData"
                  AutoGenerateColumns="True" IsReadOnly="True"
                  Margin="0,0,0,10"/>

        <GroupBox Grid.Row="1" Header="Добавить запись" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="100"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="100"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" Content="Тип:"/>
                <TextBox Grid.Row="0" Grid.Column="1" x:Name="txtTip" Margin="3" Height="24"/>

                <Label Grid.Row="0" Grid.Column="2" Content="Стоимость:"/>
                <TextBox Grid.Row="0" Grid.Column="3" x:Name="txtStoimost" Margin="3" Height="24"/>

                <Button Grid.Row="1" Grid.ColumnSpan="4"
                        Content="Добавить" Click="BtnAdd_Click"
                        Height="30" Margin="3"/>
            </Grid>
        </GroupBox>
    </Grid>
</Window>
```

**Код RaschetWindow.xaml.cs:**

```csharp
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace DemEkz
{
    public partial class RaschetWindow : Window
    {
        public RaschetWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM raschet_stoimosti", conn);
                var table   = new DataTable();
                adapter.Fill(table);
                dgData.ItemsSource = table.DefaultView;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO raschet_stoimosti (tip, stoimost) VALUES (@tip, @stoimost)", conn);

                cmd.Parameters.AddWithValue("@tip",      txtTip.Text);
                cmd.Parameters.AddWithValue("@stoimost", decimal.Parse(txtStoimost.Text));
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Запись добавлена!");
            LoadData();
        }
    }
}
```

---

## ЧАСТЬ 5: ЗАПУСК И ПРОВЕРКА

### 5.1 Запустить проект

Нажать **F5** или кнопку **► Запустить** в Visual Studio.

Должно открыться окно входа.

### 5.2 Проверить вход

| Логин | Пароль   |
|-------|----------|
| admin | admin123 |
| user  | user123  |

После успешного входа откроется главное меню с 6 кнопками.

### 5.3 Возможные ошибки и решения

| Ошибка | Причина | Решение |
|--------|---------|---------|
| `Unable to connect to any of the specified MySQL hosts` | MySQL не запущен или неверный пароль | Проверить, что MySQL Server запущен. Изменить пароль в DbHelper.cs |
| `Unknown database 'demekz'` | База данных не создана | Выполнить SQL-скрипт из Части 1 |
| `Could not load file or assembly 'MySql.Data'` | NuGet пакет не установлен | Установить пакет MySql.Data через NuGet |
| Кириллица отображается как `???` | Неверная кодировка | В строке подключения добавить `CharSet=utf8mb4;` |

---

## ЧАСТЬ 6: СТРУКТУРА ФАЙЛОВ ПРОЕКТА

После создания всех файлов, в Обозревателе решений должна быть такая структура:

```
DemEkz/
├── App.xaml                    ← стартовое окно = LoginWindow
├── App.xaml.cs
├── DbHelper.cs                 ← строка подключения к MySQL
├── LoginWindow.xaml            ← форма входа
├── LoginWindow.xaml.cs
├── MainWindow.xaml             ← главное меню (6 кнопок)
├── MainWindow.xaml.cs
├── ZakazchikiWindow.xaml       ← таблица Заказчики
├── ZakazchikiWindow.xaml.cs
├── ZakazWindow.xaml            ← таблица Заказ покупателя
├── ZakazWindow.xaml.cs
├── ProizvodstvoWindow.xaml     ← таблица Производство
├── ProizvodstvoWindow.xaml.cs
├── SpecifikaciyaWindow.xaml    ← таблица Спецификация
├── SpecifikaciyaWindow.xaml.cs
├── CenyWindow.xaml             ← таблица Цены
├── CenyWindow.xaml.cs
├── RaschetWindow.xaml          ← таблица Расчет стоимости
└── RaschetWindow.xaml.cs
```

---

## ЧАСТЬ 7: ШПАРГАЛКА — КАК РАБОТАЕТ КОД

### Паттерн каждого окна таблицы (одинаковый для всех 6):

```
Конструктор → InitializeComponent() → LoadData()

LoadData():
  1. Открыть соединение с БД
  2. Выполнить SELECT * FROM таблица
  3. Загрузить результат в DataTable
  4. Показать в DataGrid через .ItemsSource

BtnAdd_Click():
  1. Открыть соединение с БД
  2. Создать INSERT команду с параметрами
  3. Добавить значения из TextBox через .AddWithValue()
  4. Выполнить .ExecuteNonQuery()
  5. Показать MessageBox("Запись добавлена!")
  6. Перезагрузить данные через LoadData()
```

### Типы данных в C#:

| В SQL | В C# | Парсинг из TextBox |
|-------|------|--------------------|
| INT | int | `int.Parse(txtX.Text)` |
| DECIMAL | decimal | `decimal.Parse(txtX.Text)` |
| VARCHAR | string | `txtX.Text` (без парсинга) |
| DATE | DateTime | `DateTime.Parse(txtX.Text)` |
| TINYINT(1) | int (0 или 1) | `chkX.IsChecked == true ? 1 : 0` |

### Формат даты при вводе:
- Вводить в формате: `2025-06-09` (год-месяц-день)

---

## ЧАСТЬ 8: БЫСТРЫЙ ЧЕКЛИСТ ДЛЯ ЭКЗАМЕНА

- [ ] 1. Создать БД в MySQL Workbench (вставить SQL, нажать молнию)
- [ ] 2. Создать WPF проект `DemEkz`
- [ ] 3. Установить NuGet: `MySql.Data`
- [ ] 4. Создать `DbHelper.cs` → вставить код с паролем
- [ ] 5. Создать `LoginWindow.xaml` → вставить XAML → вставить C#
- [ ] 6. Изменить `App.xaml` → StartupUri="LoginWindow.xaml"
- [ ] 7. Изменить `MainWindow.xaml` → вставить XAML с 6 кнопками → вставить C#
- [ ] 8. Создать `ZakazchikiWindow.xaml` + `.cs`
- [ ] 9. Создать `ZakazWindow.xaml` + `.cs`
- [ ] 10. Создать `ProizvodstvoWindow.xaml` + `.cs`
- [ ] 11. Создать `SpecifikaciyaWindow.xaml` + `.cs`
- [ ] 12. Создать `CenyWindow.xaml` + `.cs`
- [ ] 13. Создать `RaschetWindow.xaml` + `.cs`
- [ ] 14. Нажать F5 → войти → проверить каждую таблицу
