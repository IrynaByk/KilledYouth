using System.ComponentModel;
using System.Runtime.CompilerServices;
using HypertensionControlUI.Annotations;

namespace HypertensionControlUI.ViewModels
{
    public class PageViewModelBase : IPageViewModel, INotifyPropertyChanged
    {
        #region Events and invocation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            var handler = PropertyChanged;
            if ( handler != null )
                handler( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion
    }
}
