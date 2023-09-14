using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.DirectoryServices.AccountManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TLIS_Repository.Repositories
{
    public class TokenRepository : RepositoryBase<TLIuser, UserViewModel, int>, ITokenRepository
    {
        private ApplicationDbContext _context;
        IMapper _mapper;
        private byte[] key = new byte[16]; // 128-bit key
        private byte[] iv = new byte[16]; // 128-bit IV
        public TokenRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public UserViewModel Authenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {

            UserViewModel user = null;
            ErrorMessage = null;
            //string domain = "Ids.com";
            // using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind, null, null))
            // {
            //  UserPrincipal principal = new UserPrincipal(context);

            TLIuser User = _context.TLIuser.FirstOrDefault(x => x.UserName == login.UserName && x.Active == true && x.Deleted == false);
            // principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login.UserName);

            // GroupPrincipal group = GroupPrincipal.FindByIdentity(context, domainGroup);

            //add check with TLI group 


            if (User == null)
            {
                ErrorMessage = "This account is blocked or not found ";
                return null;
            }



            //if (User.UserType.Equals(1) && principal.IsMemberOf(group))
            //{
            //    user = _mapper.Map<UserViewModel>(User);
            //}
            if (User.UserType.Equals(2))
            {

                if (string.IsNullOrEmpty(login.Password))
                {
                    User = null;

                    ErrorMessage = "The Password coudn't be empty.";
                }


                else
                {
                    byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                    // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                    //string password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    //    password: login.Password,
                    //    salt: salt,
                    //    prf: KeyDerivationPrf.HMACSHA256,
                    //    iterationCount: 100000,
                    //    numBytesRequested: 256 / 8));

                    User.Password = DecryptPassword(User.Password);
                    bool verified = (User.Password == login.Password);
                    if (verified.Equals(false))
                    {
                        var Attempt = _context.TLIuser.Where(x => x.UserName == login.UserName).Select(x => x.Domain).FirstOrDefault();
                        if (Attempt == null || Attempt == "")
                        {
                            Attempt = "0";

                        }
                        if (Attempt == "3")
                        {


                            User.Active = false;
                            User.Domain = null;
                            _context.Entry(User).State = EntityState.Modified;
                            _context.SaveChanges();
                            ErrorMessage = "You have entered the wrong password 3 times,the account is blocked ,Please contact the Administrator";
                            User = null;
                        }
                        else
                        {
                            Attempt = (Convert.ToInt32(Attempt) + 1).ToString();
                            User.Domain = Attempt;
                            _context.Entry(User).State = EntityState.Modified;
                            _context.SaveChanges();
                            ErrorMessage = "The login failed";
                            User = null;
                        }

                        return null;

                    }
                    else if (User.ChangedPasswordDate != null)
                    {
                        DateTime date = (DateTime)User.ChangedPasswordDate;
                        if (date.AddDays(90) < DateTime.Now)
                        {
                            ErrorMessage = "You have to change your password, your password is out of date";
                            User = null;
                        }
                    }
                    // }
                    user = _mapper.Map<UserViewModel>(User);
                }
                return user;

            }

            else
            {
                ErrorMessage = "User invalid.";
                return null;
            }

        }
    

        // Internal syriatel token

        public UserViewModel InternalAuthenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {

            UserViewModel user = null;
            ErrorMessage = null;
            //string domain = "Ids.com";
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind, null, null))
            {
                UserPrincipal principal = new UserPrincipal(context);
                // string chr = "\\";
                // int index = login.UserName.LastIndexOf(chr);
                //if (index != -1)
                //{
                //    login.UserName = login.UserName.Substring(index+1);
                //}
                string UserWithouDomain = login.UserName;
                TLIuser User = _context.TLIuser.FirstOrDefault(x => x.UserName == login.UserName && x.Active == true && x.Deleted == false && x.UserType == 1);
                if (User == null)
                {
                    ErrorMessage = "This account is blocked or not found    " + login.UserName;
                    return null;
                }

                if (IsPasswordValid(login.UserName, login.Password) == true)
                {

                    principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login.UserName);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, domainGroup);

                    //add check with TLI group 

                    if (User.UserType.Equals(1) && principal.IsMemberOf(group))
                    {
                        user = _mapper.Map<UserViewModel>(User);
                    }

                    return user;


                }

                else
                {
                    ErrorMessage = "This account is blocked or not found    " + login.UserName;

                    return null;
                }
            }
        }

        private bool IsPasswordValid(string username, string password)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                return context.ValidateCredentials(username, password);
            }
        }


        private string CryptPassword(string password)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(password);
            byte[] ciphertext;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
                }
            }

            return Convert.ToBase64String(ciphertext);


        }


        private string DecryptPassword(string CryptPassword)
        {
            try
            {

                byte[] ciphertext = Convert.FromBase64String(CryptPassword);
                byte[] plaintext;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
                    }
                }

                return Encoding.UTF8.GetString(plaintext);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }




    }
}
