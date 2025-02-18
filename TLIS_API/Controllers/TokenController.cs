using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static Dapper.SqlMapper;
//using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Microsoft.Extensions.Hosting;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_API.Middleware;

namespace TLIS_API.Controllers
{
     [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWork;
        private IConfiguration _config;
        private IHttpContextAccessor _httpContextAccessor;
        public TokenController(IConfiguration config, IUnitOfWorkService unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public IActionResult CreateToken([FromBody] LoginViewModel login)
        {
            try
            {
                if (TryValidateModel(login, nameof(LoginViewModel)))
                {
                    var secretKey = _config["JWT:Key"];
                    var domainName = _config["Domain"];
                    var domainGroup = _config["DomainGroup"];

                    var response = _unitOfWork.TokenService.CreateToken(login, secretKey, domainName, domainGroup);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<LoginViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //-----------------------------------------Internal Syriatel Login-------------------------
        [HttpPost]
        [Route("InternalToken")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public IActionResult CreateInternalToken([FromBody] LoginViewModel login)
        {
            try
            {
                // var identity = WindowsIdentity.GetCurrent();
                // var windowsIdentity = _httpContextAccessor.HttpContext.User.Identity as WindowsIdentity;
                // Get the username from the Windows identity
                //var username = windowsIdentity?.Name;


                //  PrincipalContext context = new PrincipalContext(ContextType.Domain, _config["Domain"], null, ContextOptions.SimpleBind, null, null);
                // var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                // Get the user's name
                // var CurrentWindowsUser = userPrincipal?.SamAccountName;

                //string CurrentWindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                // LoginViewModel login = new LoginViewModel();
                // login.UserName = CurrentWindowsUser;
                if (TryValidateModel(login, nameof(LoginViewModel)))
                {
                    var secretKey = _config["JWT:Key"];
                    var domainName = _config["Domain"];
                    var domainGroup = _config["DomainGroup"];

                    var response = _unitOfWork.TokenService.CreateInternalToken(login, secretKey, domainName, domainGroup);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<LoginViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //--------------------------------------------------Login----------------------------
        [AllowAnonymous]
        [ServiceFilter(typeof(SecurityLogFilter))]
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public IActionResult Login([FromBody] LoginViewModel login)
        {
            try
            {
                if (TryValidateModel(login, nameof(LoginViewModel)))
                {
                    var secretKey = _config["JWT:Key"];
                    var domainName = _config["Domain"];
                    var domainGroup = _config["DomainGroup"];

                    var response = _unitOfWork.TokenService.Login(login, secretKey, domainName, domainGroup);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<LoginViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Route("Logout")]
        [HttpPost]
        public IActionResult Logout(int UserId)
        {
            try
            {
                var response = _unitOfWork.TokenService.Logout(UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}