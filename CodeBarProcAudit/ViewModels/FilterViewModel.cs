﻿using CodeBarProcAudit.Commands;
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
    public class FilterViewModel: BaseViewModel
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
            set 
            { 
                _canGenerate = value; 
                OnPropertyChanged(); 
            }
        }

        public AsyncCommand LoadDataAsync { get; }
        public AsyncCommand SaveDataAsync { get; }
        public AsyncCommand GenerateCodeBarCommandAsync { get; }

        public FilterViewModel(ICodeBarService codeBarService, IExcelService excelService) : base(codeBarService, excelService)
        {
            GenerateCodeBarCommandAsync = new AsyncCommand(OnGenerateCodeBarAsyncExecuted, CanGenerateCodeBarExecute);

            LoadDataAsync = new AsyncCommand(OnLoadAsyncExecuted);
            SaveDataAsync = new AsyncCommand(OnSaveAsyncExecuted, CanSaveExecute);

            FileInfo fI = new FileInfo(_excelFile);
            LoadData(fI).Await(HandleError);
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryItems = new(await _excelService.LoadInventoryTable(table));

            DataGridCollection = CollectionViewSource.GetDefaultView(InventoryItems);
            DataGridCollection.Filter = new Predicate<object>(Filter);
        }

        private bool CanSaveExecute(object arg)
        {
            if (InventoryItems.Count > 0)
                return true;
            return false;
        }

        private async Task OnLoadAsyncExecuted()
        {
            _excelFile = SelectFile();
            FileInfo fI = new FileInfo(_excelFile); 
            await LoadData(fI);
        }

        private async Task OnSaveAsyncExecuted()
        {
            FileInfo fI = new FileInfo(_excelFile);
            await _excelService.SaveToExcel(InventoryItems, fI);

            MessageBox.Show($"Изменения сохранены!\nв инвентарную таблицу\n{_excelFile}", "Инвентарная таблица", MessageBoxButton.OK ,MessageBoxImage.Exclamation);
        }

        private bool CanGenerateCodeBarExecute(object arg)
        {
            if (InventoryItems.Count > 0)
                return true;
            return false;
        }

        private async Task OnGenerateCodeBarAsyncExecuted()
        {
            CanGenerate = false;

            if (File.Exists(_codeBarFilePath))
            {
                File.Delete(_codeBarFilePath);
            }

            Mouse.OverrideCursor = Cursors.Wait;
            await Task.Run(() =>
            {
                _codeBarService.GeneratedBarcodeHtml(InventoryItems, _codeBarFilePath);
                MessageBox.Show("Штрихкоды \nсгенерированы!");
            });
            Mouse.OverrideCursor = null;
            CanGenerate = true;
        }

        protected override void OnExitExecute(object obj)
        {
            FileInfo fI = new FileInfo(_excelFile);

            Task.Run(async ()=> {
                await _excelService.SaveToExcel(InventoryItems, fI);
                Environment.Exit(0);
            });
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
                        || (!string.IsNullOrEmpty(data.Info2) && data.Info2.Contains(_filterString))
                        || (!string.IsNullOrEmpty(data.FIO) && data.FIO.Contains(_filterString))
                        || (!string.IsNullOrEmpty(data.Room) && data.Room.Contains(_filterString));
                }
                return true;
            }
            return false;
        }
        #endregion
    }
}
