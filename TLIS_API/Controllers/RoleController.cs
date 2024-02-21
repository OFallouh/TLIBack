using System;
using System.Collections.Generic;
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

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public RoleController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        //[Authorize]
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles([FromBody] List<FilterObjectList> filters)
        {
            var response = await _unitOfWorkService.RoleService.GetRoles(filters);
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
        public IActionResult AddRolePermissionList(AddRoleViewModel addRole)
        {
            if (TryValidateModel(addRole, nameof(AddRoleViewModel)))
            {
                var response = _unitOfWorkService.RoleService.AddRole(addRole);
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
                var response = await _unitOfWorkService.RoleService.EditRole(editRole);
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
            var response = _unitOfWorkService.RoleService.DeleteRole(RoleId);
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
        public IActionResult GetRoleByRoleName(string ToleName)
        {
            var response = _unitOfWorkService.RoleService.GetRoleByRoleName(ToleName);
            return Ok(response);
        }
    }
}