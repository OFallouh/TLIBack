using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.GroupUserDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupUserController : ControllerBase
    {
        IUnitOfWorkService _unitOfWorkService;
        public GroupUserController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(List<GroupUserViewModel>))]
        public async Task<IActionResult> GetAllGroupUsers()
        {
            var response = await _unitOfWorkService.GroupUserService.GetAllGroupUsers();
            return Ok(response);
        }
        [HttpGet("GetAllGroupUsers_WFVersion")]
        [ProducesResponseType(200, Type = typeof(List<GroupUserViewModel>))]
        public async Task<IActionResult> GetAllGroupUsers_WFVersion()
        {
            var response = await _unitOfWorkService.GroupUserService.GetAllGroupUsers();
            return Ok(response.Data);
        }

        [HttpGet("GetGroupsByUserId/{UserId}")]
        [ProducesResponseType(200, Type = typeof(List<GroupUserViewModel>))]
        public async Task<IActionResult> GetGroupsByUserId(int UserId)
        {
            var response = await _unitOfWorkService.GroupUserService.GetGroupsByUserId(UserId);
            return Ok(response);
        }

        [HttpGet("GetUsersByGroupId/{GroupId}")]
        [ProducesResponseType(200, Type = typeof(List<GroupUserViewModel>))]
        public async Task<IActionResult> GetUsersByGroupId(int GroupId)
        {
            var response = await _unitOfWorkService.GroupUserService.GetUsersByGroupId(GroupId);
            return Ok(response);
        }
    }
}