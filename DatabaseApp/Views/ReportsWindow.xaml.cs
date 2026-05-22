using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Text;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public class BalanceSheetRow
    {
        public string AccountNumber { get; set; } = "";
        public string AccountName { get; set; } = "";
        public decimal OpeningBalanceDebit { get; set; }
        public decimal OpeningBalanceCredit { get; set; }
        public decimal TurnoverDebit { get; set; }
        public decimal TurnoverCredit { get; set; }
        public decimal ClosingBalanceDebit { get; set; }
        public decimal ClosingBalanceCredit { get; set; }
    }

    public partial class ReportsWindow : Window
    {
        private User currentUser;

        public ReportsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            
            // Установка периода по умолчанию (текущий месяц)
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDatePicker.SelectedDate = DateTime.Now;
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период для формирования отчёта", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала не может быть больше даты окончания", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var report = GenerateBalanceSheet(startDate, endDate);
                ReportDataGrid.ItemsSource = report;

                // Подсчёт итогов
                decimal totalDebit = report.Sum(r => r.TurnoverDebit);
                decimal totalCredit = report.Sum(r => r.TurnoverCredit);

                TotalDebitTextBlock.Text = totalDebit.ToString("N2");
                TotalCreditTextBlock.Text = totalCredit.ToString("N2");

                StatusTextBlock.Text = $"Отчёт сформирован за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}. Записей: {report.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<BalanceSheetRow> GenerateBalanceSheet(DateTime startDate, DateTime endDate)
        {
            var result = new List<BalanceSheetRow>();

            // Получение всех счетов
            string accountsQuery = "SELECT account_number, account_name, account_type FROM accounts ORDER BY account_number";
            DataTable accountsTable = DatabaseHelper.ExecuteQuery(accountsQuery);

            foreach (DataRow accountRow in accountsTable.Rows)
            {
                string accountNumber = accountRow["account_number"].ToString() ?? "";
                string accountName = accountRow["account_name"].ToString() ?? "";
                string accountType = accountRow["account_type"].ToString() ?? "";

                // Начальное сальдо (до начала периода)
                var openingBalance = CalculateBalance(accountNumber, DateTime.MinValue, startDate.AddDays(-1));

                // Обороты за период
                var turnover = CalculateTurnover(accountNumber, startDate, endDate);

                // Конечное сальдо
                decimal closingDebit = 0;
                decimal closingCredit = 0;

                if (accountType == "active")
                {
                    // Активный счёт
                    decimal balance = openingBalance.Debit - openingBalance.Credit + turnover.Debit - turnover.Credit;
                    if (balance > 0)
                        closingDebit = balance;
                    else if (balance < 0)
                        closingCredit = Math.Abs(balance);
                }
                else if (accountType == "passive")
                {
                    // Пассивный счёт
                    decimal balance = openingBalance.Credit - openingBalance.Debit + turnover.Credit - turnover.Debit;
                    if (balance > 0)
                        closingCredit = balance;
                    else if (balance < 0)
                        closingDebit = Math.Abs(balance);
                }
                else // active-passive
                {
                    // Активно-пассивный счёт
                    decimal debitBalance = openingBalance.Debit + turnover.Debit;
                    decimal creditBalance = openingBalance.Credit + turnover.Credit;
                    
                    if (debitBalance > creditBalance)
                        closingDebit = debitBalance - creditBalance;
                    else if (creditBalance > debitBalance)
                        closingCredit = creditBalance - debitBalance;
                }

                // Добавляем строку только если есть обороты или сальдо
                if (openingBalance.Debit != 0 || openingBalance.Credit != 0 || 
                    turnover.Debit != 0 || turnover.Credit != 0 ||
                    closingDebit != 0 || closingCredit != 0)
                {
                    result.Add(new BalanceSheetRow
                    {
                        AccountNumber = accountNumber,
                        AccountName = accountName,
                        OpeningBalanceDebit = openingBalance.Debit,
                        OpeningBalanceCredit = openingBalance.Credit,
                        TurnoverDebit = turnover.Debit,
                        TurnoverCredit = turnover.Credit,
                        ClosingBalanceDebit = closingDebit,
                        ClosingBalanceCredit = closingCredit
                    });
                }
            }

            return result;
        }

        private (decimal Debit, decimal Credit) CalculateBalance(string accountNumber, DateTime startDate, DateTime endDate)
        {
            decimal debit = 0;
            decimal credit = 0;

            try
            {
                // Дебетовые обороты
                string debitQuery = $@"
                    SELECT COALESCE(SUM(amount), 0) as total 
                    FROM transactions 
                    WHERE debit_account = '{accountNumber}' 
                    AND transaction_date >= '{startDate:yyyy-MM-dd}' 
                    AND transaction_date <= '{endDate:yyyy-MM-dd}'";
                
                var debitResult = DatabaseHelper.ExecuteScalar(debitQuery);
                if (debitResult != null && debitResult != DBNull.Value)
                    debit = Convert.ToDecimal(debitResult);

                // Кредитовые обороты
                string creditQuery = $@"
                    SELECT COALESCE(SUM(amount), 0) as total 
                    FROM transactions 
                    WHERE credit_account = '{accountNumber}' 
                    AND transaction_date >= '{startDate:yyyy-MM-dd}' 
                    AND transaction_date <= '{endDate:yyyy-MM-dd}'";
                
                var creditResult = DatabaseHelper.ExecuteScalar(creditQuery);
                if (creditResult != null && creditResult != DBNull.Value)
                    credit = Convert.ToDecimal(creditResult);
            }
            catch
            {
                // Если нет данных, возвращаем нули
            }

            return (debit, credit);
        }

        private (decimal Debit, decimal Credit) CalculateTurnover(string accountNumber, DateTime startDate, DateTime endDate)
        {
            return CalculateBalance(accountNumber, startDate, endDate);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportDataGrid.Items.Count == 0)
            {
                MessageBox.Show("Сначала сформируйте отчёт", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    FileName = $"Оборотно-сальдовая_ведомость_{DateTime.Now:yyyy-MM-dd}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportToCsv(saveFileDialog.FileName);
                    MessageBox.Show($"Отчёт успешно экспортирован в файл:\n{saveFileDialog.FileName}", 
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCsv(string filePath)
        {
            var sb = new StringBuilder();
            
            // Заголовок
            sb.AppendLine("Счёт;Наименование счёта;Сальдо начальное Дт;Сальдо начальное Кт;Оборот Дт;Оборот Кт;Сальдо конечное Дт;Сальдо конечное Кт");

            // Данные
            foreach (BalanceSheetRow row in ReportDataGrid.Items)
            {
                sb.AppendLine($"{row.AccountNumber};{row.AccountName};{row.OpeningBalanceDebit:N2};{row.OpeningBalanceCredit:N2};{row.TurnoverDebit:N2};{row.TurnoverCredit:N2};{row.ClosingBalanceDebit:N2};{row.ClosingBalanceCredit:N2}");
            }

            // Итоги
            sb.AppendLine();
            sb.AppendLine($"Итого оборотов:;;;;{TotalDebitTextBlock.Text};{TotalCreditTextBlock.Text};;");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
