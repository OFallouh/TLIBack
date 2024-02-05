using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;


namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
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
        [HttpPost("AddInternalUser")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> AddInternalUser(string UserName, [FromBody] List<string> Permissions)
        {
            //var UserId = HttpContext.Session.GetString("UserId");
            var domain = _configuration["Domain"];
            var response = await _unitOfWorkService.UserService.AddInternalUser(UserName, Permissions, domain);
            return Ok(response);
        }
        [HttpPost("Updateuser")]
        [ProducesResponseType(200, Type = typeof(EditUserViewModel))]
        public async Task<IActionResult> Updateuser(EditUserViewModel model)
        {
            if (TryValidateModel(User, nameof(EditUserViewModel)))
            {
                //var UserId = HttpContext.Session.GetString("UserId");
                var response = await _unitOfWorkService.UserService.Updateuser(model);
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
        [HttpPost("ChangePassword")]
        [ProducesResponseType(200, Type = typeof(ChangePasswordViewModel))]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel View)
        {
            if (TryValidateModel(View, nameof(ChangePasswordViewModel)))
            {
                var response = await _unitOfWorkService.UserService.ChangePassword(View);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ChangePasswordViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
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
        [HttpPost("DeactivateUser")]
        [ProducesResponseType(200, Type = typeof(EditUserViewModel))]
        public async Task<IActionResult> DeactivateUser(int UserId)
        {
             //var UserId = HttpContext.Session.GetString("UserId");
             var response = await _unitOfWorkService.UserService.DeactivateUser(UserId);
             return Ok(response);
        }
        [HttpPost("SendConfirmationCode")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult SendConfirmationCode(string UserEmail, int? UserId)
        {
            int? Uservalue = UserId != null ? UserId : null;
            var response = _unitOfWorkService.UserService.SendConfirmationCode(UserEmail, Uservalue);
            return Ok(response);
        }
        [HttpPost("ValidateEmail")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult ValidateEmail(string UserConfirmCode, int UserId)
        {
            var response = _unitOfWorkService.UserService.ValidateEmail(UserConfirmCode, UserId);
            return Ok(response);
        }
        [HttpPost("AddExternalUser")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> AddExternalUser(AddUserViewModel User)
        {
            if(TryValidateModel(User, nameof(AddUserViewModel)))
            {
                //var UserId = Guid.Parse(HttpContext.Session.GetString("UserId"));
                var domain = _configuration["Domain"];
                var response = await _unitOfWorkService.UserService.AddExternalUser(User, domain);
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
        [HttpGet("getAll/{GroupName}")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetUsersByGroupName(string GroupName)
        {
            var domain = _configuration["Domain"];
            var response = _unitOfWorkService.UserService.GetUsersByGroupName(GroupName, domain);
            return Ok(response);
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAll([FromBody]List<FilterObjectList> filters, [FromQuery]ParameterPagination parameter)
        {
            var response = _unitOfWorkService.UserService.GetAll(filters, parameter);
            return Ok(response);
        }
        [HttpPost("GetAllExternalUsers")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetAllExternalUsers(string UserName, [FromQuery] ParameterPagination parameter)
        {
            var response = await _unitOfWorkService.UserService.GetAllExternalUsers(UserName, parameter);
            return Ok(response);
        }
        [HttpPost("GetExternalUsersByUserName")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetExternalUsersByUserName(string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response =  _unitOfWorkService.UserService.GetExternalUsersByName(UserName, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetAllUsers_WFVersion")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAllUsers_WFVersion(string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response1 = _unitOfWorkService.UserService.GetExternalUsersByName(UserName, parameterPagination);
            var response2 = _unitOfWorkService.UserService.GetInternalUsersByName(UserName, parameterPagination);
            var Data = response1.Data.Concat(response2.Data).ToList();
            return Ok(Data);
        }
        [HttpPost("GetAllInternalUsers")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetAllInternalUsers(string UserName, [FromQuery]ParameterPagination parameter)
        {
            var response = await _unitOfWorkService.UserService.GetAllInternalUsers(UserName, parameter);
            return Ok(response);
        }
        [HttpPost("GetInternalUsersByUserName")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetInternalUsersByUserName(string UserName, [FromQuery] ParameterPagination parameter)
        {
            var response = _unitOfWorkService.UserService.GetInternalUsersByName(UserName, parameter);
            return Ok(response);
        }
        [HttpPost("GetUsersByUserType")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetUsersByUserType(int UserTypeId, string UserName, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.UserService.GetUsersByUserType(UserTypeId, UserName, parameterPagination);
            return Ok(response);
        }
        [HttpGet("GetById/{Id}")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetById(int Id)
        {
            var response = await _unitOfWorkService.UserService.GetUserById(Id);
            return Ok(response);
        }
        [HttpGet("CheckPasswordExpiryDate/{Id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult CheckPasswordExpiryDate(int Id)
        {
            var response = _unitOfWorkService.UserService.CheckPasswordExpiryDate(Id);
            return Ok(response);
        }
        [HttpGet("GetAllUserWithoutGroup")]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public IActionResult GetAllUserWithoutGroup()
        {
            var response = _unitOfWorkService.UserService.GetAllUserWithoutGroup();
            return Ok(response);
        }
        [HttpPost("EncryptAllUserPassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public async Task<IActionResult> EncryptAllUserPassword(string UserName)
        {
            var response = _unitOfWorkService.UserService.EncryptAllUserPassword(UserName);
            return Ok(response);
        }
        [HttpGet("DeletePassword")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult DeletePassword()
        {
            var response = _unitOfWorkService.UserService.DeletePassword();
            return Ok(response);
        }
    }
}