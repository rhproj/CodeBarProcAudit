using CodeBarProcAudit.Commands;
using CodeBarProcAudit.Extensions;
using CodeBarProcAudit.Model;
using CodeBarProcAudit.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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

            FileInfo fI = new FileInfo(_excelFile);
            LoadData(fI).Await(HandleError);  
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
