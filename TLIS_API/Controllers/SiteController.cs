using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AreaDTOs;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.RegionDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.SiteStatusDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.SiteService;

namespace TLIS_API.Controllers
{

    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class SiteController : Controller
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;

        public SiteController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddSite")]
        [ProducesResponseType(200, Type = typeof(AddSiteViewModel))]
        public IActionResult AddSite([FromBody] AddSiteViewModel AddSiteViewModel, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = _unitOfWorkService.SiteService.AddSite(AddSiteViewModel,TaskId, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditSite")]
        [ProducesResponseType(200, Type = typeof(EditSiteViewModel))]
        public IActionResult EditSite([FromBody] EditSiteViewModel EditSiteViewModel, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = _unitOfWorkService.SiteService.EditSite(EditSiteViewModel, TaskId, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllAreasForSiteOperation")]
        [ProducesResponseType(200, Type = typeof(List<AreaViewModel>))]
        public IActionResult GetAllAreasForSiteOperation()
        {
            var response = _unitOfWorkService.SiteService.GetAllAreasForSiteOperation();
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllSiteStatusForSiteOperation")]
        [ProducesResponseType(200, Type = typeof(List<SiteStatusViewModel>))]
        public IActionResult GetAllSiteStatusForSiteOperation()
        {
            var response = _unitOfWorkService.SiteService.GetAllSiteStatusForSiteOperation();
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllRegionsForSiteOperation")]
        [ProducesResponseType(200, Type = typeof(List<RegionViewModel>))]
        public IActionResult GetAllRegionsForSiteOperation()
        {
            var response = _unitOfWorkService.SiteService.GetAllRegionsForSiteOperation();
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllLocationTypesForSiteOperation")]
        [ProducesResponseType(200, Type = typeof(List<LocationTypeViewModel>))]
        public IActionResult GetAllLocationTypesForSiteOperation()
        {
            var response = _unitOfWorkService.SiteService.GetAllLocationTypesForSiteOperation();
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getAllSites")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModelForGetAll>))]
        public IActionResult GetAllSites(
           [FromQuery] bool? isRefresh, [FromQuery] bool? GetItemsCountOnEachSite, [FromBody] FilterRequest request)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }
            if (authHeader.ToLower().StartsWith("bearer "))
            {

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return Unauthorized();
                }

                string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
                var userId = Convert.ToInt32(userInfo);
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.SiteService.GetSites(userId,null, isRefresh, GetItemsCountOnEachSite,false, request);
                return Ok(response);

            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.SiteService.GetSites(null, username, isRefresh, GetItemsCountOnEachSite, false, request);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSiteMainSpaces")]
        [ProducesResponseType(200, Type = typeof(SiteViewModel))]
        public IActionResult GetSiteMainSpaces(string SiteCode)

        {
            var response = _unitOfWorkService.SiteService.GetSiteMainSpaces(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilsBySiteCode")]
        [ProducesResponseType(200, Type = typeof(List<SiteCivilsViewModel>))]
        public IActionResult GetCivilsBySiteCode(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetCivilsBySiteCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilsWithAllCivilInstIdsBySiteCode")]
        [ProducesResponseType(200, Type = typeof(List<SiteCivilsViewModel>))]
        public IActionResult GetCivilsWithAllCivilInstIdsBySiteCode(string SiteCode, string CivilType)
        {
            var response = _unitOfWorkService.SiteService.GetCivilsWithAllCivilInstIdsBySiteCode(SiteCode, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]

        [HttpGet("GetSideArmsBySiteCode")]
        [ProducesResponseType(200, Type = typeof(List<SideArmViewModel>))]
        public IActionResult GetSideArmsBySiteCode(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSideArmsBySiteCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetOtherInventoriesBySiteCode")]
        [ProducesResponseType(200, Type = typeof(List<SiteOtherInventoriesViewModel>))]
        public IActionResult GetOtherInventoriesBySiteCode(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetOtherInventoriesBySiteCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetLoadsBySiteCode")]
        [ProducesResponseType(200, Type = typeof(List<SiteLoadsViewModel>))]
        public IActionResult GetLoadsBySiteCode(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetLoadsBySiteCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSpaceDetails/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public IActionResult GetSpaceDetails(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSpaceDetails(SiteCode);
            return Ok(response);
        }

        //[HttpPost("UpdateRentedSpace")]
        //[ProducesResponseType(200, Type = typeof(SiteViewModel))]
        //public async Task<IActionResult> UpdateRentedSpace(string SiteCode, float RentedSpaceValue)
        //{
        //    var response = await _unitOfWorkService.SiteService.UpdateRentedSpace(SiteCode, RentedSpaceValue);
        //    //var userId = HttpContext.Session.GetString("UserId");

        //    return Ok(response);
        //}
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("UpdateSiteStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateSiteStatus(ListSiteStatusViewModel SiteStatus)
        {
            if (TryValidateModel(SiteStatus, nameof(TLIsiteStatus)))
            {
                var response = _unitOfWorkService.SiteService.UpdatSiteStatus(SiteStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListSiteStatusViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddSiteStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSiteStatus(AddSiteStatusViewModel SiteStatus)
        {
            if (TryValidateModel(SiteStatus, nameof(SiteStatusViewModel)))
            {
                var response = _unitOfWorkService.SiteService.AddSiteStatus(SiteStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListSiteStatusViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllViews")]
        [ProducesResponseType(200, Type = typeof(List<dynamic>))]
        public IActionResult GetAllViews(string sitecode, string encodedFilter,int? Case)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.ExecuteStoredProcedureAndQueryDynamicView(ConnectionString, sitecode, encodedFilter, Case);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetHistory")]
        [ProducesResponseType(200, Type = typeof(List<dynamic>))]
        public IActionResult GetHistory(
        [FromQuery] string TabelName,
        [FromQuery] string? BaseId,
        [FromQuery] int? UserId,
        [FromQuery] string SiteCode,
        [FromQuery] int? ExternalSysId,
        [FromBody] GetHistoryRequest request)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];

            var response = _unitOfWorkService.SiteService.GetHistory(TabelName, BaseId, SiteCode, UserId, ExternalSysId, ConnectionString, request.First, request.Rows, request.SortOrder,
           request.Filters, request.MultiSortMeta);
            return Ok(response);
        }

        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSiteStatusById/{SiteStatusId}")]
        [ProducesResponseType(200, Type = typeof(SiteStatusViewModel))]
        public IActionResult GetSiteStatusById(int SiteStatusId)
        {
            var response = _unitOfWorkService.SiteService.GetSiteStatusbyId(SiteStatusId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSMIS_Site")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, [FromBody] string RowContent)
        {
            var response = await _unitOfWorkService.SiteService.GetSMIS_Site(UserName, Password, ViewName, Paramater, RowContent);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSMIS_Site_Test")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetSMIS_Site_Test(string UserName, string Password, string ViewName, string Paramater, [FromBody] string RowContent)
        {
            var response = await _unitOfWorkService.SiteService.GetSMIS_Site_Test(UserName, Password, ViewName, Paramater, RowContent);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllSiteStatus")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        public IActionResult GetAllSiteStatus()//[FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters
        {
            var response = _unitOfWorkService.SiteService.GetAllSiteStatus();//parameterPagination, filters
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllRegion")]
        [ProducesResponseType(200, Type = typeof(List<RegionViewModel>))]
        public IActionResult GetAllRegion()//[FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters
        {
            var response = _unitOfWorkService.SiteService.GetAllRegion();//parameterPagination, filters
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllArea")]
        [ProducesResponseType(200, Type = typeof(List<AreaViewModel>))]
        public IActionResult GetAllArea()//[FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters
        {
            var response = _unitOfWorkService.SiteService.GetAllArea();//parameterPagination, filters
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DeleteSiteStatus/{SiteStatusId}")]
        [ProducesResponseType(200, Type = typeof(SiteStatusViewModel))]
        public IActionResult DeleteSiteStatus(int SiteStatusId)
        {
            var response = _unitOfWorkService.SiteService.DeleteSiteStatus(SiteStatusId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSteelCivil/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(List<KeyValuePair<string, int>>))]
        public IActionResult GetSteelCivil(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSteelCivil(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetNonSteel/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(List<KeyValuePair<string, int>>))]
        public IActionResult GetNonSteel(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetNonSteel(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("Test")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModel>))]
        public void Test()
        {
            _unitOfWorkService.SiteService.test();
            //return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadsOnSite")]
        // [ProducesResponseType(200, Type = typeof(LoadsViewModel))]
        public IActionResult GetLoadsOnSite([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData)
        {
            var response = _unitOfWorkService.SiteService.GetLoadsOnSite(BaseFilter, WithFilterData);
            return Ok(response);

        }
        
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetMW_BUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_BUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetMW_BUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetMW_ODUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_ODUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetMW_ODUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetMW_RFUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_RFUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetMW_RFUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetMW_OtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_OtherOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetMW_OtherOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioAntennaOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioAntennaOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetRadioAntennaOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioRRUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioRRUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetRadioRRUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioOtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioOtherOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetRadioOtherOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetPowerOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetPowerOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.SiteService.GetPowerOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("DisplaySiteDetailsBySiteCode")]
        [ProducesResponseType(200, Type = typeof(SiteViewModel))]
        public IActionResult DisplaySiteDetailsBySiteCode(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.DisplaySiteDetailsBySiteCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSitebyId")]
        [ProducesResponseType(200, Type = typeof(SiteViewModel))]
        public IActionResult GetSitebyId(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSitebyId(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditSitesMainSpaces")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        public async Task<IActionResult> EditSitesMainSpaces(string SiteCode, [FromBody] EditSiteViewModel EditSiteViewModel)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = await _unitOfWorkService.SiteService.EditSitesMainSpaces(EditSiteViewModel.RentedSpace, EditSiteViewModel.ReservedSpace, SiteCode, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSitePhotosSlideshow")]
        [ProducesResponseType(200, Type = typeof(List<SiteAssets>))]
        public IActionResult GetSitePhotosSlideshow(string SiteCode)//[FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters
        {
            var response = _unitOfWorkService.SiteService.GetSitePhotosSlideshow(SiteCode);//parameterPagination, filters
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllsiteOnMultiRegion")]
        [ProducesResponseType(200, Type = typeof(List<GetAllsiteOnMultiRegion>))]
        public IActionResult GetAllsiteOnMultiRegion([FromBody] List<RegionForSiteViewModel> Region)
        {
            var response = _unitOfWorkService.SiteService.GetAllsiteonMultiRegion(Region);
            return Ok(response);
        }

        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllsiteOnMultiArea")]
        [ProducesResponseType(200, Type = typeof(List<GetAllsiteOnMultiAreaViewModel>))]
        public IActionResult GetAllsiteOnMultiArea([FromBody] List<AreaForSiteViewModel> Area)
        {
            var response = _unitOfWorkService.SiteService.GetAllsiteonMultiArea(Area);
            return Ok(response);
        }


        //[HttpPost("SubmitInserting")]
        //[ProducesResponseType(200, Type = typeof(CivilNonSteelViewModel))]
        //public async Task<IActionResult> SubmitInserting()
        //{
        //    if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelViewModel)))
        //    {
        //        var response = await _unitOfWorkService.CivilInstService.EditCivilInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<CivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSiteNamebySiteCode")]
        [ProducesResponseType(200, Type = typeof(GetSiteNameBySitCode))]
        public async Task<IActionResult> GetSiteNamebySiteCode([FromBody] List<SiteCodeForW_F> SiteCode)
        {
            var response = await _unitOfWorkService.SiteService.GetSiteNameBySitCode(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllSitesWithoutPaginationForWorkFlow")]
        public IActionResult GetAllSitesWithoutPaginationForWorkFlow()
        {
            var response = _unitOfWorkService.SiteService.GetAllSitesWithoutPaginationForWorkFlow();
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllSitesWithoutPagination")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModelForGetAll>))]
        public IActionResult GetAllSitesWithoutPagination(bool? GetItemsCountOnEachSite)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.GetAllSitesWithoutPagination( GetItemsCountOnEachSite);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetUsedSitesCount")]
        public IActionResult GetUsedSitesCount()
        {
            var response = _unitOfWorkService.SiteService.GetUsedSitesCount();
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetItemsOnSite")]
        public IActionResult GetItemsOnSite(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetItemsOnSite(SiteCode);
            return Ok(response);
        }
        [HttpGet("RecalculateSite")]
        public IActionResult RecalculateSite()
        {
            var response = _unitOfWorkService.SiteService.RecalculateSite();
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllInstallationOnSite")]
        [ProducesResponseType(200, Type = typeof(List<RegionViewModel>))]
        public async Task<ActionResult> GetAllInstallationOnSite(string SiteCode,string TableName, int? CategoryId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.GetConfigurationTables(SiteCode, TableName, CategoryId, ConnectionString);
            var fullPath = response.Data ;
            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(bytes, contentType, Path.GetFileName(fullPath));

        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditSiteDetalis")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        public async Task<IActionResult> EditSiteDetalis([FromBody]SiteDetailsObject siteDetailsObject, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var response = _unitOfWorkService.SiteService.EditSiteDetalis(siteDetailsObject,TaskId, userId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSiteDetails")]
        public IActionResult GetSiteDetails(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSiteDetails(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllLogs")]
        public IActionResult GetAllLogs([FromBody] FilterRequest request)
        {
            var response = _unitOfWorkService.SiteService.GetLogsWithPaginationAndSorting(request);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ClearAllHistory")]
        public IActionResult ClearAllHistory(string dateFrom, string dateTo)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.ClearAllHistory(ConnectionString, dateFrom, dateTo);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ClearAllLogHistory")]
        public IActionResult ClearAllLogHistory(string dateFrom, string dateTo)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SiteService.ClearLogHistory(ConnectionString, dateFrom, dateTo);
            return Ok(response);
        }
    }
}