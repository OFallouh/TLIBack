using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.ViewModelBase;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CivilLibraryController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public CivilLibraryController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("GetCivilLibraryByType")]
        [ProducesResponseType(200, Type = typeof(List<LibraryNamesViewModel>))]
        public IActionResult GetCivilLibraryByType(string CivilType,int? CivilWithoutLegCategoryId = null)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilLibraryByType(CivilType, CivilWithoutLegCategoryId);
            return Ok(response);
        }
        [HttpGet("GetForAdd/{CivilType}")]
        // [ProducesResponseType(200, Type = typeof(List<AllItemAttributes>))]
        public IActionResult GetForAdd(string CivilType, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(CivilType, CategoryId);
            return Ok(response);
        }
    }
}