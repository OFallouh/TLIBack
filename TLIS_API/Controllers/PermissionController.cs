using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public PermissionController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetPermissions()
        {
            var response = _unitOfWorkService.PermissionService.GetPermissions();
            return Ok(response);
        }
        [HttpGet("getAllPermissionFor_WF")]
        [ProducesResponseType(200, Type = typeof(List<PermissionFor_WFViewModel>))]
        public IActionResult getAllPermission()
        {
            var response = _unitOfWorkService.PermissionService.GetAllPermissionsFor_WF();
            return Ok(response.Data);
        }
        [HttpGet("GetPermissionsForUser")]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetPermissionsForUser(int UserId)
        {
            var response = _unitOfWorkService.PermissionService.GetPermissionsForUser(UserId);
            return Ok(response);
        }
        [HttpPost("GetPermissionsByModuleName")]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetPermissionsByModuleName(string ModuleName)
        {
            var response = _unitOfWorkService.PermissionService.GetPermissionsByModuleName(ModuleName);
            return Ok(response);
        }

        [HttpPost("AddPermission")]
        [ProducesResponseType(200, Type = typeof(AddPermissionViewModel))]
        public async Task<IActionResult> AddPermission([FromBody]AddPermissionViewModel addPermission)
        {
            if(TryValidateModel(addPermission, nameof(AddPermissionViewModel)))
            {
                var response = await _unitOfWorkService.PermissionService.AddPermission(addPermission);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddPermissionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetPermissionsByName")]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetPermissionsForUser(string PermissionName)
        {
            var response = _unitOfWorkService.PermissionService.GetPermissionsByName(PermissionName);
            return Ok(response);
        }
        [HttpGet("GetAllModulesNames")]
        [ProducesResponseType(200, Type = typeof(List<ModulesNamesViewModel>))]
        public IActionResult GetAllModulesNames()
        {
            var response = _unitOfWorkService.PermissionService.GetAllModulesNames();
            return Ok(response);
        }
       
    }
}