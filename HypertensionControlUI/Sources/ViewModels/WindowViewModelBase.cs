using System.ComponentModel;
using System.Runtime.CompilerServices;
using HypertensionControlUI.Annotations;

namespace HypertensionControlUI.ViewModels
{
    public class WindowViewModelBase : IWindowViewModel, INotifyPropertyChanged
    {
        #region Events and invocation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
