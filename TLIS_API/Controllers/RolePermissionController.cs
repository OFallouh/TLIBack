using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public RolePermissionController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        //[HttpGet("GetAllPermissionsByRoleId/{RoleId}")]
        //[ProducesResponseType(200, Type = typeof(List<string>))]
        //public IActionResult GetAllPermissionsByRoleId(int RoleId)
        //{
        //    var response = _unitOfWorkService.RolePermissionService.GetAllPermissionsByRoleId(RoleId);
        //    return Ok(response);
        //}
        [HttpGet("GetAllRolePermissionsForW_F")]
        [ProducesResponseType(200, Type = typeof(List<RolePermissionViewModel>))]
        public IActionResult GetAllRolePermissionsForW_F()
        {
            var response = _unitOfWorkService.RolePermissionService.GetAllPermissionForW_F();
            return Ok(response.Data);
        }
    }
}