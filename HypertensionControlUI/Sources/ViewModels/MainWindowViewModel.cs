using System.Windows.Input;
using HypertensionControlUI.Models;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class MainWindowViewModel : WindowViewModelBase
    {
        #region Fields

        private readonly IViewProvider _viewProvider;
        private Patient _patient;
        private User _user;

        #endregion


        #region Auto-properties

        public ICommand LogOutCommand { get; private set; }
        public ICommand ShowSettingsCommand { get; private set; }

        #endregion


        #region Properties

        public User User
        {
            get { return _user; }
            set
            {
                if ( Equals( value, _user ) )
                    return;
                _user = value;
                OnPropertyChanged();
            }
        }

        public Patient Patient
        {
            get { return _patient; }
            set
            {
                if ( Equals( value, _patient ) )
                    return;
                _patient = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Initialization

        public MainWindowViewModel(IViewProvider viewProvider)
        {
            _viewProvider = viewProvider;
            LogOutCommand = new AsyncDelegateCommand( LogOutView );
            ShowSettingsCommand = new AsyncDelegateCommand(ShowUserView);
        }

        #endregion


        #region Non-public methods

        private void LogOutView( object o )
        {
            User = null;
            Patient = null;
            _viewProvider.NavigateToPage<MainViewModel>();
        }
        private void ShowUserView(object o)
        {
            _viewProvider.NavigateToPage<UserViewModel>( m => m.User = User);
        }
        #endregion
    }
}
