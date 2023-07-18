using CodeBarProcAudit.Model;
using IronBarCode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CodeBarProcAudit.Services
{
    public class CodeBarService : ICodeBarService
    {
        public void GeneratedBarcodeHtml(IEnumerable<Item> itemsCollection, string filePath)
        {
            if (itemsCollection != null && itemsCollection.Count() > 0)
            {
                var barTags = PopulateBarTags(itemsCollection);
                WriteBarcodeResult(barTags, filePath);
            }
            else
            {
                MessageBox.Show("Отсутствует инвентаризационная база");
            }
        }

        private List<string> PopulateBarTags(IEnumerable<Item> itemsCollection)
        {
            var barTags = new List<string>();
            foreach (var item in itemsCollection)
            {
                if (item != null && !string.IsNullOrEmpty(item.Inv))
                {
                    var barCode = CreateBarCode(item);
                    barTags.Add(barCode);
                }
            }
            return barTags;
        }

        private string CreateBarCode(Item item)
        {
            var barCode = BarcodeWriter.CreateBarcode(item.Inv, BarcodeEncoding.Code128).ResizeTo(50, 50).SetMargins(10);
            barCode.AddAnnotationTextAboveBarcode(item.Info1);
            barCode.AddBarcodeValueTextBelowBarcode();
            var bc = barCode.ToHtmlTag();
            return bc;
        }

        private void WriteBarcodeResult(IEnumerable<string> barTags, string filePath)
        {
            File.WriteAllLines(filePath, barTags);
        }
    }
}
