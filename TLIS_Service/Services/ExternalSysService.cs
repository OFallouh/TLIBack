using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.IntegrationBinding;
using TLIS_DAL.ViewModels.IntegrationDto;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static TLIS_DAL.ViewModels.IntegrationDto.GetAllExternalSysDto;
using System.Security.Cryptography;
using System.Collections;
using TLIS_DAL.ViewModels.IntegrationDTOs;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TLIS_DAL.ViewModels.UserDTOs;
using System.Diagnostics.SymbolStore;
using TLIS_DAL.ViewModels.ComplixFilter;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper.Filters;
using System.IO;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
namespace TLIS_Service.Services
{
    internal class ExternalSysService : IexternalSysService
    {
        private ApplicationDbContext db;
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        private IConfiguration _config;
        //private  byte[] key = new byte[16]; // 128-bit key
        //private  byte[] iv = new byte[16]; // 128-bit IV

        public ExternalSysService(IConfiguration config, IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext _ApplicationDbContext, IMapper mapper)
        {
            db = _ApplicationDbContext;
            _unitOfWork = unitOfWork;
            _services = services;
            _config = config;
            _mapper = mapper;
        }

        public Response<string> CreateExternalSys(AddExternalSysBinding mod)
        {
            if (CheckUnique(0, mod.SysName) == true)
            {
                return new Response<string>(false, null, null, "System name is dublicated", (int)Helpers.Constants.ApiReturnCode.fail);

            }
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var secretKey = _config["JWT:Key"];
                    TLIexternalSys ext = new TLIexternalSys(mod, Encrypt(mod.Password));
                    db.TLIexternalSys.Add(ext);
                    db.SaveChanges();
                    if (mod.IsToken == true)
                    {
                        ext.Token = BuildToken(ext.Id, ext.UserName, secretKey);

                    }
                    else
                    {
                        ext.Token = null;
                    }
                    db.TLIexternalSys.Update(ext);
                    db.SaveChanges();
                    if (ext.Id != 0)
                    {
                        foreach (int s in mod.ApiPermissionIds)
                        {
                            TLIexternalSysPermissions per = new TLIexternalSysPermissions(ext.Id, s);
                            db.TLIexternalSysPermissions.Add(per);
                            db.SaveChanges();
                        }
                        transaction.Complete();
                        if (mod.IsToken == true)
                        {
                            return new Response<string>(true, ext.Token, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        }
                        return new Response<string>(true, ext.Password, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                    }

                    return new Response<string>(true, null, null, "External system has't Id", (int)Helpers.Constants.ApiReturnCode.success);


                }
                catch (Exception ex)
                {
                    return new Response<string>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);

                }
            }
        }

        public Response<string> EditExternalSys(EditExternalSysBinding mod)
        {
            if (CheckUnique(mod.Id, mod.SysName) == true)
            {
                return new Response<string>(false, null, null, "System name is dublicated", (int)Helpers.Constants.ApiReturnCode.fail);

            }
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var secretKey = _config["JWT:Key"];
                    var sys = db.TLIexternalSys.AsNoTracking().FirstOrDefault(x => x.Id == mod.Id);
                    if (sys == null)
                    {
                        return new Response<string>(true, null, null, "External system not found", (int)Helpers.Constants.ApiReturnCode.success);

                    }

                    TLIexternalSys ext = new TLIexternalSys(mod, Encrypt(mod.Password));
                    ext.IsActive = sys.IsActive;
                    if (mod.IsToken == true)
                    {
                        ext.Token = BuildToken(mod.Id, mod.UserName, secretKey);

                    }
                    else
                    {
                        ext.Token = null;
                    }
                    db.Entry(ext).State = EntityState.Modified;
                    db.SaveChanges();
                    //-------------------------------------change permissions
                    var pers = db.TLIexternalSysPermissions.Where(x => x.ExtSysId == mod.Id).ToList();
                    db.TLIexternalSysPermissions.RemoveRange(pers);
                    db.SaveChanges(true);

                    foreach (int s in mod.ApiPermissionIds)
                    {
                        TLIexternalSysPermissions per = new TLIexternalSysPermissions(mod.Id, s);
                        db.TLIexternalSysPermissions.Add(per);
                        db.SaveChanges();
                    }

                    transaction.Complete();
                    if (mod.IsToken == true)
                    {
                        return new Response<string>(true, ext.Token, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                    }
                    else
                    {
                        return new Response<string>(true, ext.Password, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                    }
                }
                catch (Exception ex)
                {
                    return new Response<string>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);

                }
            }
        }

        public Response<bool> DisableExternalSys(int id)
        {
            var ext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            if (ext != null)
            {
                ext.IsActive = !ext.IsActive;
                db.Entry(ext).State = EntityState.Modified;
                db.SaveChanges();
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            return new Response<bool>(false, false, null, "External system not found", (int)Helpers.Constants.ApiReturnCode.fail);

        }

        public Response<bool> DeleteExternalSys(int id)
        {
            var ext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            if (ext != null)
            {
                ext.IsDeleted = true;
                ext.IsActive = false;
                db.Entry(ext).State = EntityState.Modified;
                db.SaveChanges();
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            return new Response<bool>(false, false, null, "External system not found", (int)Helpers.Constants.ApiReturnCode.fail);

        }

        public Response<GetAllExternalSysDto> GetByIdExternalSys(int id)
        {
            var data = db.TLIexternalSys.Where(x => x.Id == id && x.IsDeleted == false).Include(x => x.TLIexternalSysPermissions).ThenInclude(y => y.TLIinternalApi).AsQueryable();
            var response = data.Select(x => new GetAllExternalSysDto()
            {
                Id = x.Id,
                SysName = x.SysName,
                UserName = x.UserName,
                Password = Decrypt(x.Password),
                IP = x.IP,
                IsActive = x.IsActive,
                Token = x.Token,
                StartLife = x.StartLife,
                EndLife = x.EndLife,
                LifeTime = x.LifeTime,
                InternalApiPermissions = x.TLIexternalSysPermissions.Select(x => new InternalApiPermission()
                {
                    Id = x.InternalApiId,
                    ApiLabel = x.TLIinternalApi.Label
                }).ToList()
            }).FirstOrDefault();
            if (response == null)
            {
                return new Response<GetAllExternalSysDto>(true, response, null, "Id invalid", (int)Helpers.Constants.ApiReturnCode.success);

            }

            return new Response<GetAllExternalSysDto>(true, response, null, null, (int)Helpers.Constants.ApiReturnCode.success);

        }

        public Response<List<GetAllExternalSysDto>> GetAllExternalSys(string systemName, ParameterPagination parameter)
        {

            var data = db.TLIexternalSys.Where(x => x.IsDeleted == false).Include(x => x.TLIexternalSysPermissions).ThenInclude(y => y.TLIinternalApi).AsQueryable();

            if (data != null)
            {

                var response = data.Select(x => new GetAllExternalSysDto()
                {
                    Id = x.Id,
                    SysName = x.SysName,
                    UserName = x.UserName,
                    Password = Decrypt(x.Password),
                    IP = x.IP,
                    IsActive = x.IsActive,
                    Token = x.Token,
                    StartLife = x.StartLife,
                    EndLife = x.EndLife,
                    LifeTime = x.LifeTime,
                    InternalApiPermissions = x.TLIexternalSysPermissions != null ? x.TLIexternalSysPermissions.Select(x => new InternalApiPermission()
                    {
                        Id = x.InternalApiId,
                        ApiLabel = x.TLIinternalApi.Label
                    }).ToList() : null
                }).ToList();


                if (!string.IsNullOrEmpty(systemName))
                {
                    response = response.Where(x => x.SysName.ToLower().StartsWith(systemName.ToLower())).ToList();

                }


                if (parameter != null)
                {
                    response = response.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
                }


                return new Response<List<GetAllExternalSysDto>>(true, response.ToList(), null, null, (int)Helpers.Constants.ApiReturnCode.success, data.ToList().Count());
            }
            else
            {
                return new Response<List<GetAllExternalSysDto>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success, data.ToList().Count());

            }
        }

        public Response<List<IntegrationViewModel>> GetAllIntegrationAPI()
        {

            var Apis = db.TLIinternalApis.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
            var response = _mapper.Map<List<IntegrationViewModel>>(Apis);
            return new Response<List<IntegrationViewModel>>(true, response, null, null, (int)Helpers.Constants.ApiReturnCode.success, Apis.Count());

        }

        public Response<string> RegenerateToken(int id)
        {
            var system = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            if (system == null)
            {
                return new Response<string>(false, null, null, "Invalid system", (int)Helpers.Constants.ApiReturnCode.fail);

            }
            var secretKey = _config["JWT:Key"];
            system.Token = BuildToken(system.Id, system.UserName, secretKey);
            system.StartLife = DateTime.Now;
            system.EndLife = system.StartLife.AddDays(system.LifeTime);

            db.Entry(system).State = EntityState.Modified;
            db.SaveChanges();
            return new Response<string>(true, system.Token, null, null, (int)Helpers.Constants.ApiReturnCode.success);

        }

        public Response<List<TLIintegrationAccessLog>> GetListErrorLog(ClxFilter f)
        {
            List<TLIintegrationAccessLog> result = new List<TLIintegrationAccessLog>();
            var logs = db.TLIintegrationAccessLog.AsQueryable();
            int count = logs.Count();
            if (f.Filters.Count != 0)
            {
                foreach (SimpleFilter c in f.Filters)
                {
                    var filterKeyProperty = typeof(TLIintegrationAccessLog).GetProperty(c.Key);
                    if (filterKeyProperty.PropertyType == typeof(string))
                    {
                        foreach (string s in c.Values)
                        {
                            logs = logs.Where(x => filterKeyProperty.GetValue(x).ToString().ToLower().StartsWith(s.ToLower()));

                        }

                    }
                    else if (filterKeyProperty.PropertyType == typeof(DateTime))
                    {
                        logs = logs.Where(x => c.Values.Contains(x.ActionDate.ToString("yyyy-MM-dd HH")));

                    }
                    else
                    {
                        logs = logs.Where(x => c.Values.Contains(filterKeyProperty.GetValue(x).ToString()));

                    }
                }
                result = logs.ToList();
                result = result.Skip((f.PageIndex - 1) * f.PageSize).Take(f.PageSize).ToList();
                return new Response<List<TLIintegrationAccessLog>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);

            }
            result = logs.ToList();
            result = result.Skip((f.PageIndex - 1) * f.PageSize).Take(f.PageSize).ToList();
            return new Response<List<TLIintegrationAccessLog>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);

        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                string key = "9443a09ae2e433750868beaeec0fd681";
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
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

        private bool CheckUnique(int id, string systemname)
        {
            var sys = db.TLIexternalSys.Any(x => x.SysName == systemname && x.Id != id && x.IsDeleted == false);
            return sys;
        }

        public string BuildToken(int userId, string userName, string secretKey)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Use userId for Sub claim
                new Claim(JwtRegisteredClaimNames.FamilyName, userName), // Use userName for FamilyName claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique identifier for the token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                "https://localhost:44311/",
                "https://localhost:44311/",
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["expireToken"])), // Expiry from configuration
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public Response<List<ExternalPermission>> GetAllExternalPermission()
        {
            try
            {
                List<ExternalPermission> ExternalPermissions = new List<ExternalPermission>();

                var AllExternalPermissions = db.TLIinternalApis.ToList();
                foreach (var AllExternalPermission in AllExternalPermissions)
                {
                    string[] parts = AllExternalPermission.ControllerName.Split('/');
                    ExternalPermission externalPermission = new ExternalPermission()
                    {
                        Id= AllExternalPermission.Id,
                        label = AllExternalPermission.Label,
                        Type = parts.Length > 1 ? parts[1] : null,
                        EndPoint = "api/" + (parts.Length > 0 ? parts[0] : null) + "/" + AllExternalPermission.ActionName,
                    };

                    ExternalPermissions.Add(externalPermission);
                }
                return new Response<List<ExternalPermission>>(true, ExternalPermissions, null, null, (int)Helpers.Constants.ApiReturnCode.success, ExternalPermissions.Count);

            }
            catch (Exception err)
            {

                return new Response<List<ExternalPermission>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }

        }
    }
 }
