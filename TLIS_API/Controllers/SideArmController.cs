using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmInstallationPlaceDTOs;
using TLIS_DAL.ViewModels.SideArmTypeDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class SideArmController : ControllerBase
    {
        private readonly IUnitOfWorkService _UnitOfWorkService;
        private readonly IConfiguration _configuration;
        public SideArmController(IUnitOfWorkService UnitOfWorkService, IConfiguration configuration)
        {
            _UnitOfWorkService = UnitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSideArm")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SideArmViewModel>))]
        public IActionResult GetSideArm(CivilLoadsFilter BaseFilter, bool WithFilterData, List<FilterObjectList> filters)
        {
            var response = _UnitOfWorkService.SideArmService.getSideArms(BaseFilter, WithFilterData, filters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getSideArmsWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<object>))]
        public IActionResult getSideArmsWithEnabledAtt([FromBody] CombineFilters CombineFilters, CivilLoadsFilter BaseFilter, bool WithFilterData, int? CivilRecordId, string CivilType, [FromQuery] ParameterPagination parameters)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmsWithEnabledAtt(BaseFilter, WithFilterData, parameters, CombineFilters, CivilRecordId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("getSideArmsForAdd")]
        [ProducesResponseType(200, Type = typeof(List<KeyValuePair<string, int>>))]
        public IActionResult getSideArmsForAdd(string SiteCode, int CivilId, int? LegId, int? MinHeight, int? MaxHeight, int? NumberOfLoadsOnSideArm, int? MinAzimuth, int? MaxAzimuth)
        {
            var response = _UnitOfWorkService.SideArmService.getSideArmsForAdd(SiteCode, CivilId, LegId, MinHeight, MaxHeight, NumberOfLoadsOnSideArm, MinAzimuth, MaxAzimuth);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetSideArmById(int SideId)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmById(SideId, Helpers.Constants.TablesNames.TLIsideArm.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetSideArm(int id)
        {
            var response = _UnitOfWorkService.SideArmService.GetById(id);
            return Ok(response);
        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        //[HttpPost("AddSideArm")]
        //[ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        //public IActionResult AddSideArm([FromBody] AddSideArmViewModel SideArmViewModel, string SiteCode, int TaskId)
        //{
        //    var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //    var Response = _UnitOfWorkService.SideArmService.AddSideArm(SideArmViewModel, SiteCode, ConnectionString, TaskId);
        //    return Ok(Response);
        //}
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        //[HttpPost("UpdateSideArm")]
        //[ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        //public async Task<IActionResult> UpdateSideArm([FromBody] EditSideArmViewModel SideArmViewModel, int TaskId)
        //{
        //    var response = await _UnitOfWorkService.SideArmService.UpdateSideArm(SideArmViewModel, TaskId);
        //    return Ok(response);
        //}
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddSideArmInstallationPlace")]
        [ProducesResponseType(200, Type = typeof(AddSideArmInstallationPlaceViewModel))]
        public async Task<IActionResult> AddSideArmInstallationPlace([FromBody] AddSideArmInstallationPlaceViewModel SideArmInstallationPlaceViewModel)
        {
            var response = await _UnitOfWorkService.SideArmService.AddSideArmInstallationPlace(SideArmInstallationPlaceViewModel);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("UpdateSideArmInstallationPlace")]
        [ProducesResponseType(200, Type = typeof(EditSideArmInstallationPlaceViewModel))]
        public async Task<IActionResult> UpdateSideArmInstallationPlace([FromBody] EditSideArmInstallationPlaceViewModel SideArmInstallationPlaceViewModel)
        {
            var response = await _UnitOfWorkService.SideArmService.UpdateSideArmInstallationPlace(SideArmInstallationPlaceViewModel);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSideArmInstallationPlaceByType")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<SideArmInstallationPlaceViewModel>))]
        public async Task<IActionResult> GetSideArmInstallationPlace(int civilInstallationPlaceType)
        {
            var response = await _UnitOfWorkService.SideArmService.GetSideArmInstallationPlace(civilInstallationPlaceType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAdd")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibId)
        {
            var response = _UnitOfWorkService.SideArmService.GetAttForAdd(LibId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantlesideArm")]
        public IActionResult DismantlesideArm(string SiteCode, int sideArmId,int TaskId)
        {
            var response = _UnitOfWorkService.SideArmService.DismantleSideArm(SiteCode, sideArmId, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmType")]
        [ProducesResponseType(200, Type = typeof(SideArmTypeViewModel))]
        public IActionResult GetSideArmType()
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmType();
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmInstallationPlace")]
        [ProducesResponseType(200, Type = typeof(List<SideArmInstallationPlaceViewModel>))]
        public IActionResult GetSideArmInstallationPlace(string CivilType, int SideArmTypeId)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmInstallationPlace(CivilType, SideArmTypeId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmTypes")]
        [ProducesResponseType(200, Type = typeof(List<SideArmTypeViewModel>))]
        public IActionResult GetSideArmTypes(string tablename)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmTypes(tablename);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmsByAllCivilInstId/{AllCivilInstId}")]
        [ProducesResponseType(200, Type = typeof(List<SideArmViewModel>))]
        public IActionResult GetSideArmsByAllCivilInstId(int AllCivilInstId)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmsByAllCivilInstId(AllCivilInstId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSideArmsByFilters")]
        [ProducesResponseType(200, Type = typeof(List<SideArmViewModel>))]
        public IActionResult GetSideArmsByFilters(int AllCivilInstId, float? MaxAzimuth, float? MinAzimuth, float? MaxHeightBase, float? MinHeightBase)
        {
            var response = _UnitOfWorkService.SideArmService.GetSideArmsByFilters(AllCivilInstId, MaxAzimuth, MinAzimuth, MaxHeightBase, MinHeightBase);
            return Ok(response);
        }
    }
}
