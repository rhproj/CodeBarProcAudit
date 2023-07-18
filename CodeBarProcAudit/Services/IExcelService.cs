using CodeBarProcAudit.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeBarProcAudit.Services
{
    public interface IExcelService
    {
        Task<IEnumerable<Item>> LoadInventoryTable(FileInfo file);
        Task SaveToExcel(IEnumerable<Item> data, FileInfo file);
    }
}