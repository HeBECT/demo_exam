# Как создавать окна через дизайнер Visual Studio (без копирования XAML)

> Это руководство для тех, кто делает всё руками через интерфейс Visual Studio.
> Здесь объяснено: что перетаскивать, куда кликать, что писать в свойствах.

---

## ВАЖНО: ДВА ОБЯЗАТЕЛЬНЫХ ИСПРАВЛЕНИЯ

### Исправление 1 — Русские символы не отображаются

**Симптом:** В таблице вместо русских букв показывается `???` или кракозябры.

**Причина:** Неверная кодировка в строке подключения MySQL.

**Решение:** Открыть `DbHelper.cs`, найти строку `ConnectionString` и убедиться что она выглядит так:

```csharp
public static string ConnectionString =
    "Server=localhost;Database=demekz;User Id=root;Password=ВАШИЙ_ПАРОЛЬ;charset=utf8;SslMode=None;AllowPublicKeyRetrieval=True;";
```

> **Ключевые параметры:** `charset=utf8` (не utf8mb4!), `SslMode=None`, `AllowPublicKeyRetrieval=True`

---

### Исправление 2 — Редактирование записей

**Как работает редактирование:**
1. Пользователь **кликает на строку** в DataGrid
2. Данные **автоматически заполняются** в форму внизу
3. Кнопка меняется с **"Добавить"** на **"Сохранить"**
4. Пользователь меняет нужные поля и нажимает **"Сохранить"**
5. Кнопка **"Новая запись"** сбрасывает форму обратно в режим добавления

**Что нужно добавить в XAML каждого окна таблицы:**

1. У DataGrid добавить событие выбора строки:
   ```xml
   SelectionChanged="dgData_SelectionChanged"
   ```
2. У кнопки "Добавить" добавить имя:
   ```xml
   x:Name="btnAdd"
   ```
3. Добавить рядом с кнопкой "Добавить" кнопку "Новая запись":
   ```xml
   <Button Content="Новая запись" Click="BtnClear_Click" Height="30" Margin="3" Width="150"/>
   ```

**Что нужно добавить в .cs файл каждого окна:**

В самом начале класса — поле для хранения id редактируемой записи:
```csharp
private int? _editId = null;
// null = режим добавления, число = режим редактирования
```

Метод срабатывает при клике на строку таблицы:
```csharp
private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    if (dgData.SelectedItem is System.Data.DataRowView row)
    {
        _editId       = Convert.ToInt32(row["id"]);
        txtПоле1.Text = row["поле1"].ToString();
        txtПоле2.Text = row["поле2"].ToString();
        // ... заполняем все поля из строки таблицы
        btnAdd.Content = "Сохранить";
    }
}
```

Метод для кнопки "Новая запись" — сбрасывает форму:
```csharp
private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

private void ClearForm()
{
    _editId = null;
    txtПоле1.Text = txtПоле2.Text = ""; // очистить все поля
    btnAdd.Content = "Добавить";
    dgData.SelectedItem = null;
}
```

Метод BtnAdd_Click — теперь делает и INSERT и UPDATE:
```csharp
private void BtnAdd_Click(object sender, RoutedEventArgs e)
{
    using (var conn = DbHelper.GetConnection())
    {
        conn.Open();
        MySqlCommand cmd;
        if (_editId == null)
        {
            // РЕЖИМ ДОБАВЛЕНИЯ — INSERT
            cmd = new MySqlCommand("INSERT INTO таблица (поле1, поле2) VALUES (@p1, @p2)", conn);
        }
        else
        {
            // РЕЖИМ РЕДАКТИРОВАНИЯ — UPDATE
            cmd = new MySqlCommand("UPDATE таблица SET поле1=@p1, поле2=@p2 WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", _editId);
        }
        cmd.Parameters.AddWithValue("@p1", txtПоле1.Text);
        cmd.Parameters.AddWithValue("@p2", txtПоле2.Text);
        cmd.ExecuteNonQuery();
    }
    MessageBox.Show(_editId == null ? "Запись добавлена!" : "Запись обновлена!");
    ClearForm();
    LoadData();
}
```

> **Ключевая идея:** `_editId == null` → INSERT, `_editId != null` → UPDATE. Одна кнопка — два режима.

---

### Таблица: что добавить в dgData_SelectionChanged для каждого окна

#### Заказчики (ZakazchikiWindow):
```csharp
_editId           = Convert.ToInt32(row["id"]);
txtKod.Text       = row["kod"].ToString();
txtName.Text      = row["name"].ToString();
txtInn.Text       = row["inn"].ToString();
txtAdres.Text     = row["adres"].ToString();
txtPhone.Text     = row["phone"].ToString();
chkSalesman.IsChecked = row["salesman"].ToString() == "1";
chkBuyer.IsChecked    = row["buyer"].ToString()    == "1";
btnAdd.Content    = "Сохранить";
```

#### Заказ покупателя (ZakazWindow):
```csharp
_editId             = Convert.ToInt32(row["id"]);
txtNomer.Text       = row["nomer"].ToString();
txtData.Text        = Convert.ToDateTime(row["data"]).ToString("yyyy-MM-dd");
txtIspolnitel.Text  = row["ispolnitel"].ToString();
txtZakazchik.Text   = row["zakazchik"].ToString();
txtProdukciya.Text  = row["produkciya"].ToString();
txtKolichestvo.Text = row["kolichestvo"].ToString();
txtEdinica.Text     = row["edinica"].ToString();
txtCena.Text        = row["cena"].ToString();
txtSumma.Text       = row["summa"].ToString();
btnAdd.Content      = "Сохранить";
```

#### Производство (ProizvodstvoWindow):
```csharp
_editId                   = Convert.ToInt32(row["id"]);
txtNomer.Text             = row["nomer"].ToString();
txtData.Text              = Convert.ToDateTime(row["data"]).ToString("yyyy-MM-dd");
txtNazvanieProdukcii.Text = row["nazvanie_produkcii"].ToString();
txtKodProdukcii.Text      = row["kod_produkcii"].ToString();
txtKolProdukcii.Text      = row["kolichestvo_produkcii"].ToString();
txtEdProdukcii.Text       = row["edinica_produkcii"].ToString();
txtNazvanieMateriala.Text = row["nazvanie_materiala"].ToString();
txtKodMateriala.Text      = row["kod_materiala"].ToString();
txtKolMateriala.Text      = row["kolichestvo_materiala"].ToString();
txtEdMateriala.Text       = row["edinica_materiala"].ToString();
btnAdd.Content            = "Сохранить";
```

#### Спецификация (SpecifikaciyaWindow):
```csharp
_editId              = Convert.ToInt32(row["id"]);
txtNazvanie.Text     = row["nazvanie"].ToString();
txtProdukt.Text      = row["produkt"].ToString();
txtKolichestvo.Text  = row["kolichestvo"].ToString();
txtIzgotovitel.Text  = row["izgotovitel"].ToString();
txtMaterial.Text     = row["material"].ToString();
txtEdinica.Text      = row["edinica"].ToString();
txtKolMateriala.Text = row["kolichestvo_materiala"].ToString();
btnAdd.Content       = "Сохранить";
```

#### Цены (CenyWindow):
```csharp
_editId          = Convert.ToInt32(row["id"]);
txtNazvanie.Text = row["nazvanie"].ToString();
txtCena.Text     = row["cena"].ToString();
btnAdd.Content   = "Сохранить";
```

#### Расчет стоимости (RaschetWindow):
```csharp
_editId          = Convert.ToInt32(row["id"]);
txtTip.Text      = row["tip"].ToString();
txtStoimost.Text = row["stoimost"].ToString();
btnAdd.Content   = "Сохранить";
```

---

---

## ЧАСТЬ 0: ИНТЕРФЕЙС VISUAL STUDIO — ЧТО ГДЕ НАХОДИТСЯ

```
┌─────────────────────────────────────────────────────────────────┐
│  МЕНЮ: Файл  Правка  Вид  Проект  Сборка  Отладка  Инструменты │
├──────────┬──────────────────────────────────────┬───────────────┤
│          │                                      │               │
│ ПАНЕЛЬ   │         ДИЗАЙНЕР ОКНА                │  ОБОЗРЕВАТЕЛЬ │
│ ЭЛЕМЕНТОВ│      (визуальный редактор)           │  РЕШЕНИЯ      │
│(Toolbox) │                                      │               │
│          ├──────────────────────────────────────┤               │
│ Button   │         XAML РЕДАКТОР                │               │
│ Label    │      (код разметки — текст)          ├───────────────┤
│ TextBox  │                                      │  СВОЙСТВА     │
│ ...      │                                      │  (Properties) │
└──────────┴──────────────────────────────────────┴───────────────┘
```

### Как открыть нужные панели:
| Панель | Как открыть |
|--------|-------------|
| Панель элементов (Toolbox) | Меню **Вид → Панель элементов** или `Ctrl+Alt+X` |
| Свойства (Properties) | Меню **Вид → Окно свойств** или `F4` |
| Обозреватель решений | Меню **Вид → Обозреватель решений** или `Ctrl+Alt+L` |
| Конструктор (дизайнер) | Двойной клик на `.xaml` файле в Обозревателе |

### Как переключаться между дизайнером и XAML кодом:
- Внизу дизайнера есть вкладки: **Конструктор** и **Источник** (или **XAML**)
- Можно также перетащить разделитель между ними

---

## ЧАСТЬ 1: ГЛАВНОЕ ПРАВИЛО — КАК НАЗЫВАТЬ ЭЛЕМЕНТЫ

Когда вы перетаскиваете элемент, Visual Studio даёт ему имя автоматически (button1, textBox1 и т.д.)
**Это имя НУЖНО менять** — иначе в коде будет непонятная каша.

### Как изменить имя элемента:
1. Кликнуть на элемент в дизайнере
2. В панели **Свойства** найти поле **"(Name)"** (самое первое, со скобками)
3. Написать правильное имя

### Таблица правильных имён:

| Что это | Имя (Name) |
|---------|-----------|
| TextBox для логина | `txtLogin` |
| PasswordBox для пароля | `txtPassword` |
| Текст ошибки | `lblError` |
| Кнопка Войти | `btnLogin` |
| DataGrid таблицы | `dgData` |
| TextBox для кода | `txtKod` |
| TextBox для имени | `txtName` |
| CheckBox Поставщик | `chkSalesman` |
| CheckBox Покупатель | `chkBuyer` |

> **Правило имён:** txt = TextBox/PasswordBox, btn = Button, lbl = Label/TextBlock, chk = CheckBox, dg = DataGrid

---

## ЧАСТЬ 2: СОЗДАНИЕ ОКНА ВХОДА (LoginWindow)

### Шаг 1: Создать файл окна
1. Правой кнопкой на проект в Обозревателе решений
2. **Добавить → Новый элемент**
3. Выбрать **"Окно (WPF)"**
4. Имя файла: `LoginWindow.xaml`
5. Нажать **Добавить**

### Шаг 2: Настроить размер окна
Кликнуть на белое поле окна (не на элементы, а на само окно).
В панели **Свойства** найти и изменить:

| Свойство | Значение |
|----------|---------|
| Width | `320` |
| Height | `220` |
| Title | `Вход в систему` |
| WindowStartupLocation | `CenterScreen` |
| ResizeMode | `NoResize` |

### Шаг 3: Изменить корневой элемент на StackPanel

Откройте вкладку **XAML** (внизу дизайнера).
Найдите строки с `<Grid>` и `</Grid>` и **замените** их на:

```xml
<StackPanel Margin="15">

</StackPanel>
```

> Это единственное место где нужно трогать XAML в этом окне. StackPanel автоматически расставляет элементы сверху вниз — это гораздо проще Grid.

### Шаг 4: Перетащить Label "Логин"

1. Открыть вкладку **Конструктор** (дизайнер)
2. В Панели элементов найти **Label**
3. Перетащить его внутрь окна
4. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| Content | `Логин:` |

> Name для Label не нужен — мы к нему из кода не обращаемся.

### Шаг 5: Перетащить TextBox для логина

1. В Панели элементов найти **TextBox**
2. Перетащить под Label
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `txtLogin` |
| Height | `25` |

### Шаг 6: Перетащить Label "Пароль"

Так же как шаг 4, но Content = `Пароль:`

### Шаг 7: Перетащить PasswordBox

1. В Панели элементов найти **PasswordBox**
2. Перетащить под Label "Пароль:"
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `txtPassword` |
| Height | `25` |

### Шаг 8: Перетащить TextBlock для ошибки

1. В Панели элементов найти **TextBlock**
2. Перетащить вниз
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `lblError` |
| Foreground | Нажать на поле → выбрать красный цвет (Red) |
| TextWrapping | `Wrap` |

> TextBlock — для вывода текста ошибки. Он не имеет рамки в отличие от Label.

### Шаг 9: Перетащить Button

1. В Панели элементов найти **Button**
2. Перетащить вниз
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| Content | `Войти` |
| Height | `30` |

### Шаг 10: Создать обработчик нажатия кнопки

**Двойной клик** на кнопке "Войти" в дизайнере.
Visual Studio автоматически создаст метод и откроет файл `.xaml.cs`.
Вы увидите:
```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{

}
```

**Переименуйте метод** с `Button_Click` на `BtnLogin_Click`:
1. В XAML найдите `Click="Button_Click"`
2. Измените на `Click="BtnLogin_Click"`
3. В `.cs` файле тоже переименуйте метод

### Шаг 11: Написать код в LoginWindow.xaml.cs

После двойного клика откроется `.xaml.cs`. Найдите метод и замените его содержимое:

```csharp
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
```

### Шаг 12: Изменить стартовое окно в App.xaml

Открыть `App.xaml`, найти строку:
```
StartupUri="MainWindow.xaml"
```
Изменить на:
```
StartupUri="LoginWindow.xaml"
```

---

## ЧАСТЬ 3: СОЗДАНИЕ ГЛАВНОГО МЕНЮ (MainWindow)

Это уже существующий файл — просто редактируем его.

### Шаг 1: Изменить Grid на StackPanel

В XAML файле MainWindow заменить `<Grid>` на `<StackPanel Margin="15">` и `</Grid>` на `</StackPanel>`.

### Шаг 2: Настроить само окно

Кликнуть на окно → в Свойствах:

| Свойство | Значение |
|----------|---------|
| Title | `Главное меню` |
| Width | `300` |
| Height | `380` |
| WindowStartupLocation | `CenterScreen` |
| ResizeMode | `NoResize` |

### Шаг 3: Добавить Label-заголовок

Перетащить **Label** из Toolbox:

| Свойство | Значение |
|----------|---------|
| Content | `Выберите таблицу:` |
| FontSize | `12` |
| FontWeight | `Bold` |

### Шаг 4: Добавить 6 кнопок

Для каждой кнопки перетащить **Button** и задать:

| Кнопка | Content | Height | Margin |
|--------|---------|--------|--------|
| Кнопка 1 | `Заказчики` | `35` | `0,5` |
| Кнопка 2 | `Заказ покупателя` | `35` | `0,5` |
| Кнопка 3 | `Производство` | `35` | `0,5` |
| Кнопка 4 | `Спецификация` | `35` | `0,5` |
| Кнопка 5 | `Цены` | `35` | `0,5` |
| Кнопка 6 | `Расчет стоимости` | `35` | `0,5` |

> Margin `0,5` означает: 0 по горизонтали, 5 по вертикали (отступ сверху и снизу 5).
> Вводить прямо в поле Margin в свойствах как: `0,5,0,5`

### Шаг 5: Создать обработчики для каждой кнопки

Двойной клик на каждой кнопке → Visual Studio создаст метод.

В XAML переименуйте `Click="Button_Click"` для каждой кнопки:

| Кнопка | Click= |
|--------|--------|
| Заказчики | `BtnZakazchiki_Click` |
| Заказ покупателя | `BtnZakaz_Click` |
| Производство | `BtnProizvodstvo_Click` |
| Спецификация | `BtnSpecifikaciya_Click` |
| Цены | `BtnCeny_Click` |
| Расчет стоимости | `BtnRaschet_Click` |

### Шаг 6: Написать код в MainWindow.xaml.cs

```csharp
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
```

---

## ЧАСТЬ 4: СОЗДАНИЕ ОКОН ТАБЛИЦ (шаблон — один для всех 6)

Все 6 окон таблиц делаются одинаково. Объяснено на примере **Заказчиков**.
Потом по аналогии сделаете остальные 5.

### Структура каждого окна таблицы:
```
Окно
├── (верхняя половина) DataGrid — таблица с данными
└── (нижняя половина) GroupBox "Добавить запись"
    └── Форма с полями TextBox и кнопка
```

### Шаг 1: Создать файл окна

Правой кнопкой на проект → Добавить → Новый элемент → **Окно (WPF)**
Имя: `ZakazchikiWindow.xaml`

### Шаг 2: Настроить окно

| Свойство | Значение |
|----------|---------|
| Title | `Заказчики` |
| Width | `860` |
| Height | `620` |
| WindowStartupLocation | `CenterScreen` |

### Шаг 3: Настроить Grid для разделения на 2 части

Открыть вкладку **XAML** (текстовый редактор).
Найдите `<Grid>` и сразу после него добавьте эти строки:

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
```

> `Height="*"` — занимает всё оставшееся место (DataGrid растянется)
> `Height="Auto"` — занимает ровно столько, сколько нужно (форма внизу)

### Шаг 4: Добавить DataGrid

1. В Панели элементов найти **DataGrid**
2. Перетащить в верхнюю часть дизайнера
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `dgData` |
| AutoGenerateColumns | `True` |
| IsReadOnly | `True` |
| Margin | `0,0,0,10` |

4. В XAML у тега DataGrid должно быть `Grid.Row="0"`:
   Если нет — добавить руками в XAML: найти `<DataGrid` и добавить `Grid.Row="0"` внутрь тега.

### Шаг 5: Добавить GroupBox

1. В Панели элементов найти **GroupBox**
2. Перетащить в нижнюю часть дизайнера
3. В панели **Свойства** установить:

| Свойство | Значение |
|----------|---------|
| Header | `Добавить запись` |
| Padding | `8` |

4. В XAML добавить `Grid.Row="1"` к тегу GroupBox

### Шаг 6: Добавить StackPanel внутрь GroupBox

В XAML найдите тег GroupBox и между `<GroupBox ...>` и `</GroupBox>` добавьте:

```xml
<StackPanel>

</StackPanel>
```

> StackPanel внутри GroupBox будет складывать поля формы вертикально.

### Шаг 7: Добавить поля формы внутрь StackPanel

**Для каждого поля** перетащить в дизайнер (внутрь GroupBox) пару:
- **Label** — подпись поля
- **TextBox** — поле ввода

#### Таблица полей для Заказчиков:

| Label (Content) | TextBox (Name) | Тип элемента |
|-----------------|----------------|--------------|
| `Код:` | `txtKod` | TextBox |
| `Наименование:` | `txtName` | TextBox |
| `ИНН:` | `txtInn` | TextBox |
| `Адрес:` | `txtAdres` | TextBox |
| `Телефон:` | `txtPhone` | TextBox |

Для каждого TextBox в свойствах:

| Свойство | Значение |
|----------|---------|
| Height | `24` |
| Margin | `3` |

### Шаг 8: Добавить CheckBox-ы

1. В Панели элементов найти **CheckBox**
2. Перетащить в форму (2 штуки)
3. Для первого:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `chkSalesman` |
| Content | `Поставщик` |

4. Для второго:

| Свойство | Значение |
|----------|---------|
| **(Name)** | `chkBuyer` |
| Content | `Покупатель` |

### Шаг 9: Добавить кнопку "Добавить"

1. Перетащить **Button** в самый низ формы
2. Установить свойства:

| Свойство | Значение |
|----------|---------|
| Content | `Добавить` |
| Height | `30` |
| Margin | `3` |

3. **Двойной клик** на кнопке → создастся метод
4. В XAML переименовать `Click="Button_Click"` на `Click="BtnAdd_Click"`

### Шаг 10: Написать код в ZakazchikiWindow.xaml.cs

Найти класс `ZakazchikiWindow` и заменить всё содержимое:

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

## ЧАСТЬ 5: ПОЛЯ ДЛЯ ОСТАЛЬНЫХ 5 ОКОН

Структура та же — меняются только поля формы и SQL код.

### Окно: Заказ покупателя (ZakazWindow)

**Имя файла:** `ZakazWindow.xaml`  **Title:** `Заказ покупателя`  **Width:** `950`

| Label (Content) | TextBox (Name) |
|-----------------|----------------|
| `Номер:` | `txtNomer` |
| `Дата (гг-мм-дд):` | `txtData` |
| `Исполнитель:` | `txtIspolnitel` |
| `Заказчик:` | `txtZakazchik` |
| `Продукция:` | `txtProdukciya` |
| `Количество:` | `txtKolichestvo` |
| `Ед. изм.:` | `txtEdinica` |
| `Цена:` | `txtCena` |
| `Сумма:` | `txtSumma` |

**Код метода BtnAdd_Click:**
```csharp
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

        cmd.Parameters.AddWithValue("@nomer",       int.Parse(txtNomer.Text));
        cmd.Parameters.AddWithValue("@data",        System.DateTime.Parse(txtData.Text));
        cmd.Parameters.AddWithValue("@ispolnitel",  txtIspolnitel.Text);
        cmd.Parameters.AddWithValue("@zakazchik",   txtZakazchik.Text);
        cmd.Parameters.AddWithValue("@produkciya",  txtProdukciya.Text);
        cmd.Parameters.AddWithValue("@kolichestvo", decimal.Parse(txtKolichestvo.Text));
        cmd.Parameters.AddWithValue("@edinica",     txtEdinica.Text);
        cmd.Parameters.AddWithValue("@cena",        decimal.Parse(txtCena.Text));
        cmd.Parameters.AddWithValue("@summa",       decimal.Parse(txtSumma.Text));
        cmd.ExecuteNonQuery();
    }
    MessageBox.Show("Запись добавлена!");
    LoadData();
}
```

---

### Окно: Производство (ProizvodstvoWindow)

**Имя файла:** `ProizvodstvoWindow.xaml`  **Title:** `Производство`  **Width:** `1000`

| Label (Content) | TextBox (Name) |
|-----------------|----------------|
| `Номер:` | `txtNomer` |
| `Дата (гг-мм-дд):` | `txtData` |
| `Наим. продукции:` | `txtNazvanieProdukcii` |
| `Код продукции:` | `txtKodProdukcii` |
| `Кол-во продукции:` | `txtKolProdukcii` |
| `Ед. изм. продукции:` | `txtEdProdukcii` |
| `Наим. материала:` | `txtNazvanieMateriala` |
| `Код материала:` | `txtKodMateriala` |
| `Кол-во материала:` | `txtKolMateriala` |
| `Ед. изм. материала:` | `txtEdMateriala` |

**Код метода BtnAdd_Click:**
```csharp
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
        cmd.Parameters.AddWithValue("@data",  System.DateTime.Parse(txtData.Text));
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
```

---

### Окно: Спецификация (SpecifikaciyaWindow)

**Имя файла:** `SpecifikaciyaWindow.xaml`  **Title:** `Спецификация`  **Width:** `900`

| Label (Content) | TextBox (Name) |
|-----------------|----------------|
| `Название:` | `txtNazvanie` |
| `Продукт:` | `txtProdukt` |
| `Количество:` | `txtKolichestvo` |
| `Изготовитель:` | `txtIzgotovitel` |
| `Материал:` | `txtMaterial` |
| `Ед. изм.:` | `txtEdinica` |
| `Кол-во материала:` | `txtKolMateriala` |

**Код метода BtnAdd_Click:**
```csharp
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
```

---

### Окно: Цены (CenyWindow)

**Имя файла:** `CenyWindow.xaml`  **Title:** `Цены`  **Width:** `600`

| Label (Content) | TextBox (Name) |
|-----------------|----------------|
| `Наименование:` | `txtNazvanie` |
| `Цена:` | `txtCena` |

**Код метода BtnAdd_Click:**
```csharp
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
```

---

### Окно: Расчет стоимости (RaschetWindow)

**Имя файла:** `RaschetWindow.xaml`  **Title:** `Расчет стоимости продукции`  **Width:** `600`

| Label (Content) | TextBox (Name) |
|-----------------|----------------|
| `Тип:` | `txtTip` |
| `Стоимость:` | `txtStoimost` |

**Код метода BtnAdd_Click:**
```csharp
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
```

---

## ЧАСТЬ 6: ШАБЛОН КОДА — КОПИРОВАТЬ ДЛЯ КАЖДОГО ОКНА

Для каждого из 6 окон таблиц, в самом начале `.cs` файла (после using System.Windows;) добавьте:

```csharp
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
```

Конструктор и LoadData — **одинаковы для всех**, меняется только имя таблицы:

```csharp
public ИМЯWindow()
{
    InitializeComponent();
    LoadData();     // <-- сразу загружаем данные при открытии
}

private void LoadData()
{
    using (var conn = DbHelper.GetConnection())
    {
        conn.Open();
        var adapter = new MySqlDataAdapter("SELECT * FROM ИМЯ_ТАБЛИЦЫ", conn);
        var table   = new DataTable();
        adapter.Fill(table);
        dgData.ItemsSource = table.DefaultView;   // показать в DataGrid
    }
}
```

| Окно | ИМЯ_ТАБЛИЦЫ |
|------|-------------|
| ZakazchikiWindow | `zakazchiki` |
| ZakazWindow | `zakaz_pokupatelya` |
| ProizvodstvoWindow | `proizvodstvo` |
| SpecifikaciyaWindow | `specifikaciya` |
| CenyWindow | `ceny` |
| RaschetWindow | `raschet_stoimosti` |

---

## ЧАСТЬ 7: МИНИМАЛЬНЫЙ XAML (то что всё равно нужно дописать руками)

Есть 3 вещи, которые проще написать руками в XAML, чем тыкать в дизайнере:

### 1. Замена Grid на StackPanel (для LoginWindow и MainWindow):
```xml
<!-- Было: -->
<Grid>
<!-- Стало: -->
<StackPanel Margin="15">
```

### 2. Разделение окна на 2 строки (для окон таблиц):
Добавить сразу после `<Grid>`:
```xml
<Grid.RowDefinitions>
    <RowDefinition Height="*"/>
    <RowDefinition Height="Auto"/>
</Grid.RowDefinitions>
```

### 3. Привязка элементов к строкам Grid:
У DataGrid добавить: `Grid.Row="0"`
У GroupBox добавить: `Grid.Row="1"`

Это пишется прямо в теге элемента, например:
```xml
<DataGrid Grid.Row="0" x:Name="dgData" .../>
<GroupBox Grid.Row="1" Header="Добавить запись" ...>
```

---

## ЧАСТЬ 8: БЫСТРАЯ ШПАРГАЛКА ПО СВОЙСТВАМ

### Как найти свойство в панели Свойства:
- Свойства можно отсортировать по категориям (кнопка "по категориям") или по алфавиту (кнопка "А→Я")
- Самое важное: **(Name)** — всегда в самом верху списка

### Ключевые свойства:

| Свойство | Где | Что задаёт |
|----------|-----|-----------|
| **(Name)** | Любой элемент | Имя для обращения из кода (`txtLogin`, `dgData`) |
| **Content** | Button, Label | Текст на элементе |
| **Text** | TextBox, TextBlock | Текст внутри |
| **Header** | GroupBox | Заголовок рамки |
| **Height** | Любой | Высота в пикселях |
| **Width** | Любой | Ширина в пикселях |
| **Margin** | Любой | Отступ снаружи (лево, верх, право, низ) |
| **Padding** | GroupBox, Button | Отступ внутри |
| **FontSize** | Label, TextBlock | Размер шрифта |
| **FontWeight** | Label, TextBlock | Жирность (Bold = жирный) |
| **Foreground** | TextBlock | Цвет текста (выбрать Red для ошибки) |
| **IsReadOnly** | DataGrid | Запрет редактирования |
| **AutoGenerateColumns** | DataGrid | Авто-создание колонок |

### Как ввести Margin:
В поле Margin пишется через запятую: `лево, верх, право, низ`
- `3` → 3 со всех сторон
- `0,5,0,5` → 5 сверху и снизу, 0 по бокам
- `0,0,0,10` → 10 только снизу

---

## ЧАСТЬ 9: ЧАСТЫЕ ОШИБКИ

| Ошибка | Причина | Решение |
|--------|---------|---------|
| `txtLogin не существует` | Забыли задать Name элементу | В дизайнере выбрать элемент → Свойства → (Name) |
| `Button_Click не найден` | Метод не переименован | В XAML и .cs файле переименовать одинаково |
| `dgData не существует` | DataGrid не назван | Выбрать DataGrid → Свойства → (Name) = `dgData` |
| Кнопка не реагирует | Нет обработчика Click | Двойной клик на кнопке в дизайнере |
| Данные не отображаются | LoadData не вызван | В конструкторе добавить `LoadData();` |
| `dgData не в Row 0` | Не указан Grid.Row | В XAML добавить `Grid.Row="0"` к DataGrid |
