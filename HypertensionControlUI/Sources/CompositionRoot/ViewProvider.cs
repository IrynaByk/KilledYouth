using System;
using HypertensionControlUI.ViewModels;
using HypertensionControlUI.Views;
using SimpleInjector;

namespace HypertensionControlUI.CompositionRoot
{
    public class ViewProvider : IViewProvider
    {
        #region Fields

        private readonly Container _container;

        #endregion


        #region Initialization

        public ViewProvider( Container container )
        {
            _container = container;
        }

        #endregion


        #region Public methods

        public void NavigateToPage<TViewModel>( Action<TViewModel> initializer = null )
            where TViewModel : class, IPageViewModel
        {
            var view = CreatePage<TViewModel>();

            initializer?.Invoke(view.ViewModel);

            _container.GetInstance<MainWindow>().MainWindowFrame.Navigate(view);
        }

        public IViewResult<TViewModel> ShowDialog<TViewModel>( Action<TViewModel> initializer = null )
            where TViewModel : class, IWindowViewModel
        {
            var view = CreateWindow<TViewModel>();

            initializer?.Invoke(view.ViewModel);

            view.Owner = _container.GetInstance<MainWindow>();

            var dialogResult = view.ShowDialog();

            return new ViewResult<TViewModel>( dialogResult, view.ViewModel );
        }

        #endregion


        #region Non-public methods

        private WindowViewBase<TViewModel> CreateWindow<TViewModel>() where TViewModel : class, IWindowViewModel
        {
            return (WindowViewBase<TViewModel>) _container.GetInstance( typeof (WindowViewBase<TViewModel>) );
        }

        private PageViewBase<TViewModel> CreatePage<TViewModel>( TViewModel viewModel = null ) where TViewModel : class, IPageViewModel
        {
            if ( viewModel == null )
                viewModel = _container.GetInstance<TViewModel>();
            var view = (PageViewBase<TViewModel>) _container.GetInstance( typeof (PageViewBase<TViewModel>) );
            view.ViewModel = viewModel;
            return view;
        }

        #endregion
    }
}
