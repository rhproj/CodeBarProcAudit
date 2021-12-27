using CodeBarProcAudit.Model;
using IronBarCode;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeBarProcAudit.Services
{
    internal static class CodeBarService
    {
        public static void GeneratedBarcodeHtml(IEnumerable<IItem> itemsCollection, string filePath)
        {
            List<string> barTags = new List<string>();

            foreach (var item in itemsCollection.Skip(1))
            {
                var barCode = IronBarCode.BarcodeWriter.CreateBarcode(item.Inv, BarcodeEncoding.Code128).ResizeTo(50, 50).SetMargins(10);
                barCode.AddBarcodeValueTextBelowBarcode();
                var bc = barCode.ToHtmlTag();

                barTags.Add(bc);
            }

            var barText = string.Join("   ", barTags);

            WriteBarcodeResult(barTags, filePath);
        }

        private static void WriteBarcodeResult(IEnumerable<string> barTags, string filePath)
        {
            File.WriteAllLines(filePath, barTags);
        }
    }
}
