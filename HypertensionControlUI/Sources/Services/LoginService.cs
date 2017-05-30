using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HypertensionControlUI.CompositionRoot;
using HypertensionControlUI.Models;

namespace HypertensionControlUI.Services
{
    public class LoginService
    {
        
        private readonly DbContextFactory _dbFactory;

        public LoginService( DbContextFactory dbFactory )
        {
            _dbFactory = dbFactory;
        }

        public User Login(string login, string password)
        {
            using ( var db = _dbFactory.GetDbContext() )
            {
//                var passwordHash = HashUtils.GetStringHash(password);
//                return db.Users.Include( "Job" ).FirstOrDefault( u => u.Login == login && u.PasswordHash == passwordHash );
                return new User();
            }
        }
    }

    public static class HashUtils
    {
        public static string GetStringHash(string str)
        {
            using (var hash = MD5.Create())
            {
                return Convert.ToBase64String( hash.ComputeHash( Encoding.UTF8.GetBytes( str ) ) );
            }
        }
    }
}