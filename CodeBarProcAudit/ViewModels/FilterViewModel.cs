using CodeBarProcAudit.Commands;
using CodeBarProcAudit.Extensions;
using CodeBarProcAudit.Model;
using CodeBarProcAudit.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CodeBarProcAudit.ViewModels
{
    internal class FilterViewModel: BaseViewModel
    {
        private List<Item> InventoryItems = new List<Item>();
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

        private bool _canGenerate = true;
        public bool CanGenerate
        {
            get { return _canGenerate; }
            set { _canGenerate = value; OnPropertyChanged(); }
        }


        public AsyncCommand GenerateCodeBarCommandAsync { get; }

        public FilterViewModel() : base()
        {
            GenerateCodeBarCommandAsync = new AsyncCommand(OnGenerateCodeBarAsyncExecuted, CanGenerateCodeBarExecute);

            LoadData(tableFileInfo).Await(HandleError);
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryItems = new(await EPPlusService.LoadInventoryTable(table));

            DataGridCollection = CollectionViewSource.GetDefaultView(InventoryItems);
            DataGridCollection.Filter = new Predicate<object>(Filter);
        }

        ///Synchronos CB generation:
        private bool CanGenerateCodeBarExecute(object arg)
        {
            if (InventoryItems.Count > 0)
                return true;
            return false;
        }

        private async Task OnGenerateCodeBarAsyncExecuted() //object obj
        {
            CanGenerate = false;

            if (File.Exists(_cBarFilePath))
            {
                File.Delete(_cBarFilePath);
            }

            Mouse.OverrideCursor = Cursors.Wait;
            await Task.Run(() =>
            {
                CodeBarService.GeneratedBarcodeHtml(InventoryItems, _cBarFilePath);
                MessageBox.Show("Штрихкоды \nсгенерированы!");
            });
            Mouse.OverrideCursor = null;
            CanGenerate = true;
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
    }
}
