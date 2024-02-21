using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public UserPermissionController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(UserPermissionViewModel))]
        public IActionResult GetUserPermissionsByUserId(int id)
        {
            var response = _unitOfWorkService.UserPermissionService.GetUserPermissionsByUserId(id);
            return Ok(response);
        }
    }
}