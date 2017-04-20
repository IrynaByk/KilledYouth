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

        public bool? Result { get; private set; }
        public TViewModel ViewModel { get; private set; }

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
