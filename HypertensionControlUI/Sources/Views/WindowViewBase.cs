using System.Windows;
using HypertensionControlUI.ViewModels;

namespace HypertensionControlUI.Views
{
    public abstract class WindowViewBase<TViewModel> : Window where TViewModel : class, IWindowViewModel
    {
        #region Properties

        public TViewModel ViewModel
        {
            get { return DataContext as TViewModel; }
        }

        #endregion


        #region Initialization

        public WindowViewBase( TViewModel windowViewModel )
        {
            DataContext = windowViewModel;
        }

        #endregion
    }
}
