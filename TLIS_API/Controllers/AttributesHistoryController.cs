using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.StaticAttributesHistory;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributesHistoryController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public AttributesHistoryController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("GetStaticAttributesHistoryByTableName")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<StaticAttsHistoryViewModel>))]
        public IActionResult GetStaticAttributesHistoryByTableName([FromQuery] ParameterPagination parameterPagination, string TableName, [FromBody] List<FilterObjectList> filters = null)
        { 
            var response = _unitOfWorkService.AttributeHistoryService.GetStaticAttributesHistoryByTableName(filters, TableName, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetDynamicAttributesHistoryByTableName")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<StaticAttsHistoryViewModel>))]
        public IActionResult GetDynamicAttributesHistoryByTableName([FromQuery] ParameterPagination parameterPagination, string TableName, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = _unitOfWorkService.AttributeHistoryService.GetDynamicAttributesHistoryByTableName(filters, TableName, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetAttachedFileHistory")]
        [ProducesResponseType(200, Type = typeof(List<HistoryViewModel>))]
        public IActionResult GetDynamicAttributesHistoryByTableName([FromQuery] ParameterPagination parameterPagination, string TableName,int RecordId, string FileStatus)
        {
            var response = _unitOfWorkService.AttributeHistoryService.GetAttachedFileHistory( TableName, RecordId, parameterPagination);
            return Ok(response);
        }
    }
}
