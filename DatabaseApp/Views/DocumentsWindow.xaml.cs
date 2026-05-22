using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using DatabaseApp.Models;

namespace DatabaseApp.Views
{
    public partial class DocumentsWindow : Window
    {
        private User currentUser;

        public DocumentsWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM documents ORDER BY document_date DESC, document_number";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);

                List<Document> documents = new List<Document>();
                foreach (DataRow row in dataTable.Rows)
                {
                    documents.Add(new Document
                    {
                        DocumentId = Convert.ToInt32(row["document_id"]),
                        DocumentNumber = row["document_number"].ToString() ?? "",
                        DocumentDate = Convert.ToDateTime(row["document_date"]),
                        DocumentType = row["document_type"].ToString() ?? "",
                        TotalAmount = Convert.ToDecimal(row["total_amount"]),
                        Description = row["description"].ToString() ?? ""
                    });
                }

                DocumentsDataGrid.ItemsSource = documents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddDocumentWindow window = new AddDocumentWindow(currentUser);
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsDataGrid.SelectedItem is Document selected)
            {
                var result = MessageBox.Show($"Удалить документ '{selected.DocumentNumber}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM documents WHERE document_id = {selected.DocumentId}";
                        DatabaseHelper.ExecuteNonQuery(query);
                        MessageBox.Show("Документ удален", "Успех", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите документ для удаления", "Внимание", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
