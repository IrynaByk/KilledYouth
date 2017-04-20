using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HypertensionControlUI.Annotations;
using HypertensionControlUI.Services;
using HypertensionControlUI.Utils;

namespace HypertensionControlUI.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, IPageViewModel
    {
        #region Fields

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly LoginService _loginService;
        private bool _loginFailed;
        private IViewProvider _viewProvider;

        #endregion


        #region Auto-properties

        public string Login { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; private set; }

        #endregion


        #region Properties

        public bool LoginFailed
        {
            get { return _loginFailed; }
            set
            {
                if ( value == _loginFailed )
                    return;
                _loginFailed = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Events and invocation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            if ( PropertyChanged != null )
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion


        #region Initialization

        public LoginViewModel(MainWindowViewModel mainWindowViewModel,LoginService loginService, IViewProvider viewProvider)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _loginService = loginService;
            _viewProvider = viewProvider;
            LoginCommand = new AsyncDelegateCommand( LoginCommandHandler, LoginCommandCanExecute );
        }

        #endregion


        #region Non-public methods

        private bool LoginCommandCanExecute( object o )
        {
            return !string.IsNullOrEmpty( Login ) && !string.IsNullOrEmpty( Password );
        }

        private void LoginCommandHandler( object o )
        {
            var user = _loginService.Login( Login, Password );
            LoginFailed = user == null;
            if ( !LoginFailed )
            {
                _mainWindowViewModel.User = user;
                _viewProvider.NavigateToPage<PatientsViewModel>( );
            }
        }

        #endregion
    }
}
