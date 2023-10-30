using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_Service.IService
{
    public interface IUserService
    {
        Task<Response<UserViewModel>> AddInternalUser(string UserName, List<string> Permissions, string domain);
        Response<List<UserViewModel>> GetUsersByGroupName(string GroupName, string domain);
        Task<Response<UserViewModel>> AddExternalUser(AddUserViewModel model, string domain);
        Response<bool> ValidateUserInAdAndDb(string UserName, string domain);
        Task<Response<List<UserViewModel>>> GetAllInternalUsers(string UserName, ParameterPagination parameter);
        Task<Response<List<UserViewModel>>> GetAllExternalUsers(string UserName, ParameterPagination parameter);
        Task<Response<UserViewModel>> GetUserById(int Id);
        Task<Response<UserViewModel>> Updateuser(EditUserViewModel model);
        Task<Response<UserViewModel>> DeactivateUser(int UserId);
        Response<List<UserViewModel>> GetAll(List<FilterObjectList> filters, ParameterPagination parameter);
        Task<Response<ChangePasswordViewModel>> ChangePassword(ChangePasswordViewModel View);
        Task<Response<ForgetPassword>> ForgetPassword(ForgetPassword password);
        Response<bool?> CheckPasswordExpiryDate(int Id);
        Response<string> SendConfirmationCode(string UserEmail, int? UserId);
        Task<Response<string>> ValidateEmail(string UserConfirmCode, int UserId);
        Response<List<UserViewModel>> GetExternalUsersByName(string UserName, ParameterPagination parameterPagination);
        Response<List<UserViewModel>> GetInternalUsersByName(string UserName, ParameterPagination parameter);
        Response<List<UserViewModel>> GetUsersByUserType(int UserTypeId, string UserName, ParameterPagination parameterPagination);
        Response<List<UserWithoutGroupViewModel>> GetAllUserWithoutGroup();
        Response<string> EncryptAllUserPassword(string UserName);
    }
}
