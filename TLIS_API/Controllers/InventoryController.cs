using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public InventoryController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        //[HttpGet("GetForAdd")]
        //[ProducesResponseType(200, Type = typeof(List<AllItemAttributes>))]
        //public IActionResult GetForAddLibrary(string TableName)
        //{
        //    var response = _unitOfWorkService.CivilLibraryService.GetForAdd(TableName);
        //    return Ok(response);
        //}
    }
}