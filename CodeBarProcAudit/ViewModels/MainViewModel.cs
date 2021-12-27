using CodeBarProcAudit.Commands;
using CodeBarProcAudit.Extensions;
using CodeBarProcAudit.Model;
using CodeBarProcAudit.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace CodeBarProcAudit.ViewModels
{
    internal class MainViewModel: INotifyPropertyChanged
    {
        private string _folderPath;
        private string _cBarFilePath;
        private string _excelFile;
        public FileInfo tableFileInfo;

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

            tableFileInfo = GetExcelFile(_folderPath);

            LoadData(tableFileInfo).Await(HandleError);
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
                MessageBox.Show("Отсутствует инвентарная таблица");

                Environment.Exit(0);  //return null;
            }

            FileInfo fileInf = new FileInfo(_excelFile);
            return fileInf;
        }

        public void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
