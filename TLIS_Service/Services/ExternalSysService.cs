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
using Nancy.Bootstrapper;
using ClosedXML.Excel;
using static TLIS_Service.Helpers.Constants;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using static Dapper.SqlMapper;
using System.Reflection;
using TLIS_DAL.ViewModels.SiteDTOs;
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

        public Response<string> CreateExternalSys(AddExternalSysBinding mod,int UserId)
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
                    var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == "TLIexternalSys".ToLower()).Id;
                    TLIexternalSys ext = new TLIexternalSys(mod, Encrypt(mod.Password));
                    db.TLIexternalSys.Add(ext);
                    db.SaveChanges();

                    if (mod.IsToken == true)
                    {
                        ext.Token = BuildToken(ext.Id, ext.UserName, secretKey, ext.LifeTime);

                    }
                    else
                    {
                        ext.Token = null;
                    }
                    db.TLIexternalSys.Update(ext);
                    db.SaveChanges();
                    TLIhistory exthistory = new TLIhistory()
                    {
                        TablesNameId = TabelNameId,
                        HistoryTypeId = 1,
                        UserId = UserId
                    };
                    db.TLIhistory.Add(exthistory);
                    db.SaveChanges();
                    AddWithHDynamic(UserId, TabelNameId, ext, exthistory.Id);
                    
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

        public Response<string> EditExternalSys(EditExternalSysBinding mod,int UserId)
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
                    var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == "TLIexternalSys".ToLower()).Id;
                    TLIexternalSys ext = new TLIexternalSys(mod, Encrypt(mod.Password));
                    ext.IsActive = sys.IsActive;
                    if (mod.IsToken == true)
                    {
                        ext.Token = BuildToken(mod.Id, mod.UserName, secretKey, mod.LifeTime);

                    }
                    else
                    {
                        ext.Token = null;
                    }
                    db.Entry(ext).State = EntityState.Modified;
                    db.SaveChanges();
                    TLIhistory exthistory = new TLIhistory()
                    {
                        TablesNameId = TabelNameId,
                        HistoryTypeId = 2,
                        UserId = UserId,
                       
                    };
                    db.TLIhistory.Add(exthistory);
                    db.SaveChanges();
                    UpdateWithHLogic(UserId, exthistory.Id, TabelNameId, sys, ext, mod.Id);
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
        public virtual void UpdateWithHLogic(int? userId, int historyId, int tableNameId, TLIexternalSys oldObject, TLIexternalSys newObject, int recordId)
        {
            if (userId != null)
            {
            

                List<PropertyInfo> attributes = oldObject.GetType().GetProperties()
                    .Where(x => x.PropertyType.IsGenericType
                        ? x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            && (x.PropertyType.GetGenericArguments()[0] == typeof(int)
                            || x.PropertyType.GetGenericArguments()[0] == typeof(string)
                            || x.PropertyType.GetGenericArguments()[0] == typeof(double)
                            || x.PropertyType.GetGenericArguments()[0] == typeof(float)
                            || x.PropertyType.GetGenericArguments()[0] == typeof(bool)
                            || x.PropertyType.GetGenericArguments()[0] == typeof(DateTime))
                        : (x.PropertyType == typeof(int)
                        || x.PropertyType == typeof(double)
                        || x.PropertyType == typeof(string)
                        || x.PropertyType == typeof(bool)
                        || x.PropertyType == typeof(DateTime)
                        || x.PropertyType == typeof(float)
                        || x.PropertyType == typeof(Single)))
                    .ToList();

                List<TLIhistoryDet> listOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo attribute in attributes)
                {
                    object oldAttributeValue = attribute.GetValue(oldObject);
                    object newAttributeValue = attribute.GetValue(newObject);

                    if ((oldAttributeValue != null && newAttributeValue != null && oldAttributeValue.ToString() == newAttributeValue.ToString())
                        || (oldAttributeValue == null && newAttributeValue == null))
                        continue;

                    TLIhistoryDet historyDetails = new TLIhistoryDet
                    {
                        TablesNameId = tableNameId,
                        RecordId = recordId.ToString(),
                        HistoryId = historyId,
                        AttributeName = attribute.Name,
                        OldValue = oldAttributeValue?.ToString(),
                        NewValue = newAttributeValue?.ToString(),
                        AttributeType = AttributeType.Static
                    };

                    listOfHistoryDetailsToAdd.Add(historyDetails);
                }

                db.TLIhistoryDet.AddRange(listOfHistoryDetailsToAdd);
                db.SaveChanges();
            }

            oldObject = newObject;
            db.Entry(oldObject).State = EntityState.Modified; 
            db.SaveChanges();
        }

        public virtual void AddWithHDynamic(int? UserId, int TabelNameId, TLIexternalSys AddObject, int HistoryId)
        {

            
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = db.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == AddObject.GetType().Name.ToLower());

              
                int entityId = (int)AddObject.GetType().GetProperty("Id").GetValue(AddObject, null);
                string entityIdString = entityId.ToString();

                List<PropertyInfo> Attributes = AddObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDet> ListOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object NewAttributeValue = Attribute.GetValue(AddObject, null);

                    TLIhistoryDet HistoryDetails = new TLIhistoryDet
                    {
                        TablesNameId = EntityTableNameModel.Id,
                        RecordId = entityIdString,
                        HistoryId = HistoryId,
                        AttributeName = Attribute.Name,
                        NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                        AttributeType = AttributeType.Static
                    };
                    ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                }

                db.TLIhistoryDet.AddRange(ListOfHistoryDetailsToAdd);
                db.SaveChanges();
            }
        }
        public Response<bool> DisableExternalSys(int id,int UserId)
        {
            var Oldext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            var ext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            if (ext != null)
            {
                var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == "TLIexternalSys".ToLower()).Id;
                ext.IsActive = !ext.IsActive;
                db.Entry(ext).State = EntityState.Modified;
                db.SaveChanges();
                TLIhistory exthistory = new TLIhistory()
                {
                    TablesNameId = TabelNameId,
                    HistoryTypeId = 2,
                    UserId = UserId,

                };
                db.TLIhistory.Add(exthistory);
                db.SaveChanges();
                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                {

                    RecordId = id.ToString(),
                    TablesNameId = TabelNameId,
                    OldValue = Oldext.IsActive.ToString(),
                    NewValue = ext.IsActive.ToString(),
                    AttributeName = "IsActive"

                };
                db.TLIhistoryDet.Add(tLIhistoryDet);
                db.SaveChanges();
                   

                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            return new Response<bool>(false, false, null, "External system not found", (int)Helpers.Constants.ApiReturnCode.fail);

        }

        public Response<bool> DeleteExternalSys(int id,int UserId)
        {
            var ext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            var Oldext = db.TLIexternalSys.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
            var TabelNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == "TLIexternalSys".ToLower()).Id;
            if (ext != null)
            {
                ext.IsDeleted = true;
                ext.IsActive = false;
                db.Entry(ext).State = EntityState.Modified;
                db.SaveChanges();
                TLIhistory exthistory = new TLIhistory()
                {
                    TablesNameId = TabelNameId,
                    HistoryTypeId = 2,
                    UserId = UserId,

                };
                db.TLIhistory.Add(exthistory);
                db.SaveChanges();
                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                {

                    RecordId = id.ToString(),
                    TablesNameId = TabelNameId,
                    OldValue = Oldext.IsDeleted.ToString(),
                    NewValue = ext.IsDeleted.ToString(),
                    AttributeName = "IsDeleted"

                };
                db.TLIhistoryDet.Add(tLIhistoryDet);
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

        public Response<List<GetAllExternalSysDto>> GetAllExternalSys()
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
            system.Token = BuildToken(system.Id, system.UserName, secretKey, system.LifeTime);
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
            result = logs.ToList();
            if (f.Filters.Count != 0)
            {
                if (f.Filters != null ? f.Filters.Count != 0 : false)
                {
                    foreach (SimpleFilter SimpleFilter in f.Filters)
                    {
                        PropertyInfo? FilterKeyProperty = typeof(TLIintegrationAccessLog).GetProperty(SimpleFilter.Key);

                        if (FilterKeyProperty != null && FilterKeyProperty.PropertyType == typeof(string) || FilterKeyProperty.PropertyType == typeof(bool))
                        {
                            foreach (string FilterValue in SimpleFilter.Values)
                            {
                                result = result.Where(x => FilterKeyProperty.GetValue(x).ToString().ToLower().Contains(FilterValue.ToLower())).ToList();
                            }
                        }

                        else
                        {
                            result = result.Where(x => SimpleFilter.Values.Contains(FilterKeyProperty.GetValue(x).ToString())).ToList();
                        }

                    }
                }
                result = result.Skip((f.PageIndex - 1) * f.PageSize).Take(f.PageSize).ToList();
                return new Response<List<TLIintegrationAccessLog>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);

            }
         
            result = result.Skip((f.PageIndex - 1) * f.PageSize).Take(f.PageSize).ToList();
            return new Response<List<TLIintegrationAccessLog>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);

        }

        public Response<string> GetListErrorLogExport()
        {
            List<TLIintegrationAccessLog> result = new List<TLIintegrationAccessLog>();
            string excelFilePath = null;
            var logs = db.TLIintegrationAccessLog.AsQueryable();
            int count = logs.Count();
            result = logs.ToList();
            excelFilePath = ExportToExcel(result, "All Errors in internalApi");
            return new Response<string>(true, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);

        }

        public string ExportToExcel(List<TLIintegrationAccessLog> data, string tableName)
        {
            var folderPath = _config["StoreFiles"];
            string downloadFolder = Path.Combine(folderPath, "GenerateFiles");

            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }

            var filePath = Path.Combine(downloadFolder, tableName + ".xlsx");

            // Check if the file exists, and delete it if it does
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (data.Any())
                {
                    // Add headers with formatted names (adding spaces before capital letters)
                    var headers = typeof(TLIintegrationAccessLog).GetProperties()
                        .Select(prop => FormatHeader(prop.Name))
                        .ToList();

                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    // Add data rows
                    int row = 2;
                    foreach (var item in data)
                    {
                        for (int col = 0; col < headers.Count; col++)
                        {
                            var value = typeof(TLIintegrationAccessLog).GetProperty(headers[col].Replace(" ", "")).GetValue(item)?.ToString() ?? string.Empty;
                            worksheet.Cell(row, col + 1).Value = value;
                        }
                        row++;
                    }
                }
                else
                {
                    worksheet.Cell(1, 1).Value = "No data available to export.";
                }

                workbook.SaveAs(filePath);
            }

            return filePath;
        }

        // Method to add spaces before capital letters
        private string FormatHeader(string header)
        {
            return string.Concat(header.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
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

        public string BuildToken(int userId, string userName, string secretKey,int LifeTime)
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
                expires: DateTime.Now.AddDays(LifeTime), // Expiry from configuration
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
