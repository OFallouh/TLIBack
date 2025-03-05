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
using TLIS_DAL.ViewModels.wf;
using static TLIS_Service.Services.SiteService;
using static TLIS_Service.Services.UserService;

namespace TLIS_Service.IService
{
    public interface IUserService
    {
        bool GetSession(int UserId, string Ip);
        Response<UserViewModel> UpdatePermissionsUser(UpdatePermissionDto updatePermissionDto);
        Task<Response<UserViewModel>> AddInternalUser(string UserName, List<string> Permissions, string domain, int UserId);
        Task<Response<List<UserDto>>> GetAllInternalUsers(FilterRequest filterRequest);
        Task<Response<List<UserDto>>> GetAllExternalUsers( FilterRequest filterRequest);
        Response<List<UserViewModel>> GetUsersByGroupName(string GroupName, string domain);
        Task<Response<UserViewModel>> AddExternalUser(AddUserViewModel model, string domain, int UserId);
        Response<bool> ValidateUserInAdAndDb(string UserName, string domain);
        Task<Response<UserViewModel>> GetUserById(int Id);
        Task<Response<UserViewModel>> Updateuser(EditUserViewModel model, int UserId);
        Task<Response<UserViewModel>> DeactivateUser(int UserId, int userid);
        Response<List<UserViewModel>> GetAll(List<FilterObjectList> filters, ParameterPagination parameter);
        //Task<Response<ChangePasswordViewModel>> ChangePassword(ChangePasswordViewModel View);
        Task<Response<ForgetPassword>> ForgetPassword(ForgetPassword password);
        Response<bool?> CheckPasswordExpiryDate(int Id);
        Response<string> SendConfirmationCode(string UserEmail, int? UserId);
        Task<Response<string>> ValidateEmail(string UserConfirmCode, int UserId);
        Response<List<UserViewModel>> GetExternalUsersByName(string UserName, ParameterPagination parameterPagination);
        Response<List<UserViewModel>> GetInternalUsersByName(string UserName, ParameterPagination parameter);
        Response<List<UserViewModel>> GetUsersByUserType(int UserTypeId, string UserName, ParameterPagination parameterPagination);
        Response<List<UserWithoutGroupViewModel>> GetAllUserWithoutGroup();
        Response<string> EncryptAllUserPassword(string UserName);
        Response<string> DeletePassword();
        Task<CallTLIResponse> GetEmailByUserId(int UserId);
        Task<CallTLIResponse> GetNameByUserId(int UserId);
        new Response<string> ChangePassword(int UserId, string NewPassword);
        new Response<string> ResetPassword(int UserId, string NewPassword);
        Task<Response<IEnumerable<TLISercurityLogsDto>>> GetSecurityLogs(FilterRequest filterRequest);
        Response<string> AddAnAuthorizedAccessToSecurityLog(int userId, string Title, string Message);
        Response<string> ClearLogSecurity(string connectionString, string dateFrom = null, string dateTo = null);
        Task<Response<IEnumerable<TLISercurityLogsDto>>> GetSecurityLogsFile( );
        Task<Response<List<string>>> GetOldPermissionsUserById(int Id);
    }
}
