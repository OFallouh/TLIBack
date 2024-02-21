using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        IUnitOfWorkService _unitOfWorkService;
        IConfiguration _configuration;
        public GroupController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        //[Authorize]
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<GroupViewModel>))]
        public IActionResult GetAllGroups([FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.GroupService.GetAllGroups(filters);
            return Ok(response);
        }

        [HttpGet("GetById/{GroupId}")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult GetById(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.GetById(GroupId);
            return Ok(response);
        }

        [HttpGet("CheckGroupChildrens/{GroupId}")]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        public IActionResult CheckGroupChildrens(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.CheckGroupChildrens(GroupId);
            return Ok(response);
        }

        [HttpGet("GetGroupByName")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult GetGroupByName(string GroupName)
        {
            var response = _unitOfWorkService.GroupService.GetGroupByName(GroupName);
            return Ok(response);
        }
        [HttpPost("EditGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> EditGroup([FromBody]GroupViewModel groupViewModel)
        {
            if(TryValidateModel(groupViewModel, nameof(GroupViewModel)))
            {
                var domain = _configuration["Domain"];
                var response = await _unitOfWorkService.GroupService.EditGroup(groupViewModel, domain);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<GroupViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("AddGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult AddGroup([FromBody]AddGroupViewModel addGroupViewModel)
        {
            if(TryValidateModel(addGroupViewModel, nameof(AddGroupViewModel)))
            {
                //var domain = _configuration["Domain"];
                var response = _unitOfWorkService.GroupService.AddGroup(addGroupViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<GroupViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpGet("ValidateGroupNameFromAD")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult ValidateGroupNameFromAD(string GroupName)
        {
            var domain = _configuration["Domain"];
            var result = _unitOfWorkService.GroupService.ValidateGroupNameFromADAdd(GroupName, domain);
            return Ok(new Response<bool>(result));
        }

        [HttpGet("ValidateGroupNameFromDatabase")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult ValidateGroupNameFromDatabase(string GroupName)
        {
            var result = _unitOfWorkService.GroupService.ValidateGroupNameFromDatabaseAdd(GroupName);
            return Ok(new Response<bool>(result));
        }

        [HttpPost("DeleteGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> DeleteGroup(int GroupId)
        {
            var response = await _unitOfWorkService.GroupService.DeleteGroup(GroupId);
            return Ok(response);
        }

        [HttpPost("AddActorToGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> AddActorToGroup(int GroupId, int ActorId)
        {
            var response  = await _unitOfWorkService.GroupService.AddActorToGroup(GroupId, ActorId);
            return Ok(response);
        }

        [HttpPost("UpdateActorToGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> UpdateActorToGroup(int GroupId, int ActorId)
        {

            var response = await _unitOfWorkService.GroupService.UpdateActorToGroup(GroupId, ActorId);
            return Ok(response);
        }

        [HttpPost("DeleteActorToGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> DeleteActorToGroup(int GroupId)
        {

            var response = await _unitOfWorkService.GroupService.DeleteActorToGroup(GroupId);
            return Ok(response);
        }

        [HttpGet("GetGroupsTest")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult GetGroupsTest()
        {
            var response = _unitOfWorkService.GroupService.GetGroupsTest();
            return Ok(response);
        }
        [HttpPost("DeleteAssingedUppers")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public async Task<IActionResult> DeleteAssingedUppers(int GroupId)
        {

            var response = await _unitOfWorkService.GroupService.DeleteAssingedUppers(GroupId);
            return Ok(response);
        }
        [HttpPost("GetUppersOfGroup")]
        [ProducesResponseType(200, Type = typeof(List<GroupViewModel>))]
        public IActionResult GetUppersOfGroup(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.GetUppersOfGroup(GroupId);
            return Ok(response);
        }
        [HttpPost("AssignUpperToGroup")]
        [ProducesResponseType(200, Type = typeof(List<GroupViewModel>))]
        public async Task<IActionResult> AssignUpperToGroup(int GroupId, int NewUpperId)
        {
            var response = await _unitOfWorkService.GroupService.AssignUpperToGroup(GroupId, NewUpperId);
            return Ok(response);
        }
        [HttpPost("DeleteGroupWithItsChildren")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult DeleteGroupWithPermissions(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.DeleteGroupWithItsChildren(GroupId, true);
            return Ok(response);
        }

        [HttpPost("DeleteGroupWithOutChildren")]
        [ProducesResponseType(200, Type = typeof(Response<string>))]
        public IActionResult DeleteGroupWithOutChildren(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.DeleteGroupWithItsChildren(GroupId, false);
            return Ok(response);
        }



        [HttpGet("CheckGroup")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult CheckGroup(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.CheckGroup(GroupId);
            return Ok(response);
        }
        [HttpPost("GetAllGroupsWithoutLowerLevelOfUppers")]
        [ProducesResponseType(200, Type = typeof(Response<List<GroupViewModel>>))]
        public IActionResult GetAllGroupsWithoutLowerLevelOfUppers(int GroupId, List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.GroupService.GetAllGroupsWithoutLowerLevelOfUppers(GroupId, filters);
            return Ok(response);
        }
        [HttpGet("UnAssignParentRleation")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult UnAssignParentRleation(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.UnAssignParentRleation(GroupId);
            return Ok(response);
        }
        [HttpPost("DeleteGroupWithItsChilds")]
        [ProducesResponseType(200, Type = typeof(GroupViewModel))]
        public IActionResult DeleteGroupWithItsChilds(int GroupId)
        {
            var response = _unitOfWorkService.GroupService.DeleteGroupChildren(GroupId);
            return Ok(response);
        }
        [HttpGet("GetGroupByName_WFVersion")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GroupViewModel>))]
        public IActionResult GetGroupByName_WFVersion()
        {
            var response = _unitOfWorkService.GroupService.GetGroupByName_WFVersion();
            return Ok(response.Data);
        }
        
    }
}