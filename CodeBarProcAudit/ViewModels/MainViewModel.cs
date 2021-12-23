using CodeBarProcAudit.Commands;
using CodeBarProcAudit.Model;
using CodeBarProcAudit.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CodeBarProcAudit.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        //string path = $"{AppDomain.CurrentDomain.BaseDirectory}"; //\\Инвентарка
        //public string excelFile;

        private string _excelFile;
        public string ExcelFile
        {
            get { return _excelFile; }
            set { _excelFile = value; } // OnPropertyChanged(); 
        }

        //private ObservableCollection<List<string>> _inventoryData;
        //public ObservableCollection<List<string>> InventoryData
        //{
        //    get { return _inventoryData; }
        //    set { _inventoryData = value; OnPropertyChanged("InventoryData"); } //   
        //}

        private ObservableCollection<Item> _inventoryData;
        public ObservableCollection<Item> InventoryData
        {
            get { return _inventoryData; }
            set { _inventoryData = value; OnPropertyChanged(); } //   
        }


        public RelayCommand GenerateCodeBarCommand { get;}

        public MainViewModel()
        {
            var table = GetExcelFile();

            LoadData(table);

            GenerateCodeBarCommand = new RelayCommand(OnGenerateCodeBarExecuted, CanGenerateCodeBarExecute);
        }

        private bool CanGenerateCodeBarExecute(object arg)
        {
            if (InventoryData.Count > 0)
                return true;
            return false;
        }

        private void OnGenerateCodeBarExecuted(object obj)
        {
            throw new NotImplementedException();
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryData = new (await EPPlusService.LoadInventoryTable(table));
        }

        FileInfo GetExcelFile()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];

            ExcelFile = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith(".xlsx") || s.EndsWith(".xls")).Select(f => f).FirstOrDefault();

            if (string.IsNullOrEmpty(ExcelFile))
            {
                //ExcelFile = "Отсутствует инвентарная таблица";
                MessageBox.Show("Отсутствует инвентарная таблица");
                return null;
            }

            FileInfo fileInf = new FileInfo(ExcelFile);
            return fileInf;
        }

        //IEnumerable<string> InventoryName = await EPPlusService.LoadInventoryTable();


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
