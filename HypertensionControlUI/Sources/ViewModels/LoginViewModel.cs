using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Windows.Input;
using HypertensionControlUI.Annotations;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, IPageViewModel
    {
        #region Fields

        private readonly IdentityService _identityService;

        private readonly MainWindowViewModel _mainWindowViewModel;
        private bool _loginFailed;
        private readonly IViewProvider _viewProvider;

        #endregion


        #region Auto-properties

        public string Login { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; }

        #endregion


        #region Properties

        public bool LoginFailed
        {
            get => _loginFailed;
            set
            {
                if ( value == _loginFailed )
                    return;
                _loginFailed = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Commands

        private void LoginCommandHandler( object o )
        {
            try
            {
                LoginFailed = false;
                _identityService.Login( Login, Password );
                _mainWindowViewModel.User = _identityService.CurrentUser;
                _viewProvider.NavigateToPage<PatientsViewModel>();
            }
            catch ( AuthenticationException )
            {
                LoginFailed = true;
            }
        }

        #endregion


        #region Events and invocation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion


        #region Initialization

        public LoginViewModel( MainWindowViewModel mainWindowViewModel, IdentityService identityService, IViewProvider viewProvider )
        {
            _mainWindowViewModel = mainWindowViewModel;
            _identityService = identityService;
            _viewProvider = viewProvider;
            LoginCommand = new AsyncDelegateCommand( LoginCommandHandler, LoginCommandCanExecute );
        }

        #endregion


        #region Non-public methods

        private bool LoginCommandCanExecute( object o )
        {
            return !string.IsNullOrEmpty( Login ) && !string.IsNullOrEmpty( Password );
        }

        #endregion
    }
}
