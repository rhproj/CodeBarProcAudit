using CodeBarProcAudit.Services;
using CodeBarProcAudit.ViewModels;

namespace CodeBarProcAudit.Setups
{
    internal static class ViewModelServiceSetter
    {
        internal static FilterViewModel SetUpViewModel()
        {
            ICodeBarService codeBarService = new CodeBarService();
            IExcelService epPlusService = new EPPlusService();

            var viewModel = new FilterViewModel(codeBarService, epPlusService);
            return viewModel;
        }
    }
}
