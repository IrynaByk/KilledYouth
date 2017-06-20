namespace HypertensionControlUI.ViewModels
{
    public interface IViewResult<out TViewModel> where TViewModel : class, IWindowViewModel
    {
        #region Properties

        bool? Result { get; }
        TViewModel ViewModel { get; }

        #endregion
    }

    internal class ViewResult<TViewModel> : IViewResult<TViewModel> where TViewModel : class, IWindowViewModel
    {
        #region Auto-properties

        public bool? Result { get; }
        public TViewModel ViewModel { get; }

        #endregion


        #region Initialization

        public ViewResult( bool? result, TViewModel viewModel )
        {
            Result = result;
            ViewModel = viewModel;
        }

        #endregion
    }
}
