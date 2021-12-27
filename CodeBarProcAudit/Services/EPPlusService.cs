﻿using CodeBarProcAudit.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

                int row = 1; int col = 1;
                //читаем данные начиная с А1, пока есть данные(до конца таблицы)
                while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false) //если там есть значение - переведи ее в стринг
                {
                    Item item = new();

                    //TestClass t = new TestClass();

                    // Get the type and PropertyInfo.
                    Type myType = item.GetType();
                    var pinfo = myType.GetProperties();

                    foreach (var pi in pinfo)
                    {
                        pi.SetValue(item,worksheet.Cells[row, col++].Value,null);
                        //col++;
                        // do stuff here
                    }

                    #region was
                    //for (i = 1; col < 10; col++)
                    //{
                    //    item.Inv = worksheet.Cells[].Value?.ToString();
                    //}

                    //while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false)
                    //{
                    //    item.Add(worksheet.Cells[row, col].Value.ToString());
                    //    col++;
                    //} 
                    #endregion

                    output.Add(item);
                    row++;
                    col = 1;
                }
            }
            return output;
        }




        public static async Task<IEnumerable<List<string>>> LoadInventoryTableS(FileInfo file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<List<string>> output = new();

            using (var package = new ExcelPackage(file))
            {
                if (file.Exists)
                {
                    await package.LoadAsync(file);
                }

                var worksheet = package.Workbook.Worksheets[PositionID: 0];

                int row = 2; int col = 1;
                //читаем данные начиная с А2, пока есть данные(до конца таблицы)
                while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false) //если там есть значение - переведи ее в стринг
                {
                    List<string> item = new();
                    while (string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Value?.ToString()) == false)
                    {
                        item.Add(worksheet.Cells[row, col].Value.ToString());
                        col++;
                    }

                    output.Add(item); //yield return item;
                    row++;
                }
            }
            return output;
        }











        //public static IEnumerable<string> GetDataLines(FileInfo file)
        //{
        //    using var data_stream = 
        //    using var data_reader = new StreamReader();
        //    //using var data
        //    while (!data_reader.)
        //    {

        //    }
        //}



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
