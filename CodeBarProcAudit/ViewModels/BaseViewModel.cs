using CodeBarProcAudit.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeBarProcAudit.ViewModels
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected string _folderPath;
        protected string _cBarFilePath;
        protected string _excelFile;
        protected FileInfo tableFileInfo;

        public RelayCommand Exit { get; }

        public BaseViewModel()
        {
            SetFilePaths();

            Exit = new RelayCommand(OnExitExecute);

            tableFileInfo = GetExcelFile(_folderPath);
        }

        protected virtual void OnExitExecute(object obj)
        {
            Environment.Exit(0);
        }

        protected void SetFilePaths()
        {
            _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + (string)App.Current.Resources["InvFolder"];
            _cBarFilePath = $"{_folderPath}\\Штрихкоды.html";
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
