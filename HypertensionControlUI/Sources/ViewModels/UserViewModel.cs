using System.Collections.Generic;
using System.Linq;
using HypertensionControl.Domain.Models;
using HypertensionControlUI.Interfaces;

namespace HypertensionControlUI.ViewModels
{
    public class UserViewModel : PageViewModelBase
    {
        #region Fields

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        #endregion


        #region Auto-properties

        #endregion


        #region Properties

        public User User { get; set; }

        #endregion


        #region Initialization

        public UserViewModel( IUnitOfWorkFactory unitOfWorkFactory )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion
    }
}
