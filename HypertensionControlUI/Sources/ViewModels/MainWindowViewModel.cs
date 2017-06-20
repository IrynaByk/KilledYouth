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

        public ICommand LogOutCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        #endregion


        #region Properties

        public Patient Patient
        {
            get => _patient;
            set
            {
                if ( Equals( value, _patient ) )
                    return;
                _patient = value;
                OnPropertyChanged();
            }
        }

        public User User
        {
            get => _user;
            set
            {
                if ( Equals( value, _user ) )
                    return;
                _user = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Initialization

        public MainWindowViewModel( IViewProvider viewProvider )
        {
            _viewProvider = viewProvider;
            LogOutCommand = new AsyncDelegateCommand( LogOutView );
            ShowSettingsCommand = new AsyncDelegateCommand( ShowUserView );
        }

        #endregion


        #region Non-public methods

        private void LogOutView( object o )
        {
            User = null;
            Patient = null;

            _viewProvider.NavigateToPage<MainViewModel>();
        }

        private void ShowUserView( object o )
        {
            _viewProvider.NavigateToPage<UserViewModel>( m => m.User = User );
        }

        #endregion
    }
}
