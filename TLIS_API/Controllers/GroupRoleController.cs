using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupRoleController : ControllerBase
    {
        IUnitOfWorkService _unitOfWorkService;
        public GroupRoleController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(List<GroupRoleViewModel>))]
        public async Task<IActionResult> GetGroupRoles()
        {
            var response = await _unitOfWorkService.GroupRoleService.GetGroupRoles();
            return Ok(response);
        }

        [HttpGet("GetGroupsByRoleId/{RoleId}")]
        [ProducesResponseType(200, Type = typeof(List<GroupRoleViewModel>))]
        public async Task<IActionResult> GetGroupsByRoleId(int RoleId)
        {
            var response = await _unitOfWorkService.GroupRoleService.GetGroupsByRoleId(RoleId);
            return Ok(response);
        }

        [HttpGet("GetRolesByGroupId/{GroupId}")]
        [ProducesResponseType(200, Type = typeof(List<GroupRoleViewModel>))]
        public async Task<IActionResult> GetRolesByGroupId(int GroupId)
        {
            var response = await _unitOfWorkService.GroupRoleService.GetRolesByGroupId(GroupId);
            return Ok(response);
        }

        [HttpPost("AddGroupRole")]
        [ProducesResponseType(200, Type = typeof(List<AddGroupRoleViewModel>))]
        public async Task<IActionResult> AddGroupRole([FromBody]AddGroupRoleViewModel addGroupRoleView)
        {
            if(TryValidateModel(addGroupRoleView, nameof(AddGroupRoleViewModel)))
            {
                var response = await _unitOfWorkService.GroupRoleService.AddGroupRole(addGroupRoleView);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGroupRoleViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
    }
}