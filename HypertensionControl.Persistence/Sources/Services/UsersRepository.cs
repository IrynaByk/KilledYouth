using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using HypertensionControl.Domain.Models;
using HypertensionControl.Persistence.Interfaces;

namespace HypertensionControl.Persistence.Services
{
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

    public class UsersRepository : IUsersRepository
    {
        #region Fields

        private readonly SqliteDbContext _dbContext;
        private readonly IMapper _mapper;

        #endregion


        #region Initialization

        public UsersRepository( SqliteDbContext dbContext, IMapper mapper )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #endregion


        #region Public methods

        public User FindUserByLoginAndPassword( string login, string password )
        {
            var passwordHash = HashUtils.GetStringHash( password );
            var userEntity = _dbContext.Users.FirstOrDefault( u => u.Login == login && u.PasswordHash == passwordHash );
            return _mapper.Map<User>( userEntity );
        }

        #endregion
    }
}
