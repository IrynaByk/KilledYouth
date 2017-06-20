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

        #endregion
    }
}
