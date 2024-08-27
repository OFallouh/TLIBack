using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TLIS_API.Middleware.ActionFilters;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_ODULibraryDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_API.Helpers.Constants;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(ExternalSystemFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class InternalApiController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public InternalApiController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetCivilsInstalledonSite")]
        [ProducesResponseType(200, Type = typeof(AllCivilInstallationViewModel))]
        public IActionResult GetCivilsBySiteCode(string SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode(SiteCode, connectionString,userId,null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode(SiteCode, connectionString,null,username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
       
        
        [HttpPost("GetSideArmsInstalledonCivil")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttForSideArm))]
        public IActionResult GetSideArmsInstalledonCivil([Required] string SiteCode, string CivilType, string CivilName, int? LegId, float? MinAzimuth, float? MaxAzimuth, float? MinHeightBase, float? MaxHeightBase)
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
                var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilType, CivilName, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase,userId,null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilType, CivilName, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase,null,username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
       
        [HttpPost("GetLibraryforSpecificType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetLibraryforSpecificType(string TableNameLibrary, int CategoryId, [FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
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
                var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(connectionString,TableNameLibrary, CategoryId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(connectionString,TableNameLibrary, CategoryId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          
        }
        [HttpPost("GetAllSitesDetails")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModel>))]
        public IActionResult GetAllSites([FromQueryAttribute] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters, bool? isRefresh, bool? GetItemsCountOnEachSite)
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
                var response = _unitOfWorkService.SiteService.GetSites(userId, null, parameterPagination, isRefresh, GetItemsCountOnEachSite, filters);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.SiteService.GetSites(null, username, parameterPagination, isRefresh, GetItemsCountOnEachSite, filters);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAllLoadonSitebyPartandType")]
        [ProducesResponseType(200, Type = typeof(Response<LoadsDto>))]
        public IActionResult GetAllLoadonSitebyPartandType([Required] String SiteCode, string PartName, string TypeName)
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
                var response = _unitOfWorkService.InternalApiService.GetAllLoadonSitebyPartandType(SiteCode, PartName, TypeName,userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAllLoadonSitebyPartandType(SiteCode, PartName, TypeName, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        

        }
        [HttpPost("GetAlOtherInventoryonSitebyType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetAlOtherInventoryonSitebyType([Required] string OtherInventoryTypeName, [FromQuery] string SiteCode, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
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
                var response = _unitOfWorkService.InternalApiService.GetAlOtherInventoryonSitebyType(OtherInventoryTypeName, SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAlOtherInventoryonSitebyType(OtherInventoryTypeName, SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpGet("GetAllItemsonSite ")]
        [ProducesResponseType(200, Type = typeof(Response<List<ListOfCivilLoads>>))]
        public IActionResult GetAllItemsonSite(string SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetAllItemsonSite(SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAllItemsonSite(SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpPost("GetConfigurationTablesInstallation ")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetConfigurationTablesInstallation([FromQuery] string siteCode, [Required] string TableNameInstallation, int CategoryId, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
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
                var response = _unitOfWorkService.InternalApiService.GetConfigurationTables(siteCode, TableNameInstallation, CategoryId, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetConfigurationTables(siteCode, TableNameInstallation, CategoryId, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          

        }
        [HttpPost("GetConfigurationAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<List<BassAttViewModel>>))]
        public IActionResult GetConfigurationAttributes(string TableName, bool IsDynamic, int CategoryId)
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
                var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, CategoryId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, CategoryId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
       

        }
        [HttpPost("AddDynamicAttLibrary")]
        [ProducesResponseType(200, Type = typeof(AddDynamicAttViewModel))]
        public IActionResult AddDynamicAttLibrary([FromBody] AddDependencyViewModel addDependencyView)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.InternalApiService.AddDynamicAtts(addDependencyView, ConnectionString);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddDynamicAttViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }
        [HttpPost("AddDynamicAttInstallation")]
        [ProducesResponseType(200, Type = typeof(List<AddDependencyInstViewModel>))]
        public IActionResult AddDynamicAttInstallation([FromBody] AddDependencyInstViewModel addDependencyInstViewModel)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.InternalApiService.AddDynamicAttInst(addDependencyInstViewModel, ConnectionString);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<AddDynamicAttInstViewModel>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditDynamicAttLibraryAndInstallation")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public async Task<IActionResult> EditDynamicAttLibraryAndInstallation([FromBody] EditDynamicAttViewModel dynamicAttViewModel)
        {
            var response = await _unitOfWorkService.InternalApiService.Edit(dynamicAttViewModel);
            return Ok(response);
        }
        [HttpPost("AddRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRULibraryObject))]
        public IActionResult AddRadioRRULibrary(AddRadioRRULibraryObject addRadioRRU)
        {
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRULibraryObject)))
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
                    var response = _unitOfWorkService.InternalApiService.AddRadioRRULibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, connectionString, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddRadioRRULibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, connectionString, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                var errorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioRRULibraryObject>(true, null, errorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherLibraryObject))]
        public IActionResult AddRadioOtherLibrary(AddRadioOtherLibraryObject addRadioOther)
        {
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherLibraryObject)))
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
                    var response = _unitOfWorkService.InternalApiService.AddRadioOtherLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioOther, connectionString, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddRadioOtherLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioOther, connectionString, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioOtherLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaLibraryObject))]
        public IActionResult AddRadioAntennaLibrary(AddRadioAntennaLibraryObject addRadioAntenna)
        {
            if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaLibraryObject)))
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
                    var response = _unitOfWorkService.InternalApiService.AddRadioAntennaLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioAntenna, connectionString, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddRadioAntennaLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioAntenna, connectionString, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioAntennaLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetForAddRadioLibrary")]
        [ProducesResponseType(200, Type = typeof(List<AllAtributes>))]
        public IActionResult GetForAddLibrary(string TableName)
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
                var response = _unitOfWorkService.RadioLibraryService.GetForAdd(TableName, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioLibraryService.GetForAdd(TableName, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetAttForAddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioAntennaInstallation(int LibId, string SiteCode)
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
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioAntennaInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioAntennaInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetAttForAddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioRRUInstallation(int LibId, string SiteCode)
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
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioRRUInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioRRUInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetAttForAddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioOtherInstallation(int LibId, string SiteCode)
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
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioOtherInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioOtherInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("AddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaInstallationObject))]
        public IActionResult AddRadioAntennaInstallation([FromBody] AddRadioAntennaInstallationObject addRadioAntenna, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioAntenna,Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(),SiteCode, connectionString, TaskId, userId,null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioAntenna,Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(),SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                        {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddRadioAntennaInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRUInstallationObject))]
        public IActionResult AddRadioRRUInstallation([FromBody] AddRadioRRUInstallationObject addRadioRRU, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioRRU, nameof(AddRadioRRUInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioRRU, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioRRU, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddRadioRRUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherInstallationObject))]
        public IActionResult AddRadioOtherInstallation([FromBody] AddRadioOtherInstallationObject addRadioOther, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioOther, nameof(AddRadioOtherInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioOther, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioOther, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddRadioOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAttForAddMW_BU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWBUInstallation(int LibId, string SiteCode)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWBUInstallation(Helpers.Constants.LoadSubType.TLImwBU.ToString(), LibId, SiteCode, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWBUInstallation(Helpers.Constants.LoadSubType.TLImwBU.ToString(), LibId, SiteCode, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("GetAttForAddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_ODU(int LibId, string SiteCode)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWODUInstallation(Helpers.Constants.LoadSubType.TLImwODU.ToString(), LibId, SiteCode, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWODUInstallation(Helpers.Constants.LoadSubType.TLImwODU.ToString(), LibId, SiteCode, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMW_Dish(int LibId, string SiteCode)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWDishInstallation(Helpers.Constants.LoadSubType.TLImwDish.ToString(), LibId, SiteCode, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.GetAttForAddMWDishInstallation(Helpers.Constants.LoadSubType.TLImwDish.ToString(), LibId, SiteCode, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWRFUInstallation(int LibId, string SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddMWRFUInstallation(Helpers.Constants.LoadSubType.TLImwRFU.ToString(), LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddMWRFUInstallation(Helpers.Constants.LoadSubType.TLImwRFU.ToString(), LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }



        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_Other")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWOtherInstallation(int LibId, string SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddMWOtherInstallation(Helpers.Constants.LoadSubType.TLImwOther.ToString(), LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddMWOtherInstallation(Helpers.Constants.LoadSubType.TLImwOther.ToString(), LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }


        [HttpPost("AddMW_BU")]
        [ProducesResponseType(200, Type = typeof(AddMWBUInstallationObject))]
        public IActionResult AddMW_BU([FromBody] AddMWBUInstallationObject AddMW_BUViewModel, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_BUViewModel, nameof(AddMWBUInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWBUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(AddMwODUinstallationObject))]
        public IActionResult AddMW_ODU([FromBody] AddMwODUinstallationObject AddMW_ODUViewModel, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_ODUViewModel, nameof(AddMwODUinstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMwODUinstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(AddMWDishInstallationObject))]
        public IActionResult AddMW_Dish([FromBody] AddMWDishInstallationObject AddMW_DishViewModel, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_DishViewModel, nameof(AddMWDishInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMwODUinstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(AddMWRFUInstallation))]
        public IActionResult AddMW_RFU([FromBody] AddMWRFUInstallation AddMW_RFUViewModel, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_RFUViewModel, nameof(AddMWRFUInstallation)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWRFUInstallation>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddMW_Other")]
        [ProducesResponseType(200, Type = typeof(AddMWOtherInstallationObject))]
        public IActionResult AddMW_Other([FromBody] AddMWOtherInstallationObject AddMw_OtherViewModel, string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(AddMw_OtherViewModel, nameof(AddMWOtherInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditMW_BU")]
        [ProducesResponseType(200, Type = typeof(EditMWBUInstallationObject))]
        public async Task<IActionResult> EditMW_BU([FromBody] EditMWBUInstallationObject MW_BU,int TaskId)
        {
            try
            {
                if (TryValidateModel(MW_BU, nameof(EditMWBUInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditMWBUInstallation(userId, MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditMWBUInstallation(null, MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWBUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditMW_Dish")]
        [ProducesResponseType(200, Type = typeof(EditMWDishInstallationObject))]
        public async Task<IActionResult> EditMW_Dish([FromBody] EditMWDishInstallationObject MW_Dish, int TaskId)
        {
            try
            {
                if (TryValidateModel(MW_Dish, nameof(EditMWDishInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditMWDishInstallation(userId, MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditMWDishInstallation(null, MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWDishInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditMW_ODU")]
        [ProducesResponseType(200, Type = typeof(EditMWODUInstallationObject))]
        public async Task<IActionResult> EditMW_ODU([FromBody] EditMWODUInstallationObject MW_ODU,int TaskId)
        {
            try
            {
                if (TryValidateModel(MW_ODU, nameof(EditMWODUInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditMWODUInstallation(userId, MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditMWODUInstallation(null, MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWODUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditMW_RFU")]
        [ProducesResponseType(200, Type = typeof(EditMWRFUInstallationObject))]
        public async Task<IActionResult> EditMW_RFU([FromBody] EditMWRFUInstallationObject MW_RFU,int TaskId)
        {
            try
            {
                if (TryValidateModel(MW_RFU, nameof(EditMWRFUInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditMWRFUInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditMWRFUInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), connectionString, TaskId,null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWRFUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditMw_Other")]
        [ProducesResponseType(200, Type = typeof(EditMWOtherInstallationObject))]
        public async Task<IActionResult> EditMw_Other([FromBody] EditMWOtherInstallationObject Mw_Other,int TaskId)
        {
            try
            {
                if (TryValidateModel(Mw_Other, nameof(EditMWOtherInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditMWOtherInstallation(userId, Mw_Other, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditMWOtherInstallation(null, Mw_Other, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantleMW_BU")]

        public IActionResult DismantleMW_BU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId,userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, null, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantleMW_ODU")]

        public IActionResult DismantleMW_ODU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, null, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       [ HttpPost("DismantleMW_RFU")]

        public IActionResult DismantleMW_RFU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), TaskId, userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), TaskId, null, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantleMW_Dish")]

        public IActionResult DismantleMW_Dish(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, null, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantleMW_Other")]

        public IActionResult DismantleMW_Other(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, null, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantlesideArm")]
        public IActionResult DismantlesideArm(string SiteCode, int sideArmId,int  TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleSideArmInternal(SiteCode, sideArmId, TaskId, connectionString, userId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleSideArmInternal(SiteCode, sideArmId, TaskId, connectionString,null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetMW_BUById")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetMWBUInstallationById(int MW_BU)
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
                var response = _unitOfWorkService.InternalApiService.GetMWBUInstallationById(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWBUInstallationById(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpGet("GetMW_ODUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_ODUById(int MW_ODU)
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
                var response = _unitOfWorkService.InternalApiService.GetMWODUInstallationById(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWODUInstallationById(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetMW_DishById")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMW_DishById(int MW_Dish)
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
                var response = _unitOfWorkService.InternalApiService.GetMWDishInstallationById(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWDishInstallationById(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetMW_RFUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWRFUInstallationById(int MW_RFU)
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
                var response = _unitOfWorkService.InternalApiService.GetMWRFUInstallationById(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWRFUInstallationById(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetMW_OtherById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWOtherInstallationById(int mwOther)
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
                var response = _unitOfWorkService.InternalApiService.GetMWOtherInstallationById(mwOther, Helpers.Constants.LoadSubType.TLImwOther.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWOtherInstallationById(mwOther, Helpers.Constants.LoadSubType.TLImwOther.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetAttForAddSolar")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddSolarInstallation(int SolarLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddSolarInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SolarLibraryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddGenerator")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddGeneratorInstallation(int GeneratorIdLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddGeneratorInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), GeneratorIdLibraryId, SiteCode);
            return Ok(response);
        }
       
        [HttpPost("AddSolar")]
        [ProducesResponseType(200, Type = typeof(AddSolarInstallationObject))]
        public IActionResult AddSolar([FromBody] AddSolarInstallationObject addSolarViewModel, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addSolarViewModel, nameof(AddSolarInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddSolarInstallation(addSolarViewModel, SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddSolarInstallation(addSolarViewModel, SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddSolarInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("AddGenerator")]
        [ProducesResponseType(200, Type = typeof(AddGeneratorInstallationObject))]
        public IActionResult AddGenerator([FromBody] AddGeneratorInstallationObject addGeneratorViewModel, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddGeneratorInstallation(addGeneratorViewModel,SiteCode,connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddGeneratorInstallation(addGeneratorViewModel, SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddGeneratorInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("GetSolarById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetSolarInstallationById(int SolarId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarInstallationById(SolarId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetGeneratorById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetGenertorInstallationById(int GeneratorId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetGenertorInstallationById(GeneratorId, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString());
            return Ok(response);
        }
        
        [HttpPost("EditSolar")]
        [ProducesResponseType(200, Type = typeof(EditSolarInstallationObject))]
        public async Task<IActionResult> EditSolar([FromBody] EditSolarInstallationObject editSolarViewModel,int TaskId)
        {
            try
            {
                if (TryValidateModel(editSolarViewModel, nameof(EditMWOtherInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditOtherInventoryInstallationInternal(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId,userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditOtherInventoryInstallationInternal(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditMWOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EditGenerator")]
        [ProducesResponseType(200, Type = typeof(EditGeneratorInstallationObject))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditGeneratorInstallationObject editGeneratorViewModel,int TaskId)
        {
            try
            {
                if (TryValidateModel(editGeneratorViewModel, nameof(EditGeneratorInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditOtherInventoryInstallationInternal(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditOtherInventoryInstallationInternal(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditGeneratorInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //---------------------------------------------------------------------------------
        [HttpGet("DismantleOtherInventory")]
        public IActionResult DismantleOtherInventory(string SiteCode, int OtherInventoryId, string OtherInventoryName,int TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleOtherInventoryInternal(userId, SiteCode, OtherInventoryId, OtherInventoryName, TaskId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleOtherInventoryInternal(null, SiteCode, OtherInventoryId, OtherInventoryName, TaskId, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        } 
        [HttpPost("GetSolarBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetSolarBySiteWithEnabledAtt([FromQuery] string? SiteCode)
        {

            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetGeneratorWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetGeneratorBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetGeneratorBySiteWithEnabledAtt([FromQuery] string? SiteCode)
        {

            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMW_DishOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_DishOnSiteWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetMWDishInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWDishInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetMW_BUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWBUInstallationWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetMWBUInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWBUInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetMW_ODUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWODUInstallationWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetMWODUInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWODUInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost("GetMW_RFUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWRFUInstallationWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetMWRFUInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWRFUInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetMW_OtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWOtherInstallationWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetMWOtherInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWOtherInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpGet("GetSideArmById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetSideArmById(int SideId)
        {
            var response = _unitOfWorkService.SideArmService.GetSideArmById(SideId, Helpers.Constants.TablesNames.TLIsideArm.ToString());
            return Ok(response);
        }
        [HttpPost("AddSideArm")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public async Task<IActionResult> AddSideArm([FromBody] SideArmViewDto sideArmViewDto,string SiteCode, int TaskId)
        {
            try
            {
                if (TryValidateModel(sideArmViewDto, nameof(SideArmViewDto)))
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
                        var response = _unitOfWorkService.InternalApiService.AddSideArmInternal(sideArmViewDto, SiteCode, TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddSideArmInternal(sideArmViewDto, SiteCode, TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<SideArmViewDto>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("UpdateSideArm")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public async Task<IActionResult> UpdateSideArm([FromBody] EditSidearmInstallationObject SideArmViewModel,int TaskId)
        {
            try
            {
                if (TryValidateModel(SideArmViewModel, nameof(EditSidearmInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.UpdateSideArmInternal(SideArmViewModel, TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.UpdateSideArmInternal(SideArmViewModel, TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditSidearmInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAttForAddSideArm")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibId)
        {
            var response = _unitOfWorkService.SideArmService.GetAttForAdd(LibId);
            return Ok(response);
        }
        [HttpPost("getSideArmsWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<object>))]
        public IActionResult getSideArmsWithEnabledAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.SideArmService.GetSideArmInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteelWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilNonSteelLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("AddCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelLibraryObject))]
        public IActionResult AddCivilNonSteelLibrary([FromBody] AddCivilNonSteelLibraryObject addCivilNonSteelLibraryViewModel)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.AddCivilNonSteelLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, connectionString, userId,null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddCivilNonSteelLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, connectionString, null, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetCivilWithoutLegMastLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegMastLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegMastLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetCivilWithoutLegMonopoleLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegMonopoleLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegMonopoleLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetCivilWithoutLegCapsuleLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegCapsuleLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegCapsuleLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMW_ODULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_ODULibraries()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWODULibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMW_BULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMWBULibrariesEnabledAtt()
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWBULibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMW_OtherLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMWOtherLibrariesEnabledAtt()
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWOtherLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("AddSolarLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSolarLibrary([FromBody] AddSolarLibraryObject addSolarLibrary)
        {
            try
            {
                if (TryValidateModel(addSolarLibrary, nameof(AddSolarLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddSolarLibrary(userId, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addSolarLibrary, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddSolarLibrary(null, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addSolarLibrary, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddSolarLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetGeneratorLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetGeneratorLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetGeneratorLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("AddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddGeneratorLibrary([FromBody] AddGeneratorLibraryObject addGeneratorLibrary)
        {
            try
            {
                if (TryValidateModel(addGeneratorLibrary, nameof(AddGeneratorLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddGenertatoLibrary(userId, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addGeneratorLibrary, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddGenertatoLibrary(null, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addGeneratorLibrary, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddSolarLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
      
        [HttpPost("AddMW_RFULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_RFULibraryViewModel))]
        public IActionResult AddMW_RFULibrary([FromBody] AddMWRFULibraryObject addMW_RFULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_RFULibraryViewModel, nameof(AddMWRFULibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddMWRFULibrary(userId, Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString(), addMW_RFULibraryViewModel, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWRFULibrary(null, Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString(), addMW_RFULibraryViewModel, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWRFULibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetMW_RFULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_RFULibraries()
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWRFULibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("AddMW_OtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddMWOtherLibraryObject))]
        public IActionResult AddMW_OtherLibrary([FromBody] AddMWOtherLibraryObject addMW_OtherLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_OtherLibraryViewModel, nameof(AddMWOtherLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddMWOtherLibrary(userId, Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), addMW_OtherLibraryViewModel, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWOtherLibrary(null, Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), addMW_OtherLibraryViewModel, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWOtherLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMW_BULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMWBULibraryObject))]
        public IActionResult AddMW_BULibrary([FromBody] AddMWBULibraryObject addMW_BULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMWBULibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddMWBULibrary(userId, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), addMW_BULibraryViewModel, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWBULibrary(null, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), addMW_BULibraryViewModel, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWBULibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddMW_DishLibrary")]
        [ProducesResponseType(200, Type = typeof(AddMWDishLibraryObject))]
        public IActionResult AddMW_DishLibrary([FromBody] AddMWDishLibraryObject addMW_BULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMWDishLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddMWDishLibrary(userId, Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), addMW_BULibraryViewModel, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWDishLibrary(null, Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), addMW_BULibraryViewModel, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddMWDishLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetMW_DishLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_DishLibraries()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWDishLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }

        [HttpGet("GetForAddMWDishLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWDishLibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddMWBUibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWBUibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddMWODULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWODULibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddMWOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWOtherLibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddMWRFULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWRFULibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilNonSteelLibrary")]
        public IActionResult GetForAddCivilNonSteelLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithLegsLibrary")]
        public IActionResult GetForAddCivilWithLegsLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsMastLibrary")]
        public IActionResult GetForAddCivilWithoutLegsMastLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutMastLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsCapsuleLibrary")]
        public IActionResult GetForAddCivilWithoutLegsCapsuleLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutCapsuleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsMonopleLibrary")]
        public IActionResult GetForAddCivilWithoutLegsMonopleLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutMonopleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddMW_ODULibrary")]
        [ProducesResponseType(200, Type = typeof(ADDMWODULibraryObject))]
        public IActionResult AddMW_ODULibrary([FromBody] ADDMWODULibraryObject addMW_ODULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_ODULibraryViewModel, nameof(ADDMWODULibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddMWODULibrary(userId, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), addMW_ODULibraryViewModel, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWODULibrary(null, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), addMW_ODULibraryViewModel, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<ADDMWODULibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegsLibraryObject))]
        public IActionResult AddCivilWithoutLegLibrary([FromBody] AddCivilWithoutLegsLibraryObject addCivilWithoutLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegsLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary( Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString,userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString,null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithoutLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddCivilWithLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsLibraryObject))]
        public IActionResult AddCivilWithLegLibrary([FromBody] AddCivilWithLegsLibraryObject CivilWithLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(CivilWithLegLibraryViewModel, nameof(AddCivilWithLegsLibraryObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibraryViewModel, connectionString, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibraryViewModel, connectionString, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetCivilWithLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithLegLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetSolarLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetSolarLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetSolarLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithLegs(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithLegInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }

        [HttpPost("GetCivilWithLegsWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegsWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegsWithEnableAttInternal(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegsWithEnableAttInternal(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpPost("GetCivilWithoutLegMastWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegMastWithEnableAttt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMastWithEnableAttInternal(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMastWithEnableAttInternal(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegMonopoleWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegMonopoleWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMonopoleWithEnableAttInternal(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMonopoleWithEnableAttInternal(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
           
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegCapsuleWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegCapsuleWithEnableAtt([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegCapsuleWithEnableAttInternal(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegCapsuleWithEnableAttInternal(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          
        }
        [HttpPost("GetCivilNonSteelWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteel([FromQuery] string? SiteCode)
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
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelWithEnableAttInternal(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelWithEnableAttInternal(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpGet("GetAttForAddCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithoutLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCiviNonSteelInstallation(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCiviNonSteelInstallation(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddCivilWithLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsViewModel))]
        public IActionResult AddCivilWithLegs([FromBody] AddCivilWithLegsViewModel addCivilWithLeg, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsInstallation(addCivilWithLeg,Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(),SiteCode, connectionString,TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsInstallation(addCivilWithLeg,Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("AddCivilWithoutLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegViewModel))]
        public IActionResult AddCivilWithoutLegs([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("AddCivilNonSteel/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelObject))]
        public IActionResult AddCivilNonSteel([FromBody] AddCivilNonSteelObject addCivilNonSteel, string SiteCode,int TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelObject)))
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
                        var response = _unitOfWorkService.InternalApiService.AddCivilNonSteelInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilNonSteelInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, connectionString, TaskId, null, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilNonSteelObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCivilWithLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithLegsInstallationById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString());
            return Ok(response);
        }
        [HttpGet("GetCivilWithoutLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithoutLegsInstallationById(int CivilId, int CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CategoryId);
            return Ok(response);
        }
        [HttpGet("GetCivilNonSteelById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilNonSteelById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilNonSteelInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
            return Ok(response);
        }

        [HttpPost("EditCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithLegsInstallationObject))]
        public async Task<IActionResult> EditCivilWithLegs([FromBody] EditCivilWithLegsInstallationObject CivilWithLeg,int TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithLeg, nameof(EditCivilWithLegsInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditCivilWithLegsInstallation(CivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditCivilWithLegsInstallation(CivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditCivilWithLegsInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EditCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithoutLegsInstallationObject))]
        public async Task<IActionResult> EditCivilWithoutLegs([FromBody] EditCivilWithoutLegsInstallationObject CivilWithoutLeg,int TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegsInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditCivilWithoutLegsInstallation(CivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditCivilWithoutLegsInstallation(CivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditCivilWithoutLegsInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EditCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(EditCivilNonSteelInstallationObject))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditCivilNonSteelInstallationObject CivilNonSteel,int TaskId)
        {
            try
            {
                if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelInstallationObject)))
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
                        var response = _unitOfWorkService.InternalApiService.EditCivilNonSteelInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), TaskId, userId, connectionString, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.EditCivilNonSteelInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), TaskId, null, connectionString, username);
                        return Ok(response);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditCivilNonSteelInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DismantleCivilWithLegsInstallation")]
        public IActionResult DismantleCivilWithLegsInstallation(string SiteCode, int CivilId, int? TaskId)
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
                var response = _unitOfWorkService.InternalApiService.DismantleCivilWithLegsInstallation(userId, SiteCode, CivilId, TaskId, connectionString, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.DismantleCivilWithLegsInstallation(null, SiteCode, CivilId, TaskId, connectionString, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("DismantleCivilWithoutLegsInstallation")]
        public IActionResult DismantleCivilWithoutLegsInstallation(string SiteCode, int CivilId, int? TaskId)
        {
            try
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
                    var response = _unitOfWorkService.InternalApiService.DismantleCivilWithoutLegsInstallation(userId, SiteCode, CivilId, TaskId, connectionString,null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.DismantleCivilWithoutLegsInstallation(null, SiteCode, CivilId, TaskId, connectionString, username);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized();
                }
               

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("DismantleCivilNonSteelInstallation")]
        public IActionResult DismantleCivilNonSteelInstallation(string SiteCode, int CivilId, int? TaskId)
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
                var response = _unitOfWorkService.InternalApiService.DismantleCivilNonSteelInstallation(userId, SiteCode, CivilId, TaskId, connectionString, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.DismantleCivilNonSteelInstallation(null, SiteCode, CivilId, TaskId, connectionString, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
    }
}