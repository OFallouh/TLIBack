using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using System;
using TLIS_DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
//using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Org.BouncyCastle.Crypto.Parameters;
using Microsoft.IdentityModel.Tokens;
using Castle.Core.Internal;
using TLIS_DAL.Models;
using Microsoft.EntityFrameworkCore.Internal;
using NuGet.Common;
using System.IO;

namespace TLIS_API.Middleware.ActionFilters
{
    public class ExternalSystemFilter: IAuthorizationFilter, IActionFilter
    {
        private readonly TLIS_DAL.ApplicationDbContext db;
        private byte[] key = new byte[16]; // 128-bit key
        private byte[] iv = new byte[16]; // 128-bit IV
        public ExternalSystemFilter(ApplicationDbContext _ApplicationDbContext)
        {
            db= _ApplicationDbContext;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader) && authHeader.ToLower().StartsWith("bearer "))
            {
                return;
            }
            var clientIPAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();

            if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                clientIPAddress = forwardedFor.ToString().Split(", ")[0];
            }
            // Extract the Authorization header from the request

            // Check if the Authorization header is present and is of type Basic
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
            {
                // Decode the Authorization header using base64
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                byte[] decodedBytes = Convert.FromBase64String(encodedUsernamePassword);
                string decodedUsernamePassword = Encoding.UTF8.GetString(decodedBytes);

                // Split the decoded string on the : character to separate the username and password
                string[] parts = decodedUsernamePassword.Split(':', 2);
                string username = parts[0];
                string password = parts[1];

                // Validate the username and password against your authentication system
                bool isAuthenticated = ValidateUser(username, password, clientIPAddress);

                // If the authentication fails, return a 401 Unauthorized response
                if (isAuthenticated)
                {
                    // The "Authorization" header is missing, so return a 401 Unauthorized response
                    TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Username or Password is invalid.");
                    db.TLIintegrationAccessLog.Add(log);
                    db.SaveChanges();
                    context.Result = new UnauthorizedObjectResult("Please Contact to administrator. ");
                    return;
                }
                else if(!isAuthenticated)
                {
                    string controllerName = context.RouteData.Values["controller"].ToString();
                    string actionName = context.RouteData.Values["action"].ToString();
                    var sys = db.TLIexternalSys.Where(x => x.UserName == username && x.IsDeleted == false && x.IsActive == true);
                    foreach (var item in sys)
                    {
                        var passwordDecrypt = Decrypt(item.Password);
                        if (passwordDecrypt == password)

                            if (item.IP != clientIPAddress)
                            {
                                TLIintegrationAccessLog log = new TLIintegrationAccessLog(item.SysName, item.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                                db.TLIintegrationAccessLog.Add(log);
                                db.SaveChanges();
                                context.Result = new UnauthorizedObjectResult("Ip is invalid,Please Contact to administrator. ");
                                return;
                            }
                        if (item.TLIexternalSysPermissions != null)
                        {
                            foreach (var s in item.TLIexternalSysPermissions)
                            {
                                var AccessApi = db.TLIinternalApis.FirstOrDefault(x => x.Id == s.InternalApiId && x.IsDeleted == false && x.IsActive == true);
                                if (AccessApi.ControllerName == controllerName && AccessApi.ActionName == actionName)
                                {
                                    context.Result = null;
                                    return;
                                }
                            }
                            TLIintegrationAccessLog log = new TLIintegrationAccessLog(item.SysName, item.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                            db.TLIintegrationAccessLog.Add(log);
                            db.SaveChanges();
                            context.Result = new UnauthorizedObjectResult("UserName Or Password is invalid,Please Contact to administrator. ");
                            return;


                        }
                        else
                        {
                            TLIintegrationAccessLog log = new TLIintegrationAccessLog(item.SysName, item.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                            db.TLIintegrationAccessLog.Add(log);
                            db.SaveChanges();
                            context.Result = new UnauthorizedObjectResult("UserName Or Password is invalid,Please Contact to administrator.. ");
                            return;


                        }

                    }
                }
                else
                {
                    // If the Authorization header is not present or is not of type Basic, return a 401 Unauthorized response
                    TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Authorization header is not present or is not of type Basic or bearer ");
                    db.TLIintegrationAccessLog.Add(log);
                    db.SaveChanges();
                    context.Result = new UnauthorizedObjectResult("Please Contact to administrator. ");
                    return;
                }
            }
        }
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

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var clientIPAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();

                if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    clientIPAddress = forwardedFor.ToString().Split(", ")[0];
                }
                // Get the "Authorization" header from the request
                if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    // The "Authorization" header is missing, so return a 401 Unauthorized response
                    TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Token is null in the header request.");
                    db.TLIintegrationAccessLog.Add(log);
                    db.SaveChanges();
                    context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                    return;
                }

                var token = authHeader.FirstOrDefault()?.Split(' ').Last();

                if (token == null)
                {
                  
                    TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Token is null in the header request.");
                    db.TLIintegrationAccessLog.Add(log);
                    db.SaveChanges();
                    context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                    return;
                }

                try
                {
                    string authHeader2 = context.HttpContext.Request.Headers["Authorization"];

                    if (!string.IsNullOrEmpty(authHeader) && authHeader2.StartsWith("Basic "))
                    {
                        return; 
                    }

                    var tokenHandler = new JwtSecurityTokenHandler();

                    // Set the validation parameters for the token
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("veryVerySecretKey")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };

                    // Try to validate and decode the token
                    var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                    var systemname = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (CheckTokenValidation(systemname) == true)
                    {

                        string controllerName = context.RouteData.Values["controller"].ToString();
                        string actionName = context.RouteData.Values["action"].ToString();


                        var extSys = db.TLIexternalSys.Include(x => x.TLIexternalSysPermissions).
                            FirstOrDefault(x => x.SysName == systemname  && x.IsActive == true && x.IsDeleted == false);

                        if (extSys != null)
                        {
                            if(extSys.IP!= clientIPAddress)
                            {
                                TLIintegrationAccessLog log = new TLIintegrationAccessLog(extSys.SysName, extSys.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                                db.TLIintegrationAccessLog.Add(log);
                                db.SaveChanges();
                                context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                                return;
                            }
                            if (extSys.Token != token)
                            {
                                TLIintegrationAccessLog log = new TLIintegrationAccessLog(extSys.SysName, extSys.UserName, clientIPAddress, "Your token is invalid ,Contact with TLIS administrator");
                                db.TLIintegrationAccessLog.Add(log);
                                db.SaveChanges();
                                context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                                return;
                            }
                            if (extSys.TLIexternalSysPermissions != null)
                            {
                                foreach (var s in extSys.TLIexternalSysPermissions)
                                {
                                    var AccessApi = db.TLIinternalApis.FirstOrDefault(x => x.Id == s.InternalApiId && x.IsDeleted == false && x.IsActive == true);
                                    if (AccessApi.ControllerName == controllerName && AccessApi.ActionName == actionName)
                                    {
                                        context.Result = null;
                                        return;
                                    }
                                }
                                TLIintegrationAccessLog log = new TLIintegrationAccessLog(extSys.SysName, extSys.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                                db.TLIintegrationAccessLog.Add(log);
                                db.SaveChanges();
                                context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                                return;


                            }
                            else
                            {
                                TLIintegrationAccessLog log = new TLIintegrationAccessLog(extSys.SysName, extSys.UserName, clientIPAddress, "Access denied from IP:" + clientIPAddress);
                                db.TLIintegrationAccessLog.Add(log);
                                db.SaveChanges();
                                context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                                return;


                            }
                        }
                        else
                        {

                            TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress,"External System not found");
                            db.TLIintegrationAccessLog.Add(log);
                            db.SaveChanges();
                            context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                            return;
                        }
                    }
                    else
                    {
                        TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Token expired,Contact to Administrator");
                        db.TLIintegrationAccessLog.Add(log);
                        db.SaveChanges();
                        context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                        return;

                    }


                }
                catch (Exception ex)
                {
                    TLIintegrationAccessLog log = new TLIintegrationAccessLog("NA", "NA", clientIPAddress, "Token expired,Contact to Administrator");
                    db.TLIintegrationAccessLog.Add(log);
                    db.SaveChanges();
                    context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                    return;
                }
            }
            catch (Exception ex2)
            {
                context.Result = new UnauthorizedObjectResult("Token is invalid,Please Contact to administrator. ");
                return;
            }

        }
       


        private bool CheckTokenValidation(string systemname)
        {
            var system = db.TLIexternalSys.FirstOrDefault(x=>x.SysName== systemname);
            if(system==null)
            {
                return false;
            }
            TimeSpan difference = DateTime.Now.Subtract(system.StartLife);

            if(difference.TotalDays <= system.LifeTime)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        private bool ValidateUser(string username, string password,string ip)
        {
            // Implement your authentication logic here
            // Return true if the username and password are valid, false otherwise
            var sys = db.TLIexternalSys.Any(x => x.UserName == username && x.Password == password && x.IP == ip);
            if(sys==true)
            {
                return true;
            }
            return false;
        }

       
    }
}
