using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeActivatedController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public AttributeActivatedController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("AddTablesActivatedAttributes")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> AddTablesActivatedAttributes()
        {
            await _unitOfWorkService.AttributeActivatedService.AddTablesActivatedAttributes();
            return Ok();
        }
    }
}