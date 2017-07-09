using System.Security.Authentication;
using HypertensionControl.Domain.Models;
using HypertensionControlUI.Interfaces;

namespace HypertensionControlUI.Services
{
    public class IdentityService
    {
        #region Fields

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        #endregion


        #region Auto-properties

        public User CurrentUser { get; private set; }

        #endregion


        #region Initialization

        public IdentityService( IUnitOfWorkFactory unitOfWorkFactory )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion


        #region Public methods

        public void Login( string login, string password )
        {
            using ( var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork() )
            {
                CurrentUser = unitOfWork.UsersRepository.FindUserByLoginAndPassword( login, password );

                if ( CurrentUser == null )
                    throw new AuthenticationException( "Invalid user credentials" );
            }
        }

        #endregion
    }
}
