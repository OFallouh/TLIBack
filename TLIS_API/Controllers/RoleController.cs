using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TLIS_API.Controllers
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RoleController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        //[Authorize]
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles([FromBody] List<FilterObjectList> filters, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var response = await _unitOfWorkService.RoleService.GetRoles(filters, pageNumber, pageSize);
            return Ok(response);
        }
        [HttpGet("getAllFor_WF")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _unitOfWorkService.RoleService.GetRolesFor_WF();
            return Ok(response.Data);
        }
        [HttpPost("GetRoleByName")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public IActionResult GetRoles(string RoleName)
        {
            var response = _unitOfWorkService.RoleService.GetRoleByName(RoleName);
            return Ok(response);
        }

        [HttpGet("CheckRoleGroups/{RoleId}")]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        public IActionResult CheckRoleGroups(int RoleId)
        {
            var response = _unitOfWorkService.RoleService.CheckRoleGroups(RoleId);
            return Ok(response);
        }

        [HttpPost("AddRolePermissionList")]
        [ProducesResponseType(200, Type = typeof(AddRoleViewModel))]
        public async Task<IActionResult> AddRolePermissionList(AddRoleViewModel addRole)
        {
            if (TryValidateModel(addRole, nameof(AddRoleViewModel)))
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
                var response = await _unitOfWorkService.RoleService.AddRole(addRole,userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRoleViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditRole")]
        [ProducesResponseType(200, Type = typeof(EditRoleViewModel))]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRole)
        {
            if (TryValidateModel(editRole, nameof(EditRoleViewModel)))
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
                var response = await _unitOfWorkService.RoleService.EditRole(editRole, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRoleViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DeleteRole")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]

        public IActionResult DeleteRole(int RoleId)
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
            var response = _unitOfWorkService.RoleService.DeleteRole(RoleId, userId);
            return Ok(response);
        }

        [HttpPost("DeleteRoleGroups")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]

        public IActionResult DeleteRoleGroups(int RoleId)
        {
            var response = _unitOfWorkService.RoleService.DeleteRoleGroups(RoleId);
            return Ok(response);
        }
        [HttpGet("GetRoleByRoleName")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public IActionResult GetRoleByRoleName(string RoleName)
        {
            var response = _unitOfWorkService.RoleService.GetRoleByRoleName(RoleName);
            return Ok(response);

        }
        [HttpGet("GetRoleByRoleId")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public IActionResult GetRoleByRoleId(int RoleId)
        {
            var response = _unitOfWorkService.RoleService.GetRoleByRoleId(RoleId);
            return Ok(response);
        }
        [HttpGet("GetOldPermissionRoleById")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public IActionResult GetOldPermissionRoleById(int RoleId)
        {
            var response = _unitOfWorkService.RoleService.GetOldPermissionRoleById(RoleId);
            return Ok(response);
        }
    }
}