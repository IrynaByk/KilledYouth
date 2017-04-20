using System;

namespace HypertensionControlUI.ViewModels
{
    public interface IViewProvider
    {
        #region Public methods

        IViewResult<TViewModel> ShowDialog<TViewModel>( Action<TViewModel> initializer = null )
            where TViewModel : class, IWindowViewModel;

        #endregion


        void NavigateToPage<TViewModel>( Action<TViewModel> initializer = null )
            where TViewModel : class, IPageViewModel;
    }
}
