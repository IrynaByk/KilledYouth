using System.Windows.Input;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class MainViewModel : IPageViewModel
    {
        #region Fields

        private readonly IViewProvider _viewProvider;

        #endregion


        #region Auto-properties

        public ICommand ShowLoginCommand { get; }

        #endregion


        #region Initialization

        public MainViewModel( IViewProvider viewProvider )
        {
            _viewProvider = viewProvider;

            ShowLoginCommand = new AsyncDelegateCommand( ShowLoginView );
        }

        #endregion


        #region Non-public methods

        private void ShowLoginView( object o )
        {
            _viewProvider.NavigateToPage<LoginViewModel>();
        }

        #endregion
    }
}
