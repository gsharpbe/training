using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;

namespace Training.Api.Services.Excel
{
    public static class ExcelService
    {
        public static void ApplyFormatting(ExcelPivotTable pivotTable)
        {
            pivotTable.Compact = false;
            pivotTable.CompactData = false;
            pivotTable.Outline = false;
            pivotTable.OutlineData = false;
            pivotTable.Indent = 0;
            pivotTable.UseAutoFormatting = true;
            pivotTable.ShowMemberPropertyTips = false;
            pivotTable.DataOnRows = false;
            pivotTable.ShowDrill = true;
            pivotTable.EnableDrill = true;
            pivotTable.RowGrandTotals = true;
            pivotTable.ColumGrandTotals = true;
            pivotTable.MultipleFieldFilters = true;

            var fields = pivotTable.Fields.ToList();

            foreach (var field in fields)
            {
                field.Compact = false;
                field.Outline = false;
                field.ShowAll = false;
                field.SubtotalTop = false;
                field.SubTotalFunctions = eSubTotalFunctions.None;
            }

            pivotTable.TableStyle = TableStyles.Medium6;
        }

        public static byte[] GenerateWorksheet<T>(IEnumerable<T> itemsSource)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var worksheetData = excelPackage.Workbook.Worksheets.Add("Data");
                worksheetData.DefaultColWidth = 30;

                worksheetData.Cells["A1"].LoadFromCollection(itemsSource, true);

                return excelPackage.GetAsByteArray();
            }
        }

        public static List<string> ReadFieldHeaders(Stream stream)
        {
            var fieldHeaders = new List<string>();

            if (stream == null)
            {
                return fieldHeaders;
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return fieldHeaders;
                }

                fieldHeaders = GetFieldHeaders(worksheet);
            }

            return fieldHeaders;
        }

        private static List<string> GetFieldHeaders(ExcelWorksheet worksheet)
        {
            var fieldHeaders = new List<string>();

            var column = 1;
            var value = worksheet.GetValue<string>(1, column++);

            while (!string.IsNullOrWhiteSpace(value))
            {
                fieldHeaders.Add(value);
                value = worksheet.GetValue<string>(1, column++);
            }

            return fieldHeaders;
        }

        public static WorksheetValues Read(Stream stream)
        {
            var worksheetValues = new WorksheetValues();

            if (stream == null)
            {
                return worksheetValues;
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return worksheetValues;
                }

                worksheetValues.Headers = GetFieldHeaders(worksheet);

                var start = worksheet.Dimension.Start;
                var end = worksheet.Dimension.End;

                // start at the 2nd row (first row is the header)
                for (var rowIndex = start.Row + 1; rowIndex <= end.Row; rowIndex++)
                {
                    var record = new Dictionary<string, WorksheetValue>();

                    for (var columnIndex = start.Column; columnIndex <= worksheetValues.Headers.Count; columnIndex++)
                    {

                        var value = new WorksheetValue
                        {
                            Row = rowIndex,
                            Column = columnIndex,
                            Value = worksheet.GetValue(rowIndex, columnIndex)
                        };

                        record.Add(worksheetValues.Headers[columnIndex - 1], value);
                    }

                    worksheetValues.Content.Add(record);
                }
            }

            return worksheetValues;
        }

        public static byte[] Update(Stream stream, WorksheetValues worksheetValues)
        {
            if (stream == null)
            {
                return null;
            }

            using (var excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return null;
                }

                // clear all comments
                foreach (var comment in worksheet.Comments.Cast<ExcelComment>().ToList())
                {
                    worksheet.Comments.Remove(comment);
                }

                // header errors
                var col = worksheetValues.Headers.Count + 1;
                foreach (var header in worksheetValues.MissingHeaders)
                {
                    worksheet.SetValue(1, col, header);
                    var cell = worksheet.Cells[1, col];

                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    cell.AddComment(worksheetValues.MissingHeaderComment, "SYSTEM");

                    col++;
                }

                // add error column
                var errorColumn = worksheetValues.Headers.Count + 1;
                var errorHeaderCell = worksheet.Cells[1, errorColumn];

                errorHeaderCell.Value = "ERROR!";

                errorHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                errorHeaderCell.Style.Fill.BackgroundColor.SetColor(Color.Red);
                errorHeaderCell.Style.Font.Bold = true;


                // cell errors
                var cellErrors = worksheetValues.Content.SelectMany(x => x.Values).Where(x => x.HasError).ToList();

                foreach (var cellError in cellErrors)
                {
                    var cell = worksheet.Cells[cellError.Row, cellError.Column];

                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.Red);

                    cell.AddComment(cellError.Error, "SYSTEM");

                    worksheet.SetValue(cellError.Row, errorColumn, 1);
                }

                return excelPackage.GetAsByteArray();
            }
        }
    }
}