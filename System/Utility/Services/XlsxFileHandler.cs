using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace UniversityCertificates.System.Utility.Services;

public static class XlsxFileHandler
{
    public static byte[] ExportToExcel<T>(List<T> objects)
        where T : class
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (
                SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(
                    stream,
                    SpreadsheetDocumentType.Workbook
                )
            )
            {
                // Add a WorkbookPart to the document
                WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook
                Sheets sheets = spreadsheet.WorkbookPart!.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = typeof(T).Name,
                };
                sheets.Append(sheet);

                // Get the SheetData
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

                // Add header row
                Row headerRow = new Row();
                var properties = typeof(T).GetProperties();

                foreach (var prop in properties)
                {
                    Cell cell = new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(prop.Name),
                    };
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                // Add data rows
                foreach (var item in objects)
                {
                    Row newRow = new Row();

                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item)?.ToString() ?? string.Empty;
                        Cell cell = new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(value),
                        };
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookPart.Workbook.Save();
            }

            return stream.ToArray();
        }
    }

    public static List<T> ImportFromExcel<T>(Stream excelFileStream)
        where T : class, new()
    {
        var result = new List<T>();
        var properties = typeof(T).GetProperties();

        using (var spreadsheet = SpreadsheetDocument.Open(excelFileStream, false))
        {
            var workbookPart = spreadsheet.WorkbookPart;
            var worksheetPart = workbookPart?.WorksheetParts.First();
            var sheetData = worksheetPart?.Worksheet.Elements<SheetData>().First();

            if (sheetData == null)
            {
                throw new InvalidOperationException("No worksheet data found in the Excel file.");
            }

            var rows = sheetData.Elements<Row>().ToList();
            if (rows.Count < 2) // At least header and one data row
            {
                throw new InvalidOperationException(
                    "Excel file must contain at least a header row and one data row."
                );
            }

            // Get header row
            var headerRow = rows.First();
            var headerCells = headerRow.Elements<Cell>().ToList();
            var headerNames = headerCells.Select(cell => GetCellValue(cell, workbookPart)).ToList();

            // Map header names to properties
            var propertyMap = new Dictionary<int, PropertyInfo>();
            for (int i = 0; i < headerNames.Count; i++)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headerNames[i], StringComparison.OrdinalIgnoreCase)
                );
                if (property != null)
                {
                    propertyMap[i] = property;
                }
            }

            // Process data rows
            foreach (var row in rows.Skip(1)) // Skip header row
            {
                var cells = row.Elements<Cell>().ToList();

                // Skip empty rows
                if (cells.All(c => string.IsNullOrEmpty(GetCellValue(c, workbookPart))))
                {
                    continue;
                }

                var item = new T();

                for (int i = 0; i < cells.Count; i++)
                {
                    if (propertyMap.TryGetValue(i, out var property))
                    {
                        var value = GetCellValue(cells[i], workbookPart);
                        if (!string.IsNullOrEmpty(value))
                        {
                            try
                            {
                                // Handle numeric types more carefully
                                if (
                                    property.PropertyType == typeof(int)
                                    || property.PropertyType == typeof(long)
                                )
                                {
                                    if (int.TryParse(value, out int intValue))
                                    {
                                        property.SetValue(item, intValue);
                                    }
                                }
                                else if (
                                    property.PropertyType == typeof(double)
                                    || property.PropertyType == typeof(float)
                                )
                                {
                                    if (double.TryParse(value, out double doubleValue))
                                    {
                                        property.SetValue(item, doubleValue);
                                    }
                                }
                                else
                                {
                                    var convertedValue = Convert.ChangeType(
                                        value,
                                        property.PropertyType
                                    );
                                    property.SetValue(item, convertedValue);
                                }
                            }
                            catch
                            {
                                // Skip invalid values
                            }
                        }
                    }
                }

                result.Add(item);
            }
        }

        return result;
    }

    private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        if (cell.CellValue == null)
        {
            return string.Empty;
        }

        string value = cell.CellValue.Text;

        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            var stringTable = workbookPart?.SharedStringTablePart?.SharedStringTable;
            if (stringTable != null && int.TryParse(value, out int index))
            {
                value = stringTable.ElementAt(index).InnerText;
            }
        }
        else if (cell.DataType == null && double.TryParse(value, out double numericValue))
        {
            // For numeric values, remove the decimal point if it's a whole number
            value =
                numericValue == Math.Floor(numericValue) ? ((int)numericValue).ToString() : value;
        }

        return value;
    }
}
