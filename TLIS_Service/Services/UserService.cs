using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.ServiceBase;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AutoMapper;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TLIS_DAL;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.IO;
using LinqToExcel.Extensions;
using Microsoft.EntityFrameworkCore.Update.Internal;
using TLIS_DAL.ViewModels.wf;
using static TLIS_Service.Services.UserService;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TLIS_Service.Helpers.Constants;
using DocumentFormat.OpenXml.InkML;
using static TLIS_Service.Services.SiteService;
using System.Text.Json;
using TLIS_DAL.ViewModels.SiteDTOs;
using System.Linq.Expressions;
namespace TLIS_Service.Services
{
    public class UserService : IUserService
    {

        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        private byte[] key = new byte[16]; // 128-bit key
        private byte[] iv = new byte[16]; // 128-bit IV

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, ApplicationDbContext context,
            IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _services = services;
            _mapper = mapper;
            _dbContext = context;
        }
        //Function to add external user 
        //usually external user type is 2

        public async Task<Response<UserViewModel>> AddExternalUser(AddUserViewModel model, string domain, int UserId)
        {
            var connectionString = _configuration["ConnectionStrings:ActiveConnection"];

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // التحقق من صحة المستخدم
                        var validationResponse = ValidateUserInAdAndDb(model.UserName, domain);
                        if (validationResponse.Data)
                        {
                            // إدراج المستخدم
                            var userId = await InsertUserAsync(connection, model);

                            // إدراج الصلاحيات إذا كانت موجودة
                            if (model.Permissions != null && model.Permissions.Count > 0)
                            {
                                await InsertPermissionsAsync(connection, userId, model.Permissions);
                            }

                            // إدراج المجموعات إذا كانت موجودة
                            if (model.Groups != null && model.Groups.Count > 0)
                            {
                                await InsertGroupsAsync(connection, userId, model.Groups);
                            }

                            var TabelNameUser = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIuser").Id;
                            TLIhistory AddTablesHistory = new TLIhistory
                            {
                                HistoryTypeId = 1,
                                RecordId = userId.ToString(),
                                TablesNameId = TabelNameUser,
                                UserId = UserId
                            };


                            await _dbContext.TLIhistory.AddAsync(AddTablesHistory);
                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();
                            return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
                        }
                        else
                        {
                            // التراجع عن المعاملة إذا لم يكن المستخدم صالحًا
                            transaction.Rollback();
                            return new Response<UserViewModel>(false, null, null, validationResponse.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // في حالة حدوث استثناء، سجل رسالة الخطأ
                return new Response<UserViewModel>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
            }
        }


        private async Task<int> InsertUserAsync(OracleConnection connection, AddUserViewModel model)
        {
            var query = @"
            INSERT INTO ""TLIuser"" 
            (""FirstName"", ""MiddleName"", ""LastName"", ""Email"", ""MobileNumber"", ""UserName"", 
             ""Password"", ""UserType"", ""Active"", 
             ""Deleted"", ""ValidateAccount"")
            VALUES 
            (:FirstName, :MiddleName, :LastName, :Email, :MobileNumber, :UserName, 
             :Password, :UserType, :Active, , :IsFirstLogin, 
             :Deleted, :ValidateAccount)
            RETURNING ""Id"" INTO :UserId";

            using (var command = new OracleCommand(query, connection))
            {

                command.Parameters.Add(new OracleParameter("FirstName", OracleDbType.Varchar2)).Value = (object)model.FirstName ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("MiddleName", OracleDbType.Varchar2)).Value = (object)model.MiddleName ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("LastName", OracleDbType.Varchar2)).Value = (object)model.LastName ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("Email", OracleDbType.Varchar2)).Value = (object)model.Email ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("MobileNumber", OracleDbType.Varchar2)).Value = (object)model.MobileNumber ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("UserName", OracleDbType.Varchar2)).Value = model.UserName;
                command.Parameters.Add(new OracleParameter("Password", OracleDbType.Varchar2)).Value = (object)model.Password ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter("UserType", OracleDbType.Int32)).Value = model.UserType;


                command.Parameters.Add(new OracleParameter("Active", OracleDbType.Int32)).Value = 1;
                command.Parameters.Add(new OracleParameter("Deleted", OracleDbType.Int32)).Value = 0;
                command.Parameters.Add(new OracleParameter("ValidateAccount", OracleDbType.Int32)).Value = 1;
                command.Parameters.Add(new OracleParameter("IsFirstLogin", OracleDbType.Int32)).Value = 1;


                var userIdParam = new OracleParameter("UserId", OracleDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(userIdParam);

                await command.ExecuteNonQueryAsync();


                var userIdDecimal = (OracleDecimal)userIdParam.Value;
                return userIdDecimal.ToInt32();

            }
        }




        private async Task InsertPermissionsAsync(OracleConnection connection, int userId, List<string> permissions)
        {
            var query = @"
            INSERT INTO ""TLIuser_Permissions"" (""UserId"", ""PageUrl"", ""Active"", ""Delete"")
            VALUES (:pUserId, :pPageUrl, :pActive, :pDelete)";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.Add(new OracleParameter("pUserId", OracleDbType.Int32)).Value = userId;
                var pageUrlParam = new OracleParameter("pPageUrl", OracleDbType.Varchar2);
                command.Parameters.Add(pageUrlParam);
                command.Parameters.Add(new OracleParameter("pActive", OracleDbType.Int32)).Value = 1; // true
                command.Parameters.Add(new OracleParameter("pDeleted", OracleDbType.Int32)).Value = 0; // false


                foreach (var permission in permissions)
                {
                    pageUrlParam.Value = permission;
                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        private async Task InsertGroupsAsync(OracleConnection connection, int userId, List<GroupNamesViewModel> groups)
        {
            var query = @"
        INSERT INTO ""TLIgroupUser"" (""GroupId"", ""UserId"")
        VALUES (:GroupId, :UserId)";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.Add(new OracleParameter("GroupId", OracleDbType.Int32));
                command.Parameters.Add(new OracleParameter("UserId", OracleDbType.Int32)).Value = userId;

                foreach (var group in groups)
                {
                    if (group.Id > 0)
                    {
                        command.Parameters["GroupId"].Value = group.Id;
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        // Helper Method For Sending Confirmation Code To The User's Email..
        public Response<string> SendConfirmationCode(string UserEmail, int? UserId)
        {
            try
            {
                string to;
                if (!String.IsNullOrEmpty(UserEmail))
                {
                    to = UserEmail;
                }
                else
                {
                    to = _unitOfWork.UserRepository.GetWhereSelectFirst(x => x.Id == UserId.Value && x.Active && !x.Deleted, x => x.Email);
                }
                string from = _configuration["SMTP:AhmadAhmad:user"].ToString();
                MailMessage message = new MailMessage(from, to);
                Random Generator = new Random();
                string ConfirmationCode = Generator.Next(0, 1000000).ToString("D6");
                string mailbody = $"This Is Your Confirmation Code \t{ConfirmationCode}";
                message.Subject = "Confirm Your Email";
                message.Body = mailbody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(_configuration["SMTP:AhmadAhmad:server"].ToString(), 587); //Gmail smtp    
                NetworkCredential basicCredential1 = new NetworkCredential(_configuration["SMTP:AhmadAhmad:user"].ToString(), _configuration["SMTP:AhmadAhmad:pass"].ToString());
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;

                client.Send(message);
                return new Response<string>(true, ConfirmationCode, null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
            }
        }
        public async Task<Response<string>> ValidateEmail(string UserConfirmCode, int UserId)
        {
            try
            {
                TLIuser user = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == UserId && x.Active && !x.Deleted);
                if (user != null)
                {
                    if (user.ConfirmationCode == UserConfirmCode)
                    {
                        user.ValidateAccount = true;
                        user.ConfirmationCode = null;
                        _unitOfWork.UserRepository.Update(user);
                        await _unitOfWork.SaveChangesAsync();

                        return new Response<string>(true, "Succeed", null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
                    }
                    else
                    {
                        return new Response<string>(true, "Invalide Confirmation Code", null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
                    }
                }

                return new Response<string>(true, "Invalide User Id", null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
            }
        }

        //Function to add internal userpublic 
        //usually internal user type is 1
        public async Task<Response<UserViewModel>> AddInternalUser(string UserName, List<String> Permissions, string domain, int UserId)
        {
            try
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                using (var connection = new OracleConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {


                        using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind, null, null))
                        {
                            //TLIuser CheckUser = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName.ToLower() == UserName.ToLower());
                            //if (CheckUser != null)
                            //    return new Response<UserViewModel>(true, null, null, $"This User {UserName} is already Exist in TLIS", (int)Helpers.Constants.ApiReturnCode.fail);

                            UserPrincipal principal = new UserPrincipal(context);
                            UserViewModel userModel = null;
                            if (context != null)
                            {
                                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "TLI");
                                principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName);
                                if (principal != null && principal.IsMemberOf(group))
                                {
                                    if (principal.Name.Replace($" {principal.Surname}", "") == null || principal.Surname == null ||
                                       principal.EmailAddress == null || principal.SamAccountName == null)
                                    {
                                        return new Response<UserViewModel>(true, null, null, "This user information is insufficient in Active Directory", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    TLIuser user = new TLIuser();
                                    user.FirstName = principal.Name.Replace($" {principal.Surname}", "");
                                    user.MiddleName = principal.MiddleName;
                                    user.LastName = principal.Surname;
                                    user.Email = principal.EmailAddress;
                                    user.MobileNumber = principal.VoiceTelephoneNumber;
                                    user.UserName = principal.SamAccountName;
                                    user.IsFirstLogin=true;
                                    var tliuser = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName == UserName && !x.Deleted);
                                    if (tliuser != null)
                                    {
                                        return new Response<UserViewModel>(false, null, null, $"This User {UserName} is Already Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                    user.Domain = null;
                                    user.AdGUID = principal.Guid.ToString();
                                    user.UserType = 1;
                                    await _unitOfWork.UserRepository.AddAsyncWithH(UserId, null, user);
                                    await _unitOfWork.SaveChangesAsync();

                                    if (Permissions != null)
                                    {
                                        await InsertPermissionsAsync(connection, user.Id, Permissions);
                                    }

                                }
                                else
                                {
                                    return new Response<UserViewModel>(false, null, null, $"This User {UserName} is Not Exist in AD", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            transaction.Commit();
                            return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function enable or disable user depened on user status
        public async Task<Response<UserViewModel>> DeactivateUser(int UserId, int userid)
        {
            try
            {
                TLIuser OldUser = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault
                    (x => x.Id == UserId && !x.Deleted);
                if (OldUser == null)
                    return new Response<UserViewModel>(true, null, null, "This user is not found", (int)Helpers.Constants.ApiReturnCode.fail);

                TLIuser User = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == UserId && !x.Deleted);
                User.Active = !(User.Active);
                _unitOfWork.UserRepository.UpdateWithH(userid, null, OldUser, User, false);
                await _unitOfWork.SaveChangesAsync();
                return new Response<UserViewModel>();
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function to GetAll active users
        public Response<List<UserViewModel>> GetAll(List<FilterObjectList> filters, ParameterPagination parameter)
        {
            List<UserViewModel> Users = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository
                .GetWhere(x => !x.Deleted && x.Active).ToList());

            foreach (FilterObjectList Filter in filters)
            {
                string FilterKey = Filter.key;

                foreach (object FilterValue in Filter.value)
                {
                    Users = Users.Where(x => x.GetType().GetProperties()
                        .FirstOrDefault(Attribute => Attribute.Name.ToLower() == FilterKey.ToLower()).GetValue(x, null).ToString().ToLower()
                            .StartsWith(FilterValue.ToString().ToLower())).ToList();
                }
            }

            return new Response<List<UserViewModel>>(true, Users, null, null, (int)Helpers.Constants.ApiReturnCode.success, Users.Count());
        }

        //Function to get all external users
        public async Task<Response<List<UserViewModel>>> GetAllExternalUsers(string UserName, ParameterPagination parameter)
        {
            try
            {
                List<TLIuser> ExternalUsers = new List<TLIuser>();
                if (string.IsNullOrEmpty(UserName))
                    ExternalUsers = _unitOfWork.UserRepository.GetWhere(x => !x.Deleted && x.UserType == 2).OrderBy(x => x.UserName).ToList();
                else
                    ExternalUsers = _unitOfWork.UserRepository.GetWhere(x => !x.Deleted && x.UserType == 2 && x.UserName.ToLower().StartsWith(UserName.ToLower()))
                        .OrderBy(x => x.UserName).ToList();

                List<UserViewModel> UsersViewModels = _mapper.Map<List<UserViewModel>>(ExternalUsers);

                foreach (UserViewModel User in UsersViewModels)
                {
                    List<int> PermissionsIds = _unitOfWork.UserPermissionRepository.GetWhere(x =>
                        x.userId == User.Id).Select(x => x.permissionId).Distinct().ToList();

                    List<PermissionViewModel> Permissions = _mapper.Map<List<PermissionViewModel>>(_unitOfWork.PermissionRepository.GetWhere(x =>
                        PermissionsIds.Contains(x.Id) && x.Active).ToList());

                    // User.Permissions = Permissions;

                    List<int> GroupsIds = _unitOfWork.GroupUserRepository.GetWhere(x =>
                        x.userId == User.Id).Select(x => x.groupId).Distinct().ToList();

                    List<GroupNamesViewModel> GroupsNames = _mapper.Map<List<GroupNamesViewModel>>(_unitOfWork.GroupRepository.GetWhere(x =>
                        GroupsIds.Contains(x.Id) && x.Active && !x.Deleted).ToList());

                    User.Groups = GroupsNames;
                }

                int Count = UsersViewModels.Count();
                UsersViewModels = UsersViewModels.Skip((parameter.PageNumber - 1) * parameter.PageSize)
                    .Take(parameter.PageSize).AsQueryable().OrderBy(x => x.UserName).ToList();

                return new Response<List<UserViewModel>>(true, UsersViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<UserViewModel>> GetExternalUsersByName(string UserName, ParameterPagination parameterPagination)
        {
            try
            {
                List<UserViewModel> UserModel = new List<UserViewModel>();

                if (string.IsNullOrEmpty(UserName))
                    UserModel = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository
                        .GetWhere(x => x.UserType == 2 && !x.Deleted)).OrderBy(x => x.UserName).ToList();
                else
                    UserModel = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository
                        .GetWhere(x => x.UserName.ToLower().StartsWith(UserName.ToLower()) && x.UserType == 2 && !x.Deleted).OrderBy(x => x.UserName).ToList());

                int Count = UserModel.Count();

                UserModel = UserModel.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                    .Take(parameterPagination.PageSize).AsQueryable().ToList();

                return new Response<List<UserViewModel>>(true, UserModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail); ;
            }
        }
        //Function to get all internal users
        public async Task<Response<List<UserViewModel>>> GetAllInternalUsers(string UserName, ParameterPagination parameter)
        {
            try
            {
                List<TLIuser> ExternalUsers = new List<TLIuser>();
                if (string.IsNullOrEmpty(UserName))
                    ExternalUsers = _unitOfWork.UserRepository.GetWhere(x => !x.Deleted && x.UserType == 1).OrderBy(x => x.UserName).ToList();
                else
                    ExternalUsers = _unitOfWork.UserRepository.GetWhere(x => !x.Deleted && x.UserType == 1 && x.UserName.ToLower().StartsWith(UserName.ToLower()))
                        .OrderBy(x => x.UserName).ToList();

                List<UserViewModel> UsersViewModels = _mapper.Map<List<UserViewModel>>(ExternalUsers);

                foreach (UserViewModel User in UsersViewModels)
                {
                    List<int> PermissionsIds = _unitOfWork.UserPermissionRepository.GetWhere(x =>
                        x.userId == User.Id).Select(x => x.permissionId).Distinct().ToList();

                    List<PermissionViewModel> Permissions = _mapper.Map<List<PermissionViewModel>>(_unitOfWork.PermissionRepository.GetWhere(x =>
                        PermissionsIds.Contains(x.Id) && x.Active).ToList());

                    // User.Permissions = Permissions;

                    List<int> GroupsIds = _unitOfWork.GroupUserRepository.GetWhere(x =>
                        x.userId == User.Id).Select(x => x.groupId).Distinct().ToList();

                    List<GroupNamesViewModel> GroupsNames = _mapper.Map<List<GroupNamesViewModel>>(_unitOfWork.GroupRepository.GetWhere(x =>
                        GroupsIds.Contains(x.Id) && x.Active && !x.Deleted).ToList());

                    User.Groups = GroupsNames;
                }

                int Count = UsersViewModels.Count();
                UsersViewModels = UsersViewModels.Skip((parameter.PageNumber - 1) * parameter.PageSize)
                    .Take(parameter.PageSize).AsQueryable().OrderBy(x => x.UserName).ToList();

                return new Response<List<UserViewModel>>(true, UsersViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<UserViewModel>> GetInternalUsersByName(string UserName, ParameterPagination parameter)
        {
            try
            {
                List<UserViewModel> UserModel = new List<UserViewModel>();

                if (string.IsNullOrEmpty(UserName))
                    UserModel = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository
                        .GetWhere(x => x.UserType == 1 && !x.Deleted).OrderBy(x => x.UserName).ToList());
                else
                    UserModel = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository
                        .GetWhere(x => x.UserName.ToLower().StartsWith(UserName.ToLower()) && x.UserType == 1 && !x.Deleted).OrderBy(x => x.UserName).ToList());

                int Count = UserModel.Count();

                UserModel = UserModel.Skip((parameter.PageNumber - 1) * parameter.PageSize)
                    .Take(parameter.PageSize).AsQueryable().ToList();

                return new Response<List<UserViewModel>>(true, UserModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<UserViewModel>> GetUsersByUserType(int UserTypeId, string UserName, ParameterPagination parameterPagination)
        {
            try
            {
                List<UserViewModel> Usermodel = _mapper.Map<List<UserViewModel>>(_unitOfWork.UserRepository.
                    GetWhere(x => x.UserType == UserTypeId && x.Active && !x.Deleted &&
                    (!string.IsNullOrEmpty(UserName) ? x.UserName.ToLower().StartsWith(UserName.ToLower()) : true)).
                        Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                        .Take(parameterPagination.PageSize).AsQueryable().ToList());

                return new Response<List<UserViewModel>>(true, Usermodel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function to get user information and list his groups and list of his permissions
        public async Task<Response<UserViewModel>> GetUserById(int Id)
        {
            try
            {
                var newPermissionsViewModelsUser = new List<string>();
                UserViewModel User = _mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetWhereFirst(x => x.Id == Id && !x.Deleted));
                List<PermissionsGroup> Group = new List<PermissionsGroup>();

                if (User != null)
                {
                    User.WorkFlowMode = _configuration["WorkFlowMode"].ToString();
                    List<string> UserPermissions = _unitOfWork.UserPermissionssRepository.GetWhere(x =>
                    x.UserId == Id && x.Active == true && x.Delete == false).Select(x => x.PageUrl).ToList();
                    List<int> GroupUserId = _unitOfWork.GroupUserRepository.GetWhere(x => x.userId == Id && x.Active && !x.Deleted).Select(x => x.groupId).ToList();
                    List<int?> ParentGroup = GetParentGroup(GroupUserId);
                    foreach (var item in ParentGroup)
                    {
                        List<int> RoleGroup = _unitOfWork.GroupRoleRepository.GetWhere(x => x.groupId == item && !x.Deleted && x.Active).Select(x => x.roleId).ToList();
                        List<string> Rolepermissions = _unitOfWork.RolePermissionsRepository.GetWhere(x => RoleGroup.Any(y => y == x.RoleId) && !x.Delete && x.Active).Select(x => x.PageUrl).ToList();
                        TLIgroup ObjGroupName = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == item);
                        string GroupName = ObjGroupName?.Name;
                        Group.Add(new PermissionsGroup()
                        {
                            GroupId = item,
                            GroupName = GroupName,
                            PermissionsOfGroup = Rolepermissions
                        });
                    }
                    User.Groups = await _unitOfWork.GroupUserRepository.GetAllAsQueryable().AsNoTracking()
                        .Where(x => x.userId == User.Id && x.Active && !x.Deleted).Select(g => new GroupNamesViewModel(g.groupId, g.group.Name)).ToListAsync();
                    newPermissionsViewModelsUser.AddRange(UserPermissions);
                    User.PermissionsUser = newPermissionsViewModelsUser;
                    User.PermissionsRole = Group;
                }
                else
                {
                    return new Response<UserViewModel>(false, null, null, "The Id Is Not Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<UserViewModel>(true, User, null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
            }
        }

        //Function to get users by group name
        public Response<List<UserViewModel>> GetUsersByGroupName(string GroupName, string domain)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind))
                {
                    GroupPrincipal groupPrincipal = null;
                    if (context != null)
                    {
                        groupPrincipal = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, GroupName);
                    }
                    if (groupPrincipal != null)
                    {
                        var GroupUsers = groupPrincipal.GetMembers().ToList();
                        List<UserViewModel> users = new List<UserViewModel>();
                        //foreach (var GroupUser in GroupUsers)
                        //{
                        //    var test = _unitOfWork.UserRepository.GetAllAsQueryable().Where(u => u.AdGUID == GroupUser.Guid.ToString()).FirstOrDefault();
                        //    if (test != null)
                        //    {
                        //        var user = _unitOfWork.UserRepository.GetAllAsQueryable().Where(u => u.AdGUID == GroupUser.Guid.ToString()).FirstOrDefault();
                        //        users.Add(_mapper.Map<UserViewModel>(user));
                        //    }
                        //}
                        Parallel.ForEach(GroupUsers, GroupUser =>
                        {
                            var test = _unitOfWork.UserRepository.GetAllAsQueryable().Where(u => u.AdGUID == GroupUser.Guid.ToString()).FirstOrDefault();
                            if (test != null)
                            {
                                var user = _unitOfWork.UserRepository.GetAllAsQueryable().Where(u => u.AdGUID == GroupUser.Guid.ToString()).FirstOrDefault();
                                users.Add(_mapper.Map<UserViewModel>(user));
                            }
                        });
                        return new Response<List<UserViewModel>>(true, users, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                    else
                    {
                        return new Response<List<UserViewModel>>(true, null, null, "The group is not exist", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
            }
            catch (Exception err)
            {

                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function to update user and his permissions
        public async Task<Response<UserViewModel>> Updateuser(EditUserViewModel model, int UserId)
        {
            string OldP = null;
            string NewP = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            using (var connection = new OracleConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (model.UserType == 2)
                        {
                            var OldUserInfo = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == model.Id);
                            if (OldUserInfo == null)
                                return new Response<UserViewModel>(false, null, null, $"This User is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                            var UserName = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName.ToLower() == model.UserName.ToLower() && x.Id != model.Id);
                            if (UserName != null)
                            {
                                return new Response<UserViewModel>(false, null, null, $"This User Name {model.UserName} Is Already Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            TLIuser UserEntity = _mapper.Map<TLIuser>(model);

                            UserEntity.Password = null;

                            string OldPassword = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == model.Id)?.Password;
                          
                            UserEntity.Password = OldPassword;
                            
                            UserEntity.Active = OldUserInfo.Active;
                            UserEntity.Deleted = false;
                            _unitOfWork.UserRepository.UpdateWithH(UserId, null, OldUserInfo, UserEntity, false);
                            await _unitOfWork.SaveChangesAsync();

                            List<string> AllUserPermissionsInDB = _unitOfWork.UserPermissionssRepository
                              .GetWhere(x => x.UserId == model.Id && x.Delete == false && x.Active == true).Select(x => x.PageUrl).ToList();

                            var DeletePermissions = _unitOfWork.UserPermissionssRepository.GetWhere(x => x.UserId == model.Id);
                            _unitOfWork.UserPermissionssRepository.RemoveRangeItems(DeletePermissions);
                            await _unitOfWork.SaveChangesAsync();

                            if (model.permissions != null)
                            {
                                await InsertPermissionsAsync(connection, model.Id, model.permissions);
                            }
                            List<int> UserGroups = _unitOfWork.GroupUserRepository.GetWhere(x =>
                                x.userId == model.Id).Select(x => x.groupId).Distinct().ToList();
                            List<int> ModelGroups = model.Groups.Select(x => x.Id).ToList();

                            List<int> GroupsToDelete = UserGroups.Except(ModelGroups).ToList();
                            foreach (var GroupId in GroupsToDelete)
                            {
                                TLIgroupUser UserGroup = _unitOfWork.GroupUserRepository.GetWhereFirst(u => u.groupId == GroupId && u.userId == model.Id);
                                _unitOfWork.GroupUserRepository.RemoveItem(UserGroup);
                            }

                            List<int> GroupsToAdd = ModelGroups.Except(UserGroups).ToList();
                            foreach (var GroupId in GroupsToAdd)
                            {
                                TLIgroupUser GroupUser = new TLIgroupUser();
                                GroupUser.userId = UserEntity.Id;
                                GroupUser.groupId = GroupId;
                                _unitOfWork.GroupUserRepository.Add(GroupUser);
                            }

                            await _unitOfWork.SaveChangesAsync();

                        }
                        else if (model.UserType == 1)
                        {
                            List<string> AllUserPermissionsInDB = _unitOfWork.UserPermissionssRepository
                             .GetWhere(x => x.UserId == model.Id && x.Delete == false && x.Active == true).Select(x => x.PageUrl).ToList();

                            var DeletePermissions = _unitOfWork.UserPermissionssRepository.GetWhere(x => x.UserId == model.Id);
                            _unitOfWork.UserPermissionssRepository.RemoveRangeItems(DeletePermissions);
                            await _unitOfWork.SaveChangesAsync();

                            if (model.permissions != null)
                            {
                                await InsertPermissionsAsync(connection, model.Id, model.permissions);
                            }
                            await _unitOfWork.SaveChangesAsync();
                            List<int> UserGroups = _unitOfWork.GroupUserRepository.GetWhere(x =>
                                x.userId == model.Id).Select(x => x.groupId).Distinct().ToList();
                            List<int> ModelGroups = model.Groups.Select(x => x.Id).ToList();

                            List<int> GroupsToDelete = UserGroups.Except(ModelGroups).ToList();
                            foreach (var GroupId in GroupsToDelete)
                            {
                                TLIgroupUser UserGroup = _unitOfWork.GroupUserRepository.GetWhereFirst(u => u.groupId == GroupId && u.userId == model.Id);
                                _unitOfWork.GroupUserRepository.RemoveItem(UserGroup);
                            }

                            List<int> GroupsToAdd = ModelGroups.Except(UserGroups).ToList();
                            foreach (var GroupId in GroupsToAdd)
                            {
                                TLIgroupUser GroupUser = new TLIgroupUser();
                                GroupUser.userId = model.Id;
                                GroupUser.groupId = GroupId;
                                _unitOfWork.GroupUserRepository.Add(GroupUser);
                            }

                            await _unitOfWork.SaveChangesAsync();

                        }
                        transaction.Commit();
                        return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                    catch (Exception err)
                    {
                        return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
            }

        }

        //Function to check if the user not exist in active directory and database
        public Response<bool> ValidateUserInAdAndDb(string UserName, string domain)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind))
                {
                    UserPrincipal ValidateUserNameInAd = new UserPrincipal(context);
                    if (context != null)
                    {
                        //ValidateUserNameInAd = null;
                        ValidateUserNameInAd = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName);
                    }
                    TLIuser ValidateUserNameInDatabase = _unitOfWork.UserRepository.GetWhereFirst(u => u.UserName.ToLower() == UserName.ToLower());
                    if (ValidateUserNameInAd == null && ValidateUserNameInDatabase == null)
                    {
                        return new Response<bool>(true, true, null, "", (int)Helpers.Constants.ApiReturnCode.success);
                    }
                    else
                    {
                        return new Response<bool>(true, false, null, $"This User Name {UserName} Is Already Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                // PrincipalContext context = new PrincipalContext(ContextType.Domain, domain);
            }
            catch (Exception err)
            {
                return new Response<bool>(true, false, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<ChangePasswordViewModel>> ChangePassword(ChangePasswordViewModel View)
        {
            try
            {
                TLIuser User = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == View.Id);
                byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                View.OldPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: View.OldPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                if (View.OldPassword == User.Password)
                {
                    User.ChangedPasswordDate = DateTime.Now;
                    _unitOfWork.UserRepository.Update(User);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<ChangePasswordViewModel>();
                }
                else
                {
                    return new Response<ChangePasswordViewModel>(true, null, null, "The old password isn't valid try again", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                return new Response<ChangePasswordViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<ForgetPassword>> ForgetPassword(ForgetPassword password)
        {
            try
            {
                TLIuser User = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().Where(x => x.Id == password.Id).FirstOrDefault();
                byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                User.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: User.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                User.ChangedPasswordDate = DateTime.Now;
                _unitOfWork.UserRepository.Update(User);
                await _unitOfWork.SaveChangesAsync();
                return new Response<ForgetPassword>();
            }
            catch (Exception err)
            {
                return new Response<ForgetPassword>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<bool?> CheckPasswordExpiryDate(int Id)
        {
            try
            {
                //var User = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().Where(x => x.Id == Id).FirstOrDefault();
                TLIuser User = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == Id);
                if (User == null)
                {
                    return new Response<bool?>(true, null, null, "User not existed", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    if (User.ChangedPasswordDate == null)
                    {
                        return new Response<bool?>(true, null, null, "You have to change your password", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        DateTime date = (DateTime)User.ChangedPasswordDate;
                        if (date.AddDays(90) < DateTime.Now)
                        {
                            return new Response<bool?>(true, false, null, "Your passowrd is out of date", (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            return new Response<bool?>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                return new Response<bool?>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<UserWithoutGroupViewModel>> GetAllUserWithoutGroup()
        {
            try
            {
                List<UserWithoutGroupViewModel> userWithoutGroupViewModel = new List<UserWithoutGroupViewModel>();
                List<TLIuser> user = _unitOfWork.UserRepository.GetAllWithoutCount().ToList();
                List<TLIgroupUser> GroupUser = _unitOfWork.GroupUserRepository.GetAllWithoutCount().ToList();
                List<TLIuser> userIdsNotInGroupUser = user
                .Where(user => !GroupUser.Any(groupUser => groupUser.userId == user.Id))
                .Select(user => user)
                .ToList();
                foreach (var item in userIdsNotInGroupUser)
                {
                    userWithoutGroupViewModel.Add(new UserWithoutGroupViewModel()
                    {
                        Id = item.Id,
                        FirstName = item.FirstName,
                        MiddleName = item.MiddleName,
                        LastName = item.LastName,
                        Email = item.Email,
                        MobileNumber = item.MobileNumber,
                        UserName = item.UserName,
                        Password = item.Password,
                        Domain = item.Domain,
                        AdGUID = item.AdGUID,
                        UserType = item.UserType,
                        Active = item.Active,
                        Deleted = item.Deleted,
                        ConfirmationCode = item.ConfirmationCode,
                        ValidateAccount = item.ValidateAccount,
                        Permissions = _unitOfWork.UserPermissionssRepository.GetWhere(x => x.UserId == item.Id).Select(x => x.PageUrl).ToList()

                    });
                }

                return new Response<List<UserWithoutGroupViewModel>>(true, userWithoutGroupViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, userWithoutGroupViewModel.Count());
            }
            catch (Exception err)
            {

                return new Response<List<UserWithoutGroupViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }


        }
        public Response<string> EncryptAllUserPassword(string UserName)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var AllUser = _unitOfWork.UserRepository.GetWhere(x => x.Password == null);
                    foreach (var item in AllUser)
                    {
                        item.Password = "Password123@";
                        _unitOfWork.UserRepository.Update(item);
                    }
                    _unitOfWork.SaveChanges();

                    var AllUsers = _unitOfWork.UserRepository.GetWhere(x => x.UserName != UserName);
                    foreach (var item1 in AllUsers)
                    {
                        item1.Password = Encrypt(item1.Password);
                        _unitOfWork.UserRepository.Update(item1);

                    }
                    _unitOfWork.SaveChanges();
                    scope.Complete();
                    return new Response<string>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception ex)
                {
                    return new Response<string>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }


            }
        }
        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                string Key = "9443a09ae2e433750868beaeec0fd681";
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
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
        public List<int?> GetParentGroup(List<int> GroupId)
        {
            List<int?> Childs = new List<int?>();
            List<int?> currentGroup = _unitOfWork.GroupRepository.GetWhere(x => GroupId.Any(y => y == x.Id) && !x.Deleted && x.Active).Select(x => x.Id).Cast<int?>().ToList();
            List<TLIgroup> Group = _unitOfWork.GroupRepository.GetWhere(x => x.Active && !x.Deleted).ToList();
            while (currentGroup.Count != 0)
            {
                Childs.AddRange(currentGroup);

                currentGroup = Group.Where(x => currentGroup.Any(y => y == x.Id && x.ParentId != null)).Select(x => x.ParentId).ToList();
            }
            return Childs;
        }
        public Response<string> DeletePassword()
        {
            try
            {
                var User = _unitOfWork.UserRepository.GetWhere(x => x.UserType == 1);
                foreach (var item in User)
                {
                    item.Password = null;
                    _unitOfWork.UserRepository.Update(item);
                    _unitOfWork.SaveChanges();
                }
                return new Response<string>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception ex)
            {

                return new Response<string>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public async Task<CallTLIResponse> GetEmailByUserId(int UserId)
        {
            CallTLIResponse callTLIResponse = new CallTLIResponse();
            try
            {
                var Email = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == UserId && x.Active && !x.Deleted);
                if (Email != null)
                {
                    callTLIResponse.result = Email.Email;
                }
                else
                {
                    callTLIResponse.errorMessage = "This User Is Not Active";
                }
            }
            catch (Exception err)
            {

                callTLIResponse.errorMessage = err.Message;
            }
            return callTLIResponse;
        }
        public async Task<CallTLIResponse> GetNameByUserId(int UserId)
        {
            CallTLIResponse callTLIResponse = new CallTLIResponse();
            try
            {

                var UserName = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == UserId && x.Active && !x.Deleted);
                if (UserName != null)
                {
                    callTLIResponse.result = UserName.UserName;
                }
                else
                {
                    callTLIResponse.errorMessage = "This User Is Not Active";
                }
            }
            catch (Exception err)
            {

                callTLIResponse.errorMessage = err.Message;
            }
            return callTLIResponse;
        }

        public class CallTLIResponse
        {

            public string result { get; set; }
            public object count { get; set; }
            public object errorMessage { get; set; }


        }

        public bool GetSession(int UserId, string Ip)
        {
            var SessionInfo = _dbContext.TLIsession.FirstOrDefault(x => x.UserId == UserId && x.IP == Ip && x.LoginDate < DateTime.Now);
            if (SessionInfo != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public new Response<string> ChangePassword(int UserId, string NewPassword)
        {

            var UserInfo = _dbContext.TLIuser.FirstOrDefault(x => x.Id == UserId);

            if (UserInfo != null)
            {

                UserInfo.Password = NewPassword; // تأكد أن حقل "Password" موجود في الكيان
                _dbContext.SaveChanges(); // حفظ التغييرات

                // إرجاع استجابة النجاح
                return new Response<string>(
                    true,
                    "Password changed successfully.", // رسالة النجاح
                    null,
                    null,
                    (int)Helpers.Constants.ApiReturnCode.success
                );
            }
            else
            {
                // إرجاع استجابة الفشل
                return new Response<string>(
                    false,
                    "User not found.", // رسالة الفشل
                    null,
                    null,
                    (int)Helpers.Constants.ApiReturnCode.fail
                );
            }
        }

        public new Response<string> ResetPassword(int UserId, string NewPassword)
        {
            // التحقق من إدخال كلمة المرور الجديدة
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                return new Response<string>(
                    false,
                    "New password cannot be empty.", // رسالة خطأ في حال كلمة المرور فارغة
                    null,
                    null,
                    (int)Helpers.Constants.ApiReturnCode.fail
                );
            }

            // البحث عن المستخدم في قاعدة البيانات
            var UserInfo = _dbContext.TLIuser.FirstOrDefault(x => x.Id == UserId);

            if (UserInfo != null)
            {

                UserInfo.Password = NewPassword;
                // حفظ التغييرات
                _dbContext.SaveChanges();

                // إرجاع استجابة النجاح
                return new Response<string>(
                    true,
                    "Password reset successfully.", // رسالة النجاح
                    null,
                    null,
                    (int)Helpers.Constants.ApiReturnCode.success
                );
            }
            else
            {
                // إرجاع استجابة الفشل
                return new Response<string>(
                    false,
                    "User not found.", // رسالة الفشل
                    null,
                    null,
                    (int)Helpers.Constants.ApiReturnCode.fail
                );
            }
        }

        public new Response<string> AddAnAuthorizedAccessToSecurityLog(int userId, string Title,string Message)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    TLISecurityLogs tLISercurityLogs = new TLISecurityLogs()
                    {
                        Date = DateTime.Now,
                        UserId = userId,
                        Message = Message,
                        Title = Title,
                        ControllerName ="User",
                        FunctionName = Title,
                    };
                    _dbContext.TLISecurityLogs.Add(tLISercurityLogs);
                    _dbContext.SaveChanges();

                    scope.Complete();
                    return new Response<string>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {

                    return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }    
               
            }
        }


        public async Task<Response<IEnumerable<TLISercurityLogsDto>>> GetSecurityLogs(FilterRequest filterRequest)
        {
            try
            {
                var query = _dbContext.TLISecurityLogs.AsQueryable();

                // Get total count before filtering
                var totalCount = query.Count();

                // Apply filters
                query = ApplyFilter(query, filterRequest);

                // Get filtered count
                var filteredCount = query.Count();

                // Apply pagination
                if (filterRequest.First.HasValue && filterRequest.Rows.HasValue)
                {
                    query = query.Skip(filterRequest.First.Value).Take(filterRequest.Rows.Value);
                }

                // Map data to ViewModel
                var result = await query.Select(q => new TLISercurityLogsDto
                {
                    Id = q.Id,
                    Date = q.Date,
                    UserName = q.User.UserName,
                    ControllerName = q.ControllerName,
                    FunctionName = q.FunctionName,
                    UserType = q.User.UserType == 0 ? "InternalUser" : "ExternalUser", // Convert int to string
                    ResponseStatus = q.ResponseStatus,
                    Message = q.Message
                }).ToListAsync();

                return new Response<IEnumerable<TLISercurityLogsDto>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, totalCount);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<TLISercurityLogsDto>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        private IQueryable<T> ApplyFilter<T>(
         IQueryable<T> query,
         FilterRequest filterRequest)
        {
            if (filterRequest == null || filterRequest.Filters == null || !filterRequest.Filters.Any())
                return query;

            foreach (var filter in filterRequest.Filters)
            {
                string fieldName = filter.Key;
                var filterValue = filter.Value.Value;
                var matchMode = filter.Value.MatchMode;

                // التحقق إذا كانت القيمة من نوع JsonElement
                if (filterValue is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Null || string.IsNullOrEmpty(jsonElement.GetString()))
                    {
                        // إذا كانت القيمة null أو فارغة، يتم تجاهل هذا الفلتر
                        continue;
                    }

                    // تحويل JsonElement إلى القيمة الفعلية إذا كانت صالحة
                    filterValue = jsonElement.GetString();
                }

                // التحقق من إذا كانت القيمة null أو فارغة بعد التحويل
                if (filterValue == null || string.IsNullOrEmpty(filterValue.ToString()))
                {
                    // تجاهل الفلاتر التي تحتوي على قيم فارغة أو null
                    continue;
                }

                if (fieldName.ToLower() == "controllername" || fieldName.ToLower() == "functionname" || fieldName.ToLower() == "message" || fieldName.ToLower() == "title")
                {
                    query = ApplyStringFilter(query, fieldName, filterValue, matchMode);
                }
                else if (fieldName.ToLower() == "date")
                {
                    query = ApplyDateFilter(query, filterValue, matchMode);
                }
                else if (fieldName.ToLower() == "username")
                {
                    query = ApplyStringFilter(query, "User.UserName", filterValue, matchMode);
                }
                else if (fieldName.ToLower() == "usertype")
                {
                    query = ApplyIntFilter(query, fieldName, filterValue, matchMode);
                }

            }

            return query;
        }
        private IQueryable<T> ApplyIntFilter<T>(
            IQueryable<T> query,
            string fieldName,
            object filterValue,
            string matchMode)
        {
            if (filterValue == null || !int.TryParse(filterValue.ToString(), out var filterIntValue)) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, fieldName);
            var constant = Expression.Constant(filterIntValue);

            Expression body = matchMode switch
            {
                FilterMatchMode.EQUALS => Expression.Equal(property, constant),
                FilterMatchMode.NOT_EQUALS => Expression.NotEqual(property, constant),
                FilterMatchMode.GREATER_THAN => Expression.GreaterThan(property, constant),
                FilterMatchMode.LESS_THAN => Expression.LessThan(property, constant),
            
                _ => null
            };

            if (body == null) return query;

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(predicate);
        }

        private IQueryable<T> ApplyStringFilter<T>(
         IQueryable<T> query,
         string fieldName,
         object filterValue,
         string matchMode)
        {
            if (filterValue == null) return query;

            string filterText = filterValue.ToString();
            if (string.IsNullOrEmpty(filterText)) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, fieldName);
            var constant = Expression.Constant(filterText.ToLower()); // تحويل النص إلى أحرف صغيرة

            // تحويل الخاصية أيضا إلى أحرف صغيرة لكي تكون المقارنة غير حساسة لحالة الأحرف
            var propertyLower = Expression.Call(property, "ToLower", null);

            Expression body = matchMode switch
            {
                FilterMatchMode.STARTS_WITH => Expression.Call(propertyLower, "StartsWith", null, constant),
                FilterMatchMode.CONTAINS => Expression.Call(propertyLower, "Contains", null, constant),
                FilterMatchMode.NOT_CONTAINS => Expression.Not(Expression.Call(propertyLower, "Contains", null, constant)),
                FilterMatchMode.ENDS_WITH => Expression.Call(propertyLower, "EndsWith", null, constant),
                FilterMatchMode.EQUALS => Expression.Equal(propertyLower, constant),
                FilterMatchMode.NOT_EQUALS => Expression.NotEqual(propertyLower, constant),
                _ => null
            };

            if (body == null) return query;

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(predicate);
        }
        private IQueryable<T> ApplyDateFilter<T>(
            IQueryable<T> query,
            object filterValue,
            string matchMode)
        {
            if (filterValue == null) return query;

            if (!DateTime.TryParse(filterValue.ToString(), out var filterDate)) return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, "Date");
            var constant = Expression.Constant(filterDate);

            Expression body = matchMode switch
            {
                FilterMatchMode.DATE_IS => Expression.Equal(property, constant),
                FilterMatchMode.DATE_IS_NOT => Expression.NotEqual(property, constant),
                FilterMatchMode.DATE_BEFORE => Expression.LessThan(property, constant),
                FilterMatchMode.DATE_AFTER => Expression.GreaterThan(property, constant),
                _ => null
            };

            if (body == null) return query;

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(predicate);
        }

        // مثال لطريقة تشفير كلمة المرور
        private string EncryptPassword(string password)
        {
            // يمكنك استبدال هذا التشفير بخوارزمية أكثر أمانًا مثل BCrypt
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        //private string CryptPassword(string password)
        //{
        //    byte[] plaintext = Encoding.UTF8.GetBytes(password);
        //    byte[] ciphertext;

        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Key = key;
        //        aes.IV = iv;

        //        using (ICryptoTransform encryptor = aes.CreateEncryptor())
        //        {
        //            ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
        //        }
        //    }

        //    return Convert.ToBase64String(ciphertext);


        //}

        //private string DecryptPassword(string CryptPassword)
        //{
        //    try
        //    {

        //        byte[] ciphertext = Convert.FromBase64String(CryptPassword);
        //        byte[] plaintext;

        //        using (Aes aes = Aes.Create())
        //        {
        //            aes.Key = key;
        //            aes.IV = iv;

        //            using (ICryptoTransform decryptor = aes.CreateDecryptor())
        //            {
        //                plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        //            }
        //        }

        //        return Encoding.UTF8.GetString(plaintext);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}


    }
}
