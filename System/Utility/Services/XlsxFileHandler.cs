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
}
