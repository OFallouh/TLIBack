using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
            TLIuser User = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName == login.UserName && !x.Deleted && x.Active);
            if (User != null && User.UserType == 1)
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind, null, null))
                {
                    UserPrincipal principal = new UserPrincipal(context);

                    string UserWithouDomain = login.UserName;
                    if (IsPasswordValid(login.UserName, login.Password) == true)
                    {

                        principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, login.UserName);
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(context, domainGroup);

                        //add check with TLI group 

                        if (User.UserType.Equals(1) && principal.IsMemberOf(group))
                        {
                            user = _mapper.Map<UserViewModel>(User);

                            if (user != null)
                            {
                                var tokenString = BuildToken(user, secretKey);
                                response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }

                    }
                    else
                    {
                        response = new Response<string>(true, null, null, $"This account is blocked or not found + {login.UserName}", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                }
            }
            else if (User != null && User.UserType == 2)
            {
                if (string.IsNullOrEmpty(login.Password))
                {
                    response = new Response<string>(true, null, null, "The Password coudn't be empty", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                }
                //User.Password = Decrypt(User.Password);
                login.Password = Decrypt(login.Password, login.SecretKey,login.IV);
                bool verified = (User.Password == login.Password);
                if (verified.Equals(false))
                {
                    string Attempt = _unitOfWork.UserRepository.GetWhere(x => x.UserName == login.UserName).Select(x => x.Domain).FirstOrDefault();
                    if (Attempt == null || Attempt == "")
                    {
                        Attempt = "0";

                    }
                    if (Attempt == "3")
                    {


                        User.Active = false;
                        User.Domain = null;
                        _unitOfWork.UserRepository.Update(User);
                        _unitOfWork.SaveChanges();
                        response = new Response<string>(true, null, null, "You have entered the wrong password 3 times,the account is blocked ,Please contact the Administrator", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                    else
                    {
                        Attempt = (Convert.ToInt32(Attempt) + 1).ToString();
                        User.Domain = Attempt;
                        _unitOfWork.UserRepository.Update(User);
                        _unitOfWork.SaveChanges();
                        response = new Response<string>(true, null, null, "The login failed", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }

                    return null;

                }
                if (User.ChangedPasswordDate != null)
                {
                    DateTime date = (DateTime)User.ChangedPasswordDate;
                    if (date.AddDays(90) < DateTime.Now)
                    {
                        response = new Response<string>(true, null, null, "You have to change your password, your password is out of date", (int)Helpers.Constants.ApiReturnCode.uncompleted);
                    }
                }
                if(verified.Equals(true))
                {
                    user = _mapper.Map<UserViewModel>(User);

                    if (user != null)
                    {
                        var tokenString = BuildToken(user, secretKey);
                        response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                }
            }
            else
            {
                response = new Response<string>(true, null, null, "The User Is Not Found", (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }
            return response;
        }
        private bool IsPasswordValid(string username, string password)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                return context.ValidateCredentials(username, password);
            }
        }
        public string Decrypt(string encryptedText, byte[] keyBytes, byte[] ivBytes)
        {
            //string key = "9443a09ae2e433750868beaeec0fd681";
            // string IV = "8e67d9852e93d17f";
            // Convert the secret key to bytes
           // byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Convert the IV to bytes
            //byte[] ivBytes = Encoding.UTF8.GetBytes(IV);

            // Convert the Base64-encoded encrypted text to bytes
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            // Create an AES decryptor
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;
                aesAlg.Mode = CipherMode.CFB;
                aesAlg.Padding = PaddingMode.Zeros;

                // Create a decryptor to perform the decryption
                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    // Create a MemoryStream to write the decrypted data to
                    using (MemoryStream msDecrypt = new MemoryStream())
                    {
                        // Create a CryptoStream to perform the decryption
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            // Write the encrypted bytes to the CryptoStream
                            csDecrypt.Write(encryptedBytes, 0, encryptedBytes.Length);
                        }

                        // Get the decrypted bytes from the MemoryStream
                        byte[] decryptedBytes = msDecrypt.ToArray();

                        // Convert the decrypted bytes to a UTF-8 string
                        string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                        return decryptedText;
                    }
                }
            }
        }
        //public static string Decrypt(string encryptedText)
        //{
        //    string key = "72d025d8716aa7fa971b40aec4c969d9528f8f35c7b2f76fcf67e1087dc199e5";
        //    string IV = "bde174535a9ae66bbc903f9992a68ecb";
        //    // Convert the static key string to a byte array
        //    byte[] keyBytes = Convert.FromHexString(key);

        //    // Convert the Base64-encoded ciphertext back to a byte array
        //    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        //    // Extract the salt from the beginning of the encrypted data
        //    byte[] IVS = Convert.FromHexString(IV);

        //    // Create a new Rijndael AES cipher with a 256-bit key
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.KeySize = 256;
        //        aesAlg.BlockSize = 128;
        //        aesAlg.Mode = CipherMode.CFB; // Use the same mode as in CryptoJS
        //        aesAlg.Padding = PaddingMode.Zeros; // Use the same padding as in CryptoJS

        //        // Set the static key
        //        aesAlg.Key = keyBytes;

        //        // Set the extracted salt as the IV
        //        aesAlg.IV = IVS;

        //        // Create a memory stream to write the decrypted data to
        //        using (MemoryStream msDecrypt = new MemoryStream())
        //        {
        //            // Create a crypto stream to perform decryption
        //            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
        //            {
        //                // Write the encrypted data (excluding the salt) to the crypto stream
        //                msDecrypt.Write(encryptedBytes, 16, encryptedBytes.Length - 16);
        //            }

        //            // Get the decrypted bytes
        //            byte[] decryptedBytes = msDecrypt.ToArray();

        //            // Convert the decrypted bytes to a UTF-8 string (in this case, the password)
        //            string decryptedPassword = Encoding.UTF8.GetString(decryptedBytes);
        //            return decryptedPassword;
        //        }
        //    }
        //}

        //public static byte[] HexStringToByteArray(string hex)
        //{
        //    int numberChars = hex.Length;
        //    byte[] bytes = new byte[numberChars / 2];
        //    for (int i = 0; i < numberChars; i += 2)
        //    {
        //        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        //    }
        //    return bytes;
        //}


    }

}

