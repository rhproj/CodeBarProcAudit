using CodeBarProcAudit.Model;
using IronBarCode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CodeBarProcAudit.Services
{
    internal static class CodeBarService
    {
        public static void GeneratedBarcodeHtml(IEnumerable<Item> itemsCollection, string filePath)
        {
            List<string> barTags = new List<string>();

            if (itemsCollection != null && itemsCollection.Count() > 0)
            {
                foreach (var item in itemsCollection.Skip(1))
                {
                    if (item != null && !string.IsNullOrEmpty(item.Inv))
                    {
                        var barCode = IronBarCode.BarcodeWriter.CreateBarcode(item.Inv, BarcodeEncoding.Code128).ResizeTo(50, 50).SetMargins(10);
                        barCode.AddAnnotationTextAboveBarcode(item.Info1);
                        barCode.AddBarcodeValueTextBelowBarcode();
                        var bc = barCode.ToHtmlTag();

                        barTags.Add(bc);
                    }
                }

                var barText = string.Join("   ", barTags);

                WriteBarcodeResult(barTags, filePath);
            }
            else
            {
                MessageBox.Show("Данные не соответст");
            }
        }

        private static void WriteBarcodeResult(IEnumerable<string> barTags, string filePath)
        {
            File.WriteAllLines(filePath, barTags);
        }
    }
}
