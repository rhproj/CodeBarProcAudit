using CodeBarProcAudit.ViewModels;
using System;
using System.Windows;

namespace CodeBarProcAudit.Views
{
    /// <summary>
    /// Interaction logic for FCWindow.xaml
    /// </summary>
    public partial class FCWindow : Window
    {
        public FCWindow(FilterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            btnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            btnMax.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            btnClear.Click += (s, e) => { SearchBox.Text = String.Empty; };
        }
    }
}
