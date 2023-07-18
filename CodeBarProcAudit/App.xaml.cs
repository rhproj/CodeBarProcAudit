using CodeBarProcAudit.Setups;
using CodeBarProcAudit.Views;
using System.Windows;

namespace CodeBarProcAudit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var viewModel = ViewModelServiceSetter.SetUpViewModel();

            Application.Current.MainWindow = new FCWindow(viewModel);
            Application.Current.MainWindow.Show();
        }
    }
}
