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
        public async Task<Response<UserViewModel>> AddExternalUser(AddUserViewModel model, string domain)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    Response<bool> test = ValidateUserInAdAndDb(model.UserName, domain);
                    if (test.Data == true)
                    {
                        byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                        //model.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        //    password: model.Password,
                        //    salt: salt,
                        //    prf: KeyDerivationPrf.HMACSHA256,
                        //    iterationCount: 100000,
                        //    numBytesRequested: 256 / 8));

                         model.Password = EncryptPassword(model.Password);

                        TLIuser UserEntity = _mapper.Map<TLIuser>(model);

                        //Response<string> CheckEmail = SendConfirmationCode(model.Email, null);
                        //if (!string.IsNullOrEmpty(CheckEmail.Data))
                        //{
                        //    UserEntity.ConfirmationCode = CheckEmail.Data;
                        //}
                        //else
                        //{
                        //    return new Response<UserViewModel>(true, null, null, CheckEmail.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
                        //}
                        UserEntity.ValidateAccount = false;
                        UserEntity.UserType = 2;
                        await _unitOfWork.UserRepository.AddAsync(UserEntity);
                        await _unitOfWork.SaveChangesAsync();
                        if (model.Permissions != null)
                        {
                            var userPermissionsList = new List<TLIuser_Permissions>();
                            foreach (var Permission in model.Permissions)
                            {
                                TLIuser_Permissions UserPermission = new TLIuser_Permissions();
                                UserPermission = new TLIuser_Permissions()
                                {
                                    UserId = UserEntity.Id,
                                    PageUrl = Permission,
                                    Active = true,
                                    Delete = false,
                                    user = UserEntity
                                };
                                userPermissionsList.Add(UserPermission);

                            }
                            _dbContext.TLIuser_Permissions.AddRange(userPermissionsList);
                            _dbContext.SaveChanges();
                        }
                        if (model.Groups != null)
                        {
                            List<int> GroupsIds = model.Groups.Select(x => x.Id).ToList();
                            foreach (int GroupId in GroupsIds)
                            {
                                if (GroupId > 0)
                                {
                                    TLIgroupUser GroupUser = new TLIgroupUser();
                                    GroupUser.groupId = GroupId;
                                    GroupUser.userId = UserEntity.Id;
                                    GroupUser.user = UserEntity;
                                    _unitOfWork.GroupUserRepository.Add(GroupUser);
                                }
                            }
                        }
                        _dbContext.SaveChanges();
                        transaction.Complete();
                        return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
                    }
                    else
                    {
                        return new Response<UserViewModel>(false, null, null, test.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
                    }
                }
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
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
        public async Task<Response<UserViewModel>> AddInternalUser(string UserName, List<String> Permissions, string domain)
        {
            try
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
                            TLIuser user = new TLIuser();
                            user.FirstName = principal.Name.Replace($" {principal.Surname}", "");
                            user.MiddleName = principal.MiddleName;
                            user.LastName = principal.Surname;
                            user.Email = principal.EmailAddress;
                            user.MobileNumber = principal.VoiceTelephoneNumber;
                            user.UserName = principal.SamAccountName;
                            var tliuser = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName == UserName && !x.Deleted);
                            if (tliuser != null)
                            {
                                return new Response<UserViewModel>(true, null, null, $"This User {UserName} is Already Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            user.Domain = null;
                            user.AdGUID = principal.Guid.ToString();
                            user.UserType = 1;
                            await _unitOfWork.UserRepository.AddAsync(user);
                            await _unitOfWork.SaveChangesAsync();

                            if (Permissions != null)
                            {
                                var userPermissionsList = new List<TLIuser_Permissions>();
                                foreach (var Permission in Permissions)
                                {
                                    TLIuser_Permissions UserPermission = new TLIuser_Permissions();
                                    UserPermission = new TLIuser_Permissions()
                                    {
                                        UserId = user.Id,
                                        PageUrl = Permission,
                                        Active = true,
                                        Delete = false,
                                        user = user
                                    };
                                    userPermissionsList.Add(UserPermission);

                                }
                                _dbContext.TLIuser_Permissions.AddRange(userPermissionsList);
                                _dbContext.SaveChanges();
                            }

                        }
                        else
                        {
                            return new Response<UserViewModel>(false, null, null, $"This User {UserName} is Not Exist in AD", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function enable or disable user depened on user status
        public async Task<Response<UserViewModel>> DeactivateUser(int UserId)
        {
            try
            {
                TLIuser User = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == UserId && !x.Deleted);
                User.Active = !(User.Active);
                _unitOfWork.UserRepository.Update(User);
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
            try
            {
                int count = 0;
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                var Users = _unitOfWork.UserRepository.GetAllIncludeMultipleWithCondition(parameter, filters, condition, out count, null).ToList();
                return new Response<List<UserViewModel>>(true, _mapper.Map<List<UserViewModel>>(Users), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<UserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
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
                var newPermissionsViewModels = new List<string>();
                UserViewModel User = _mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetWhereFirst(x => x.Id == Id && !x.Deleted));
                if (User != null)
                {
                    User.Password = DecryptPassword(User.Password);
                    var UserPermissions = _unitOfWork.UserPermissionssRepository.GetWhere(x =>
                    x.UserId == Id && x.Active == true && x.Delete == false).Select(x => x.PageUrl).ToList();
                    var GroupUser = _unitOfWork.GroupUserRepository.GetWhere(x => x.userId == Id).Select(x => x.groupId).ToList();
                    var RoleGroup = _unitOfWork.GroupRoleRepository.GetIncludeWhere(x => GroupUser.Any(y => y == x.groupId)).Select(x => x.roleId).ToList();
                    var Rolepermissions = _unitOfWork.RolePermissionsRepository.GetIncludeWhere(x => RoleGroup.Any(y => y == x.RoleId) && !x.Delete && x.Active).Select(x => x.PageUrl).ToList();
                    User.Groups = await _unitOfWork.GroupUserRepository.GetAllAsQueryable().AsNoTracking()
                        .Where(x => x.userId == User.Id).Select(g => new GroupNamesViewModel(g.groupId, g.group.Name)).ToListAsync();
                    newPermissionsViewModels.AddRange(UserPermissions);
                    newPermissionsViewModels.AddRange(Rolepermissions);
                    User.Permissions = newPermissionsViewModels;
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
        public async Task<Response<UserViewModel>> Updateuser(EditUserViewModel model)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {

                    TLIuser UserEntity = _mapper.Map<TLIuser>(model);

                    UserEntity.Password = null;

                    string OldPassword = _unitOfWork.UserRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Password;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

                        string CheckPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: model.Password,
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA256,
                            iterationCount: 100000,
                            numBytesRequested: 256 / 8));

                        if (CheckPassword != OldPassword)
                            UserEntity.Password = CheckPassword;
                    }
                    else
                    {
                        UserEntity.Password = OldPassword;
                    }
                    _unitOfWork.UserRepository.Update(UserEntity);


                    List<string> AllUserPermissionsInDB = _unitOfWork.UserPermissionssRepository
                      .GetWhere(x => x.UserId == model.Id && x.Delete == false && x.Active == true).Select(x => x.PageUrl).ToList();

                    var Exi = AllUserPermissionsInDB.Except(model.permissions).Union(model.permissions).Except(AllUserPermissionsInDB).Distinct().ToList();
                    foreach (var item in Exi)
                    {
                        TLIuser_Permissions tLIuserPermissions = new TLIuser_Permissions();
                        tLIuserPermissions = new TLIuser_Permissions()
                        {
                            UserId = model.Id,
                            PageUrl = item,
                            Active = true,
                            Delete = false
                        };
                        _unitOfWork.UserPermissionssRepository.Add(tLIuserPermissions);
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
                    transaction.Complete();

                    return new Response<UserViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<UserViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
                    TLIuser ValidateUserNameInDatabase = _unitOfWork.UserRepository.GetWhereFirst(u => u.UserName == UserName);
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

        //////private string CryptPassword(string password)
        //////{
        //////    byte[] plaintext = Encoding.UTF8.GetBytes(password);
        //////    byte[] ciphertext;

        //////    using (Aes aes = Aes.Create())
        //////    {
        //////        aes.Key = key;
        //////        aes.IV = iv;
        //////        aes.Padding = PaddingMode.PKCS7;
        //////        using (ICryptoTransform encryptor = aes.CreateEncryptor())
        //////        {
        //////            ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
        //////        }
        //////    }

        //////    return Convert.ToBase64String(ciphertext);


        //////}


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
        //            aes.Padding = PaddingMode.PKCS7;
        //            using (ICryptoTransform decryptor = aes.CreateDecryptor())
        //            {
        //                plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        //            }
        //        }

        //        int paddingLength = plaintext[plaintext.Length - 1];
        //        byte[] unpaddedPlaintext = new byte[plaintext.Length - paddingLength];
        //        Array.Copy(plaintext, unpaddedPlaintext, unpaddedPlaintext.Length);

        //        return Encoding.UTF8.GetString(unpaddedPlaintext);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        private static readonly byte[] keys = Generate256BitKey();
        public static string EncryptPassword(string password)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keys;
                aesAlg.GenerateIV(); // Generate a random IV
                byte[] iv = aesAlg.IV; // Save the IV for later decryption

                aesAlg.Mode = CipherMode.CFB; // You can change the mode as per your requirement.
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(password);
                        }
                    }
                    // Combine IV and ciphertext for storage, and return as Base64 string
                    byte[] ivAndCiphertext = new byte[iv.Length + msEncrypt.ToArray().Length];
                    Array.Copy(iv, ivAndCiphertext, iv.Length);
                    Array.Copy(msEncrypt.ToArray(), 0, ivAndCiphertext, iv.Length, msEncrypt.ToArray().Length);
                    return Convert.ToBase64String(ivAndCiphertext);
                }
            }
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            byte[] ivAndCiphertext = Convert.FromBase64String(encryptedPassword);
            byte[] iv = new byte[16];
            byte[] ciphertext = new byte[ivAndCiphertext.Length - 16];

            Array.Copy(ivAndCiphertext, iv, 16);
            Array.Copy(ivAndCiphertext, 16, ciphertext, 0, ciphertext.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keys;
                aesAlg.IV = iv;

                aesAlg.Mode = CipherMode.CFB; // You must use the same mode as used for encryption.
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
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

        private static byte[] Generate256BitKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 256 bits
                rng.GetBytes(key);
                return key;
            }
        }

    }
}
