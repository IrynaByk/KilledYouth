using System.Windows.Controls;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views
{
    public abstract class PageViewBase<TViewModel> : Page where TViewModel : class, IPageViewModel
    {
        #region Properties

        public virtual TViewModel ViewModel
        {
            get { return DataContext as TViewModel; }
            set { DataContext = value; }
        }

        #endregion
    }
}
