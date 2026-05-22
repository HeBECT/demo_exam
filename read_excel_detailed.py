# -*- coding: utf-8 -*-
import openpyxl
import json

def read_excel_all_data(filename):
    """Читает все данные из Excel файла"""
    wb = openpyxl.load_workbook(filename)
    ws = wb.active
    
    all_data = []
    for row in ws.iter_rows(values_only=True):
        if any(row):  # Пропускаем полностью пустые строки
            all_data.append(list(row))
    
    return all_data

# Список файлов
files = {
    "Заказ покупателя": "Таблицы/Заказ покупателя.xlsx",
    "Производство": "Таблицы/Производство.xlsx",
    "Расчет стоимости": "Таблицы/Расчет стоимости продукции.xlsx",
    "Спецификация": "Таблицы/Спецификация.xlsx",
    "Цены": "Таблицы/Цены.xlsx"
}

print("=" * 80)
for name, file in files.items():
    print(f"\n{name}:")
    print("-" * 80)
    try:
        data = read_excel_all_data(file)
        for i, row in enumerate(data[:15], 1):  # Показываем первые 15 строк
            # Убираем None в конце
            row_clean = []
            for cell in row:
                if cell is not None:
                    row_clean.append(str(cell))
                else:
                    row_clean.append("")
            print(f"Строка {i}: {' | '.join(row_clean)}")
    except Exception as e:
        print(f"Ошибка: {e}")
    print()

print("=" * 80)
