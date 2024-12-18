using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.SiteService;


namespace TLIS_API.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public UserController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("AddInternalUser")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> AddInternalUser(string UserName, [FromBody] List<string> Permissions)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var domain = _configuration["Domain"];
            var response = await _unitOfWorkService.UserService.AddInternalUser(UserName, Permissions, domain, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("Updateuser")]
        [ProducesResponseType(200, Type = typeof(EditUserViewModel))]
        public async Task<IActionResult> Updateuser(EditUserViewModel model)
        {
            if (TryValidateModel(User, nameof(EditUserViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                string authHeader = HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
                {
                    return Unauthorized();
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return Unauthorized();
                }

                string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
                var userId = Convert.ToInt32(userInfo);
                var response = await _unitOfWorkService.UserService.Updateuser(model, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditUserViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        //[HttpPost("ChangePassword")]
        //[ProducesResponseType(200, Type = typeof(ChangePasswordViewModel))]
        //public async Task<IActionResult> ChangePassword(ChangePasswordViewModel View)
        //{
        //    if (TryValidateModel(View, nameof(ChangePasswordViewModel)))
        //    {
        //        var response = await _unitOfWorkService.UserService.ChangePassword(View);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<ChangePasswordViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ForgetPassword")]
        [ProducesResponseType(200, Type = typeof(ForgetPassword))]
        public async Task<IActionResult> ForgetPassword(ForgetPassword View)
        {
            if (TryValidateModel(View, nameof(ForgetPassword)))
            {
                var response = await _unitOfWorkService.UserService.ForgetPassword(View);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ForgetPassword>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(SecurityLogFilter))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("DeactivateUser")]
        [ProducesResponseType(200, Type = typeof(EditUserViewModel))]
        public async Task<IActionResult> DeactivateUser(int UserId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = await _unitOfWorkService.UserService.DeactivateUser(UserId, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("SendConfirmationCode")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult SendConfirmationCode(string UserEmail, int? UserId)
        {
            int? Uservalue = UserId != null ? UserId : null;
            var response = _unitOfWorkService.UserService.SendConfirmationCode(UserEmail, Uservalue);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ValidateEmail")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult ValidateEmail(string UserConfirmCode, int UserId)
        {
            var response = _unitOfWorkService.UserService.ValidateEmail(UserConfirmCode, UserId);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("AddExternalUser")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> AddExternalUser(AddUserViewModel User)
        {
            if (TryValidateModel(User, nameof(AddUserViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                string authHeader = HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
                {
                    return Unauthorized();
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return Unauthorized();
                }

                string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
                var userId = Convert.ToInt32(userInfo);
                var domain = _configuration["Domain"];
                var response = await _unitOfWorkService.UserService.AddExternalUser(User, domain, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<UserViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("getAll/{GroupName}")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetUsersByGroupName(string GroupName)
        {
            var domain = _configuration["Domain"];
            var response = _unitOfWorkService.UserService.GetUsersByGroupName(GroupName, domain);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAll")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAll([FromBody] List<FilterObjectList> filters, [FromQuery] ParameterPagination parameter)
        {
            var response = _unitOfWorkService.UserService.GetAll(filters, parameter);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllExternalUsers")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetAllExternalUsers(string UserName, [FromQuery] ParameterPagination parameter)
        {
            var response = await _unitOfWorkService.UserService.GetAllExternalUsers(UserName, parameter);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetExternalUsersByUserName")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetExternalUsersByUserName(string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.UserService.GetExternalUsersByName(UserName, parameterPagination);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllUsers_WFVersion")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAllUsers_WFVersion(string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response1 = _unitOfWorkService.UserService.GetExternalUsersByName(UserName, parameterPagination);
            var response2 = _unitOfWorkService.UserService.GetInternalUsersByName(UserName, parameterPagination);
            var Data = response1.Data.Concat(response2.Data).ToList();
            return Ok(Data);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllInternalUsers")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetAllInternalUsers(string UserName, [FromQuery] ParameterPagination parameter)
        {
            var response = await _unitOfWorkService.UserService.GetAllInternalUsers(UserName, parameter);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetInternalUsersByUserName")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetInternalUsersByUserName(string UserName, [FromQuery] ParameterPagination parameter)
        {
            var response = _unitOfWorkService.UserService.GetInternalUsersByName(UserName, parameter);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetUsersByUserType")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetUsersByUserType(int UserTypeId, string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.UserService.GetUsersByUserType(UserTypeId, UserName, parameterPagination);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [HttpGet("GetById/{Id}")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetById(int Id)
        {
            var response = await _unitOfWorkService.UserService.GetUserById(Id);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("CheckPasswordExpiryDate/{Id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult CheckPasswordExpiryDate(int Id)
        {
            var response = _unitOfWorkService.UserService.CheckPasswordExpiryDate(Id);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllUserWithoutGroup")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAllUserWithoutGroup()
        {
            var response = _unitOfWorkService.UserService.GetAllUserWithoutGroup();
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("EncryptAllUserPassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public async Task<IActionResult> EncryptAllUserPassword(string UserName)
        {
            var response = _unitOfWorkService.UserService.EncryptAllUserPassword(UserName);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("DeletePassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult DeletePassword()
        {
            var response = _unitOfWorkService.UserService.DeletePassword();
            return Ok(response);
        }

        // ------------------------------------------------------------------------
        [ServiceFilter(typeof(SecurityLogFilter))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ChangePassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult ChangePassword(int UserId, string NewPassword)
        {
           
            var response = _unitOfWorkService.UserService.ChangePassword(UserId, NewPassword);
            return Ok(response);
        }
        [ServiceFilter(typeof(SecurityLogFilter))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ResetPassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult ResetPassword( string NewPassword)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = _unitOfWorkService.UserService.ResetPassword(userId, NewPassword);
            return Ok(response);
        }
        [ServiceFilter(typeof(SecurityLogFilter))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllSecurityLogs")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetSecurityLogs([FromBody] FilterRequest request)
        {

            var response = _unitOfWorkService.UserService.GetSecurityLogs(request);
            return Ok(response);
        }
        [ServiceFilter(typeof(LogFilterAttribute))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ClearAllHistory")]
        public IActionResult ClearAllHistory(string dateFrom, string dateTo)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.ClearAllHistory(ConnectionString, dateFrom, dateTo);
            return Ok(response);
        }
        [ServiceFilter(typeof(SecurityLogFilter))]
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("AddAnAuthorizedAccessToSecurityLog")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult AddAnAuthorizedAccessToSecurityLog( string Title, string Message)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var domain = _configuration["Domain"];
            var response = _unitOfWorkService.UserService.AddAnAuthorizedAccessToSecurityLog(userId, Title, Message);
            return Ok(response);
        }
    }
}