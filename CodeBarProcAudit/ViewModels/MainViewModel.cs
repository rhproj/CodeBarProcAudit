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
using System.Windows.Input;

namespace CodeBarProcAudit.ViewModels
{
    internal class MainViewModel: BaseViewModel
    {
        private ObservableCollection<Item> _inventoryData = new();
        public ObservableCollection<Item> InventoryData
        {
            get { return _inventoryData; }
            set { _inventoryData = value; OnPropertyChanged(); } //   
        }

        public RelayCommand GenerateCodeBarCommand { get;}

        public MainViewModel() : base()
        {
            GenerateCodeBarCommand = new RelayCommand(OnGenerateCodeBarExecuted, CanGenerateCodeBarExecute);
            LoadData(tableFileInfo).Await(HandleError);
        }

        ///Synchronos CB generation:
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
    }
}
