using CodeBarProcAudit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Services
{
    public interface ICodeBarService
    {
        void GeneratedBarcodeHtml(IEnumerable<Item> itemsCollection, string filePath);
    }
}
