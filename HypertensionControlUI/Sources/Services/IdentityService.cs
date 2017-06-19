using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class IdentityService
    {
        #region Fields

        private readonly DbContextFactory _dbFactory;

        #endregion


        #region Auto-properties

        public User CurrentUser { get; private set; }

        #endregion


        #region Initialization

        public IdentityService( DbContextFactory dbFactory )
        {
            _dbFactory = dbFactory;
        }

        #endregion


        #region Public methods

        public void Login( string login, string password )
        {
            using ( var db = _dbFactory.GetDbContext() )
            {
                var passwordHash = HashUtils.GetStringHash( password );
                CurrentUser = db.Users
                                .Include( "Job" )
                                .FirstOrDefault( u => u.Login == login && u.PasswordHash == passwordHash );

                if (CurrentUser == null)
                    throw new AuthenticationException("Invalid user credentials");
            }
        }

        #endregion
    }

    public static class HashUtils
    {
        #region Public methods

        public static string GetStringHash( string str )
        {
            using ( var hash = MD5.Create() )
                return Convert.ToBase64String( hash.ComputeHash( Encoding.UTF8.GetBytes( str ) ) );
        }

        #endregion
    }
}
