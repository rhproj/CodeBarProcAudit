using CodeBarProcAudit.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Services
{
    internal class EPPlusService
    {
        public static async Task<IEnumerable<Item>> LoadInventoryTable(FileInfo file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<Item> output = new();

            using (var package = new ExcelPackage(file))
            {
                if (file.Exists)
                {
                    await package.LoadAsync(file);
                }

                var worksheet = package.Workbook.Worksheets[PositionID: 0];

                int row = 2; int col = 1;
                //читаем данные начиная с А1, пока есть данные(до конца таблицы)
                while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false) //если там есть значение - переведи ее в стринг
                {
                    Item item = new();

                    Type myType = item.GetType();
                    var pinfo = myType.GetProperties();

                    foreach (var pi in pinfo)
                    {
                        pi.SetValue(item,worksheet.Cells[row, col++].Value,null);
                    }

                    output.Add(item);
                    row++;
                    col = 1;
                }
            }
            return output;
        }

        public static async Task SaveToExcel(IEnumerable<string> data, FileInfo file)
        {
            using (var package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets.Add("Инвентар");

                var range = worksheet.Cells["A1"].LoadFromCollection(data, true);

                range.AutoFitColumns();

                worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Row(1).Style.Font.Size = 20;
                worksheet.Row(1).Style.Font.Bold = true;

                await package.SaveAsync();
            }
        }
    }
}
