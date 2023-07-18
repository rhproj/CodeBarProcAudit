using CodeBarProcAudit.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Services
{
    public class EPPlusService : IExcelService
    {
        public async Task<IEnumerable<Item>> LoadInventoryTable(FileInfo file)
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
                while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false)
                {
                    Item item = new();

                    Type myType = item.GetType();
                    var propInfo = myType.GetProperties();

                    foreach (var pi in propInfo)
                    {
                        pi.SetValue(item, worksheet.Cells[row, col++].Value, null);
                    }

                    output.Add(item);
                    row++;
                    col = 1;
                }
            }
            return output;
        }

        public async Task SaveToExcel(IEnumerable<Item> data, FileInfo file)
        {
            using (var package = new ExcelPackage(file))
            {
                if (package != null)
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    var range = worksheet.Cells["A2"].LoadFromCollection(data, false);
                    range.AutoFitColumns();

                    await package.SaveAsync();
                }
            }
        }
    }
}
