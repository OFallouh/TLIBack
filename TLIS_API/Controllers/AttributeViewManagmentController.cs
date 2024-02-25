using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AttributeViewManagmentDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using NLog;
using System.Diagnostics;
using TLIS_API.Middleware.WorkFlow;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeViewManagmentController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly Logger _logger = LogManager.GetLogger(typeof(LogFilterAttribute).FullName);


        public AttributeViewManagmentController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }

        [HttpPost("GetAttributesForView")]
        [ProducesResponseType(200, Type = typeof(List<AttributeViewManagmentViewModel>))]
        public IActionResult GetAttributesForView(string ViewName, [FromQuery]ParameterPagination parameterPagination, string AttributeName)
        {
            var response = _unitOfWorkService.AttributeViewManagmentService.GetAttributesForView(ViewName, parameterPagination, AttributeName);
            return Ok(response);
        }
        
        [HttpPost("UpdateAttributeStatus/{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AttributeViewManagmentViewModel>))]
        public async Task<IActionResult> UpdateAttributeStatus(int id)
        {
            var response = await _unitOfWorkService.AttributeViewManagmentService.UpdateAttributeStatus(id);
            return Ok(response);
        }
        [HttpPost("GetAttributesForViewWithoutPagination")]
        [ProducesResponseType(200, Type = typeof(List<AttributeViewManagmentViewModel>))]
        public IActionResult GetAttributesForViewWithoutPagination(string ViewName, string Search)
        {
            var response = _unitOfWorkService.AttributeViewManagmentService.GetAttributesForViewWithoutPagination(ViewName, Search);
            return Ok(response);
        }
    }
}
