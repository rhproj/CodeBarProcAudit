using CodeBarProcAudit.Commands;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CodeBarProcAudit.ViewModels
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected string _cBarFilePath;
        protected string _excelFile;

        public RelayCommand Exit { get; }

        public BaseViewModel()
        {
            SetFilePaths();

            Exit = new RelayCommand(OnExitExecute);
        }

        protected virtual void OnExitExecute(object obj)
        {
            Environment.Exit(0);
        }

        protected void SetFilePaths()
        {
            string _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];
            _cBarFilePath = $"{_folderPath}\\Штрихкоды.html";

            _excelFile = SetExcelFile(_folderPath);
        }

        string SetExcelFile(string folderPath)
        {
            _excelFile = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith(".xlsx") || s.EndsWith(".xls")).Select(f => f).FirstOrDefault();

            if (string.IsNullOrEmpty(_excelFile))
            {
                MessageBox.Show("Отсутствует инвентарная таблица");

                Environment.Exit(0);
            }

            return _excelFile;
        }

        FileInfo GetExcelFile(string folderPath)
        {
            _excelFile = Directory.EnumerateFiles(folderPath).Where(s => s.EndsWith(".xlsx") || s.EndsWith(".xls")).Select(f => f).FirstOrDefault();

            if (string.IsNullOrEmpty(_excelFile))
            {
                MessageBox.Show("Отсутствует инвентарная таблица");

                Environment.Exit(0);
            }

            FileInfo fileInf = new FileInfo(_excelFile);
            return fileInf;
        }

        protected string SelectFile()
        {
            string path = _excelFile;
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Файлы Таблиц(*.xlsx)|*.xlsx|XLS файлы (*.xls)|*.xls|CSV Файлы (*.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                path = dlg.FileName;
            }          
            return path;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
