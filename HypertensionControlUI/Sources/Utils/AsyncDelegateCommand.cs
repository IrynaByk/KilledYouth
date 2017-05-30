using System;
using System.Windows.Input;

namespace HypertensionControlUI.Utils
{
    public class AsyncDelegateCommand : ICommand
    {
        #region Fields

        private readonly Func<object, bool> canExecute;
        private Action<object> execute;

        #endregion


        #region Events and invocation

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    

        #endregion


        #region Initialization

        public AsyncDelegateCommand( Action<object> execute, Func<object, bool> canExecute = null )
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion


        #region Public methods

        public bool CanExecute( object parameter )
        {
            if ( canExecute == null )
                return true;
            return canExecute( parameter );
        }

        public void Execute( object parameter )
        {
            if ( execute != null )
            {
                execute( parameter );
            }
        }

        #endregion
    }

    public class AsyncDelegateCommand<T> : ICommand
    {
        #region Fields

        private readonly Func<T, bool> canExecute;
        private Action<T> execute;

        #endregion


        #region Events and invocation

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    

        #endregion


        #region Initialization

        public AsyncDelegateCommand( Action<T> execute, Func<T, bool> canExecute = null )
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion


        #region Public methods

        public bool CanExecute( object parameter )
        {
            if ( canExecute == null )
                return true;
            if (!(parameter is T typedParameter))
                return false;
            return canExecute( typedParameter );
        }

        public void Execute( object parameter )
        {
            if (parameter is T typedParameter)
                execute?.Invoke( typedParameter );
        }

        #endregion
    }
}
