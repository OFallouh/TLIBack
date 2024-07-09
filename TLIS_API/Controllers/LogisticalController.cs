using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_Service.ServiceBase;
using System.Collections.Generic;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using Org.BouncyCastle.Asn1.Cmp;
using TLIS_API.Middleware.WorkFlow;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticalController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        public LogisticalController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("GetById/{LogisticalId}")]
        [ProducesResponseType(200, Type = typeof(MainLogisticalViewModel))]
        public IActionResult GetById(int LogisticalId)
        {
            var response = _unitOfWorkService.LogisiticalService.GetById(LogisticalId);
            return Ok(response);
        }
        [HttpPost("GetLogisticalByTypeOrPart")]
        [ProducesResponseType(200, Type = typeof(List<MainLogisticalViewModel>))]
        public IActionResult GetLogisticalByTypeOrPart(string TablePartName, string LogisticalType, string Search, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.LogisiticalService.GetLogisticalByTypeOrPart(TablePartName, LogisticalType, Search, parameterPagination);
            return Ok(response);
        }
        [HttpPost("AddLogistical")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult AddLogistical([FromBody] AddNewLogistical NewLogistical)
        {
            var response = _unitOfWorkService.LogisiticalService.AddLogistical(NewLogistical);
            return Ok(response);
        }
        [HttpPost("DeleteLogistical/{LogisticalId}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult DeleteLogistical(int LogisticalId)
        {
            var response = _unitOfWorkService.LogisiticalService.DeleteLogistical(LogisticalId);
            return Ok(response);
        }
        [HttpGet("DisableLogistical")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult DisableLogistical(int LogisticalId)
        {
            var response = _unitOfWorkService.LogisiticalService.DisableLogistical(LogisticalId);
            return Ok(response);
        }
        [HttpPost("EditLogistical/{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult EditLogistical(int Id, EditLogisticalViewModel EditLogisticalViewModel)
        {
            if(Id != EditLogisticalViewModel.Id)
            {
                return BadRequest("Id is not valid.");
            }
            var response = _unitOfWorkService.LogisiticalService.EditLogistical(EditLogisticalViewModel);
            return Ok(response);
        }
        [HttpGet("GetLogisticalTypes")]
        [ProducesResponseType(200, Type = typeof(List<LogisticalViewModel>))]
        public IActionResult GetLogisticalTypes()
        {
            var response = _unitOfWorkService.LogisiticalService.GetLogisticalTypes();
            return Ok(response);
        }
    }
}
