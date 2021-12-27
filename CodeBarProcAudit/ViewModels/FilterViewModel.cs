using CodeBarProcAudit.Extensions;
using CodeBarProcAudit.Model;
using CodeBarProcAudit.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CodeBarProcAudit.ViewModels
{
    internal class FilterViewModel: MainViewModel
    {
        private List<Item> InventoryItems;
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

        public FilterViewModel() : base()
        {
            LoadData(tableFileInfo).Await(HandleError);
        }

        private async Task LoadData(FileInfo table)
        {
            InventoryItems = new(await EPPlusService.LoadInventoryTable(table));

            DataGridCollection = CollectionViewSource.GetDefaultView(InventoryItems);
            DataGridCollection.Filter = new Predicate<object>(Filter);
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
