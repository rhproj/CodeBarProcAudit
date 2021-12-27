using CodeBarProcAudit.Model;
//using CodeBarProcAudit.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CodeBarProcAudit.Model;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CodeBarProcAudit.ViewModels
{
    public class FilterViewModel: INotifyPropertyChanged
    {
        string folderPath;
        string cBarFilePath;
        string excelFile;

        //private ObservableCollection<Item> _inventoryData = new();
        //public ObservableCollection<Item> InventoryData
        //{
        //    get { return _inventoryData; }
        //    set { _inventoryData = value; NotifyPropertyChanged("InventoryData"); } //   
        //}
        public List<Item> InventoryData;

        private ICollectionView _dataGridCollection;
        public ICollectionView DataGridCollection
        {
            get { return _dataGridCollection; }
            set { _dataGridCollection = value; NotifyPropertyChanged("DataGridCollection"); }
        }

        public List<Item> InvTestData;


        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }

        public FilterViewModel()
        {
            SetFilePaths();

            var table = GetExcelFile(folderPath);

            LoadData(table).Await(HandleError);
        }

        private void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private async Task LoadData(FileInfo table)
        {
            //InventoryData = new(await EPPlusService.LoadInventoryTable(table));
            InventoryData = await LoadInventoryTable(table);

            DataGridCollection = CollectionViewSource.GetDefaultView(InventoryData);
            DataGridCollection.Filter = new Predicate<object>(Filter);
        }


        private void SetFilePaths()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];
            cBarFilePath = $"{folderPath}\\Штрихкоды.html";
        }

        FileInfo GetExcelFile(string folderPath)
        {

            excelFile = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith(".xlsx") || s.EndsWith(".xls")).Select(f => f).FirstOrDefault();

            if (string.IsNullOrEmpty(excelFile))
            {
                //ExcelFile = "Отсутствует инвентарная таблица";
                MessageBox.Show("Отсутствует инвентарная таблица");
                return null;
            }

            FileInfo fileInf = new FileInfo(excelFile);
            return fileInf;
        }


        #region Filtering
        private void FilterCollection()
        {
            if (_dataGridCollection != null)
            {
                _dataGridCollection.Refresh();
            }
        }

        public bool Filter(object obj)
        {
            var data = obj as Item;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return (!string.IsNullOrEmpty(data.Inv) && data.Inv.Contains(_filterString))
                        || (!string.IsNullOrEmpty(data.Info1) && data.Info1.Contains(_filterString))
                        || (!string.IsNullOrEmpty(data.Info2) && data.Info2.Contains(_filterString));
                }
                return true;
            }
            return false;
        }
        #endregion


        public static async Task<List<Item>> LoadInventoryTable(FileInfo file)
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

                    Type myType = item.GetType();
                    var pinfo = myType.GetProperties();

                    foreach (var pi in pinfo)
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public static class TaskExtensions
    {
        public async static void Await(this Task tsk, Action<Exception> errorCB)
        {
            try
            {
                await tsk;
            }
            catch (Exception ex)
            {
                errorCB?.Invoke(ex);
            }
        }
    }
}
