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
using System.Runtime.CompilerServices;
using CodeBarProcAudit.Extensions;

namespace CodeBarProcAudit.ViewModels
{
    internal class FilterViewModel: INotifyPropertyChanged
    {
        private string _folderPath;
        private string _cBarFilePath;
        private string _excelFile;

        private List<Item> InventoryData;
        private ICollectionView _dataGridCollection;
        public ICollectionView DataGridCollection
        {
            get { return _dataGridCollection; }
            set { _dataGridCollection = value; OnPropertyChanged(); }
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                OnPropertyChanged();
                FilterCollection();
            }
        }

        public FilterViewModel()
        {
            SetFilePaths();

            var table = GetExcelFile(_folderPath);

            LoadData(table).Await(HandleError);
        }

        private void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryData = await LoadInventoryTable(table);

            DataGridCollection = CollectionViewSource.GetDefaultView(InventoryData);
            DataGridCollection.Filter = new Predicate<object>(Filter);
        }


        private void SetFilePaths()
        {
            _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];
            _cBarFilePath = $"{_folderPath}\\Штрихкоды.html";
        }

        FileInfo GetExcelFile(string folderPath)
        {

            _excelFile = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith(".xlsx") || s.EndsWith(".xls")).Select(f => f).FirstOrDefault();

            if (string.IsNullOrEmpty(_excelFile))
            {
                //_excelFile = "Отсутствует инвентарная таблица";
                MessageBox.Show("Отсутствует инвентарная таблица");
                return null;
            }

            FileInfo fileInf = new FileInfo(_excelFile);
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
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }


}
