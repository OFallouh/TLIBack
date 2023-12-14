using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Transactions;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class TokenService : ITokenService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _config;
        private IMapper _mapper;

        public TokenService(IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _mapper = mapper;
        }
        //Function to authenticate login
        public UserViewModel Authenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {
            return _unitOfWork.TokenRepository.Authenticate(login, out ErrorMessage, domain, domainGroup);
        }
        public UserViewModel InternalAuthenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {
            return _unitOfWork.TokenRepository.InternalAuthenticate(login, out ErrorMessage, domain, domainGroup);
        }
        //Function build token depened on user data
        //Function return token
        public string BuildToken(UserViewModel user, string secretKey)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("https://localhost:44311/",
              "https://localhost:44311/",
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //Function take 1 parameter
        //first authenticate the user
        //if user is authenticate build token for user and return the token
        //else give error message The login failed
        public Response<string> CreateToken(LoginViewModel login, string secretKey, string domain, string domainGroup)
        {
            Response<string> response = null;
            string ErrorMessage;
            var user = Authenticate(login, out ErrorMessage, domain, domainGroup);

            if (user != null)
            {
                var tokenString = BuildToken(user, secretKey);
                response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            else
            {
                response = new Response<string>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }

            return response;
        }

        public Response<string> CreateInternalToken(LoginViewModel login, string secretKey, string domain, string domainGroup)
        {
            Response<string> response = null;
            string ErrorMessage;
            var user = InternalAuthenticate(login, out ErrorMessage, domain, domainGroup);

            if (user != null)
            {
                var tokenString = BuildToken(user, secretKey);
                response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            else
            {
                response = new Response<string>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }

            return response;
        }
      
        public Response<string> Login(LoginViewModel login, string secretKey, string domain, string domainGroup)
        {
            Response<string> response = null;
            UserViewModel user = null;
            login.Wedcto = Decrypt(login.Wedcto);
            login.Yuqrgh= Decrypt(login.Yuqrgh);
            login.beresd = Decrypt(login.beresd);
            int Trycount= Convert.ToInt32(login.Yuqrgh);
            TLIuser User = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName.ToLower() == login.Wedcto.ToLower() && !x.Deleted && x.Active);
            if (User != null && User.UserType == 1)
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind, null, null))
                {
                    UserPrincipal principal = new UserPrincipal(context);
                    var usernotfound = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName.ToLower() == login.Wedcto.ToLower());
                    if(usernotfound == null)
                    {
                        return response = new Response<string>(false, null, null, $"This User Is Not Found In TLI + {login.Wedcto}", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                    else if(usernotfound!=null && usernotfound.Active==false)
                    {
                        return response = new Response<string>(false, null, null, $"This Account Is Blocked In TLI + {login.Wedcto}", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                    else if (IsPasswordValid(login.Wedcto.ToLower(), login.beresd) == true)
                    {

                        principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login.Wedcto.ToLower());
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(context, domainGroup);

                        //add check with TLI group 

                        if (User.UserType.Equals(1) && principal.IsMemberOf(group))
                        {
                            user = _mapper.Map<UserViewModel>(User);

                            if (user != null)
                            {
                                var tokenString = BuildToken(user, secretKey);
                                return response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }

                    }
                    else if (Trycount == 3)
                    {
                        User.Active = false;
                        _unitOfWork.UserRepository.Update(User);
                        _unitOfWork.SaveChanges();
                        return response = new Response<string>(false, null, null, "You have entered the wrong password 3 times,the account is blocked ,Please contact the Administrator", (int)Helpers.Constants.ApiReturnCode.uncompleted);
 
                    }
                    else
                    {
                      return  response = new Response<string>(false, null, null, "This username or password is not correct", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                }
            }
            else if (User != null && User.UserType == 2)
            {
                if (string.IsNullOrEmpty(login.beresd))
                {
                   return response = new Response<string>(false, null, null, "The Password coudn't be empty", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                }
                var OldPass = Decrypt(User.Password);
                bool verified = (OldPass == login.beresd);
                if (verified.Equals(false))
                {

                    if (Trycount == 3)
                    {
                        User.Active = false;
                        _unitOfWork.UserRepository.Update(User);
                        _unitOfWork.SaveChanges();
                       return response = new Response<string>(false, null, null, "You have entered the wrong password 3 times,the account is blocked ,Please contact the Administrator", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                    else
                    {
                        return response = new Response<string>(false, null, null, "Your Password Is Not Correct", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }

                }
                if (User.ChangedPasswordDate != null)
                {
                    DateTime date = (DateTime)User.ChangedPasswordDate;
                    if (date.AddDays(90) < DateTime.Now)
                    {
                        return response = new Response<string>(false, null, null, "You have to change your password, your password is out of date", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                }
                if (verified.Equals(true))
                {
                    user = _mapper.Map<UserViewModel>(User);

                    if (user != null)
                    {
                        var tokenString = BuildToken(user, secretKey);
                        return response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                }
            }
            else
            {
                return response = new Response<string>(false, null, null, "The User Is Not Found", (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }
            return response;
        }
        //string key = "9443a09ae2e433750868beaeec0fd681";
        //public static string Encrypt(string plainText, string key)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        string iv = "abcdefghijklmnopq";
        //        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        //        aesAlg.IV = Encoding.UTF8.GetBytes(iv);
        //        aesAlg.Mode = CipherMode.ECB;
        //        aesAlg.Padding = PaddingMode.PKCS7;

        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    swEncrypt.Write(plainText);
        //                }
        //            }

        //            return Convert.ToBase64String(msEncrypt.ToArray());
        //        }
        //    }
        //}
      
        public static string Decrypt(string encryptedText)
            {
            using (Aes aesAlg = Aes.Create())
            {
                string Key = "9443a09ae2e433750868beaeec0fd681";
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.Mode = CipherMode.ECB; // Use ECB mode (no IV)
                aesAlg.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

        }

        private bool IsPasswordValid(string username, string password)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                return context.ValidateCredentials(username.ToLower(), password);
            }
        }

        //public static string Encrypt(string plainText)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        string Key = "9443a09ae2e433750868beaeec0fd681";
        //        aesAlg.Key = Encoding.UTF8.GetBytes(Key);
        //        aesAlg.Mode = CipherMode.ECB;
        //        aesAlg.Padding = PaddingMode.PKCS7;

        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    swEncrypt.Write(plainText);
        //                }
        //            }

        //            return Convert.ToBase64String(msEncrypt.ToArray());
        //        }
        //    }
        //}

        //public static string Decrypt(string encryptedText, string key)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        //        aesAlg.Mode = CipherMode.ECB; // Use ECB mode (no IV)
        //        aesAlg.Padding = PaddingMode.PKCS7;

        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {
        //                    return srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }

        //}

    }
}

