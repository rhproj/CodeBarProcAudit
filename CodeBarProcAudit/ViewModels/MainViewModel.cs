using CodeBarProcAudit.Commands;
using CodeBarProcAudit.Extensions;
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
    internal class MainViewModel: INotifyPropertyChanged
    {
        private string _folderPath;
        private string _cBarFilePath;
        private string _excelFile;

        //string path = $"{AppDomain.CurrentDomain.BaseDirectory}"; //\\Инвентарка

        private ObservableCollection<Item> _inventoryData = new();
        public ObservableCollection<Item> InventoryData
        {
            get { return _inventoryData; }
            set { _inventoryData = value; OnPropertyChanged(); } //   
        }

        public RelayCommand GenerateCodeBarCommand { get;}

        public MainViewModel()
        {
            SetFilePaths();

            GenerateCodeBarCommand = new RelayCommand(OnGenerateCodeBarExecuted, CanGenerateCodeBarExecute);
            
            var table = GetExcelFile(_folderPath);

            LoadData(table).Await(HandleError);
        }

        private void SetFilePaths()
        {
            _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];
            _cBarFilePath = $"{_folderPath}\\Штрихкоды.html";
        }

        private bool CanGenerateCodeBarExecute(object arg)
        {
            if (InventoryData.Count > 0)
                return true;
            return false;
        }

        private void OnGenerateCodeBarExecuted(object obj)
        {
            CodeBarService.GeneratedBarcodeHtml(InventoryData, _cBarFilePath);
            MessageBox.Show("Готово!");
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryData = new (await EPPlusService.LoadInventoryTable(table));
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

        private void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
