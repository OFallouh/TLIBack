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
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
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
    //[ServiceFilter(typeof(WorkFlowMiddleware))]
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
        public IActionResult GetCivilsInstalledonSite(string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode(SiteCode, connectionString, null, username);
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
                var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilType, CivilName, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilType, CivilName, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("GetLibraryforSpecificType")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetLibraryforSpecificType(string TableNameLibrary)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(connectionString, TableNameLibrary, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(connectionString, TableNameLibrary, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetAllSitesDetails")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModelForGetAll>))]
        public IActionResult GetAllSitesDetails([FromQueryAttribute] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters, bool? isRefresh, bool? GetItemsCountOnEachSite)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpGet("GetAllLoadonSitebyPartandType")]
        [ProducesResponseType(200, Type = typeof(Response<LoadsDtoInternal>))]
        public IActionResult GetAllLoadonSitebyPartandType([Required] String SiteCode, string PartName, string TypeName)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetAllLoadonSitebyPartandType(SiteCode, PartName, TypeName, userId, null);
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
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetAlOtherInventoryonSitebyType([Required] string OtherInventoryTypeName, [FromQuery] string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpGet("GetAllItemsOnSite ")]
        [ProducesResponseType(200, Type = typeof(Response<List<ListOfCivilLoads>>))]
        public IActionResult GetAllItemsOnSite(string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetConfigurationTablesInstallation([FromQuery] string siteCode, [Required] string TableNameInstallation)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetConfigurationTables(siteCode, TableNameInstallation, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetConfigurationTables(siteCode, TableNameInstallation, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost("GetConfigurationAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<List<BassAttViewModel>>))]
        public IActionResult GetConfigurationAttributes(string TableName, bool IsDynamic)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost("AddDynamicAttributeLibrary")]
        public IActionResult AddDynamicAttributeLibrary([FromBody] AddDynamicObject addDynamicObject, string TabelName, int? CategoryId)
        {
            if (ModelState.IsValid)
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];


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
                    var response = _unitOfWorkService.InternalApiService.AddDynamicInternal(addDynamicObject, connectionString, TabelName, userId, CategoryId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddDynamicInternal(addDynamicObject, connectionString, TabelName, null, CategoryId, username);
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
                return Ok(new Response<AddDynamicAttViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }
        [HttpPost("AddDynamicAttributeInstallation")]
        public IActionResult AddDynamicAttributeInstallation([FromBody] AddDynamicObject addDynamicObject, string TabelName, int? CategoryId)
        {
            if (ModelState.IsValid)
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];

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
                    var response = _unitOfWorkService.InternalApiService.AddDynamicInternal(addDynamicObject, connectionString, TabelName, userId, CategoryId, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response = _unitOfWorkService.InternalApiService.AddDynamicInternal(addDynamicObject, connectionString, TabelName, null, CategoryId, username);
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
                return Ok(new Response<AddDynamicAttViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditDynamicAttLibraryAndInstallation")]
        public async Task<IActionResult> EditDynamicAttLibraryAndInstallation(int DynamicAttributeId, [FromBody] AddDynamicObject dynamicAttViewModel)
        {
            if (ModelState.IsValid)
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];


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
                    var response =await _unitOfWorkService.InternalApiService.EditDynamicAttribute(DynamicAttributeId, dynamicAttViewModel, userId, connectionString, null);
                    return Ok(response);
                }
                else if (authHeader.ToLower().StartsWith("basic "))
                {

                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':')[0];
                    var password = decodedUsernamePassword.Split(':')[1];
                    var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                    var response =await _unitOfWorkService.InternalApiService.EditDynamicAttribute(DynamicAttributeId, dynamicAttViewModel, null, connectionString, username);
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
                return Ok(new Response<AddDynamicAttViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }
        [HttpPost("AddRadioRRULibrary")]
        public IActionResult AddRadioRRULibrary(AddRadioRRULibraryObject addRadioRRU)
        {
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRULibraryObject)))
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        public IActionResult AddRadioOtherLibrary(AddRadioOtherLibraryObject addRadioOther)
        {
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherLibraryObject)))
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];


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
        public IActionResult AddRadioAntennaLibrary(AddRadioAntennaLibraryObject addRadioAntenna)
        {
            if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaLibraryObject)))
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [ProducesResponseType(200, Type = typeof(GetForAddCivilLibrarybject))]
        public IActionResult GetForAddRadioLibrary(string TableName)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddRadioLibrary(TableName, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddRadioLibrary(TableName, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetAttForAddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddRadioAntennaInstallation(int LibId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioAntennaInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioAntennaInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetAttForAddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddRadioRRUInstallation(int LibId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioRRUInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioRRUInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetAttForAddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddRadioOtherInstallation(int LibId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioOtherInstallation(LibId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioOtherInstallation(LibId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("AddRadioAntennaInstallation")]
        public IActionResult AddRadioAntennaInstallation([FromBody] AddRadioAntennaInstallationObject addRadioAntenna, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddRadioInstallationInternal(addRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), SiteCode, connectionString, TaskId, null, username);
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
        public IActionResult AddRadioRRUInstallation([FromBody] AddRadioRRUInstallationObject addRadioRRU, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioRRU, nameof(AddRadioRRUInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        public IActionResult AddRadioOtherInstallation([FromBody] AddRadioOtherInstallationObject addRadioOther, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addRadioOther, nameof(AddRadioOtherInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddMWBUInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMWBUInstallation(int LibId, string SiteCode)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddMWODUInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMWODUInstallation(int LibId, string SiteCode)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddMWDishInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMWDishInstallation(int LibId, string SiteCode)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddMWRFUInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMWRFUInstallation(int LibId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddMWOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMWOtherInstallation(int LibId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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


        [HttpPost("AddMWBUInstallation")]
        public IActionResult AddMWBUInstallation([FromBody] AddMWBUInstallationObject AddMW_BUViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_BUViewModel, nameof(AddMWBUInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(userId, AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, connectionString, TaskId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(null, AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, connectionString, TaskId, username);
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
        [HttpPost("AddMWODUInstallation")]
        public IActionResult AddMWODUInstallation([FromBody] AddMwODUinstallationObject AddMW_ODUViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_ODUViewModel, nameof(AddMwODUinstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(userId, AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, connectionString, TaskId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(null, AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, connectionString, TaskId, username);
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
        [HttpPost("AddMWDishInstallation")]
        public IActionResult AddMWDishInstallation([FromBody] AddMWDishInstallationObject AddMW_DishViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_DishViewModel, nameof(AddMWDishInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(userId, AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, connectionString, TaskId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(null, AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, connectionString, TaskId, username);
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
        [HttpPost("AddMWRFUInstallation")]
        public IActionResult AddMWRFUInstallation([FromBody] AddMWRFUInstallation AddMW_RFUViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(AddMW_RFUViewModel, nameof(AddMWRFUInstallation)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddMWRFUInstallation(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWRFUInstallation(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, connectionString, TaskId, null, username);
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

        [HttpPost("AddMWOtherInstallation")]
        public IActionResult AddMWOtherInstallation([FromBody] AddMWOtherInstallationObject AddMw_OtherViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(AddMw_OtherViewModel, nameof(AddMWOtherInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(userId, AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, connectionString, TaskId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddMWInstallationInternal(null, AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, connectionString, TaskId, username);
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
        [HttpPost("EditMWBUInstallation")]
        public async Task<IActionResult> EditMWBUInstallation([FromBody] EditMWBUInstallationObject MW_BU, int? TaskId)
        {
            try
            {
                if (TryValidateModel(MW_BU, nameof(EditMWBUInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("EditMWDishInstallation")]
        public async Task<IActionResult> EditMWDishInstallation([FromBody] EditMWDishInstallationObject MW_Dish, int? TaskId)
        {
            try
            {
                if (TryValidateModel(MW_Dish, nameof(EditMWDishInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("EditMWODUInstallation")]
        public async Task<IActionResult> EditMWODUInstallation([FromBody] EditMWODUInstallationObject MW_ODU, int? TaskId)
        {
            try
            {
                if (TryValidateModel(MW_ODU, nameof(EditMWODUInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("EditMWRFUInstallation")]
        public async Task<IActionResult> EditMWRFUInstallation([FromBody] EditMWRFUInstallationObject MW_RFU, int? TaskId)
        {
            try
            {
                if (TryValidateModel(MW_RFU, nameof(EditMWRFUInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.EditMWRFUInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), connectionString, TaskId, null, username);
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
        [HttpPost("EditMwOtherInstallation")]
        public async Task<IActionResult> EditMwOtherInstallation([FromBody] EditMWOtherInstallationObject Mw_Other, int? TaskId)
        {
            try
            {
                if (TryValidateModel(Mw_Other, nameof(EditMWOtherInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("DismantleMWBUInstallation")]
        public IActionResult DismantleMWBUInstallation(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.DismantleLoadsInternal(sitecode, LoadId, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, userId, connectionString, null);
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
        [HttpPost("DismantleMWODUInstallation")]

        public IActionResult DismantleMWODUInstallation(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("DismantleMWRFUInstallation")]

        public IActionResult DismantleMWRFUInstallation(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("DismantleMWDishInstallation")]

        public IActionResult DismantleMWDishInstallation(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("DismantleMWOtherInstallation")]

        public IActionResult DismantleMWOtherInstallation(string sitecode, int LoadId, string LoadName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("DismantlesideArmInstallation")]
        public IActionResult DismantlesideArmInstallation(string SiteCode, int sideArmId, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.DismantleSideArmInternal(SiteCode, sideArmId, TaskId, connectionString, null, username);
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
        [HttpGet("GetMWBUByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMWBUByIdInstallation(int MW_BU)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpGet("GetMWODUByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMWODUByIdInstallation(int MW_ODU)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetMWDishByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMWDishByIdInstallation(int MW_Dish)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetMWRFUByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMWRFUByIdInstallation(int MW_RFU)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetMWOtherByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMWOtherByIdInstallation(int mwOther)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpGet("GetAttForAddSolarInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddOtherInventoryInstallationObject))]
        public IActionResult GetAttForAddSolarInstallation(int SolarLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddSolarInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SolarLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddSolarInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SolarLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetAttForAddGeneratorInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddOtherInventoryInstallationObject))]
        public IActionResult GetAttForAddGeneratorInstallation(int GeneratorIdLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddGeneratorInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), GeneratorIdLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddGeneratorInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), GeneratorIdLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("AddSolarInstallation")]
        public IActionResult AddSolarInstallation([FromBody] AddSolarInstallationObject addSolarViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addSolarViewModel, nameof(AddSolarInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddGeneratorInstallation")]
        public IActionResult AddGeneratorInstallation([FromBody] AddGeneratorInstallationObject addGeneratorViewModel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddGeneratorInstallation(addGeneratorViewModel, SiteCode, connectionString, TaskId, userId, null);
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
        [HttpGet("GetSolarByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddOtherInventoryInstallationObject))]
        public IActionResult GetSolarByIdInstallation(int SolarId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetSolarInstallationById(SolarId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSolarInstallationById(SolarId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetGeneratorByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddOtherInventoryInstallationObject))]
        public IActionResult GetGeneratorByIdInstallation(int GeneratorId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetGenertorInstallationById(GeneratorId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetGenertorInstallationById(GeneratorId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("EditSolarInstallation")]
        public async Task<IActionResult> EditSolarInstallation([FromBody] EditSolarInstallationObject editSolarViewModel, int? TaskId)
        {
            try
            {
                if (TryValidateModel(editSolarViewModel, nameof(EditMWOtherInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.EditOtherInventoryInstallationInternal(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId, userId, connectionString, null);
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

        [HttpPost("EditGeneratorInstallation")]
        public async Task<IActionResult> EditGeneratorInstallation([FromBody] EditGeneratorInstallationObject editGeneratorViewModel, int? TaskId)
        {
            try
            {
                if (TryValidateModel(editGeneratorViewModel, nameof(EditGeneratorInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("DismantleOtherInventoryInstallation")]
        public IActionResult DismantleOtherInventoryInstallation(string SiteCode, int OtherInventoryId, string OtherInventoryName, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetSolarBySiteWithEnabledAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetSolarWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSolarWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetGeneratorBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetGeneratorBySiteWithEnabledAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetGeneratorWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetGeneratorWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("GetMWDishOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetMWDishOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetMWBUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetMWBUOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetMWODUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetMWODUOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetMWRFUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetMWRFUOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetMWOtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult GetMWOtherOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetSideArmByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetSideArmByIdInstallation(int SideId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetSideArmByInternalId(SideId, Helpers.Constants.TablesNames.TLIsideArm.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSideArmByInternalId(SideId, Helpers.Constants.TablesNames.TLIsideArm.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost("AddSideArmInstallation")]
        public async Task<IActionResult> AddSideArmInstallation([FromBody] SideArmViewDto sideArmViewDto, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(sideArmViewDto, nameof(SideArmViewDto)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("UpdateSideArmInstallation")]
        public async Task<IActionResult> UpdateSideArmInstallation([FromBody] EditSidearmInstallationObject SideArmViewModel, int? TaskId)
        {
            try
            {
                if (TryValidateModel(SideArmViewModel, nameof(EditSidearmInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetAttForAddSideArmInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddCivilLoadObject))]
        public IActionResult GetAttForAddSideArmInstallation(int LibId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetAttForAddSideArm(LibId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetAttForAddSideArm(LibId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("getSideArmsBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(GetEnableAttribute))]
        public IActionResult getSideArmsBySiteWithEnabledAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetSideArmInstallationWithEnableAtt(SiteCode, connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSideArmInstallationWithEnableAtt(SiteCode, connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetCivilNonSteelLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilNonSteelLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("AddCivilNonSteelLibrary")]
        public IActionResult AddCivilNonSteelLibrary([FromBody] AddCivilNonSteelLibraryObject addCivilNonSteelLibraryViewModel)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.AddCivilNonSteelLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, connectionString, userId, null);
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
        [HttpPost("GetCivilWithoutLegMastLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithoutLegMastLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMastLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMastLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("GetCivilWithoutLegMonopoleLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithoutLegMonopoleLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMonopoleLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegMonopoleLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("GetCivilWithoutLegCapsuleLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithoutLegCapsuleLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegCapsuleLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegCapsuleLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("GetMWODULibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetMWODULibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWODULibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWODULibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetMWBULibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetMWBULibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetMWOtherLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetMWOtherLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWOtherLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWOtherLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("AddSolarLibraryEnabledAtt")]
        public IActionResult AddSolarLibraryEnabledAtt([FromBody] AddSolarLibraryObject addSolarLibrary)
        {
            try
            {
                if (TryValidateModel(addSolarLibrary, nameof(AddSolarLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetGeneratorLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetGeneratorLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetGeneratorLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("AddGeneratorLibrary")]
        public IActionResult AddGeneratorLibrary([FromBody] AddGeneratorLibraryObject addGeneratorLibrary)
        {
            try
            {
                if (TryValidateModel(addGeneratorLibrary, nameof(AddGeneratorLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        public IActionResult AddMW_RFULibrary([FromBody] AddMWRFULibraryObject addMW_RFULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_RFULibraryViewModel, nameof(AddMWRFULibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("GetMWRFULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetMWRFULibraries()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWRFULibrariesEnabledAtt(connectionString,userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWRFULibrariesEnabledAtt( connectionString,null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpPost("AddMWOtherLibrary")]
        public IActionResult AddMWOtherLibrary([FromBody] AddMWOtherLibraryObject addMW_OtherLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_OtherLibraryViewModel, nameof(AddMWOtherLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddMWBULibrary")]
        public IActionResult AddMWBULibrary([FromBody] AddMWBULibraryObject addMW_BULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMWBULibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddMWDishLibrary")]
        public IActionResult AddMWDishLibrary([FromBody] AddMWDishLibraryObject addMW_BULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMWDishLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetMWDishLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetMWDishLibraries()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWDishLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWDishLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("GetForAddMWDishLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWDishLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetForAddMWBUibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWBUibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddMWODULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWODULibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddMWOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWOtherLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddMWRFULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWRFULibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddMWLibrary(Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddCivilNonSteelLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddCivilWithLegsLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddCivilWithLegsLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddCivilWithoutLegsMastLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddCivilWithoutLegsMastLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutMastLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutMastLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddCivilWithoutLegsCapsuleLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddCivilWithoutLegsCapsuleLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutCapsuleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutCapsuleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetForAddCivilWithoutLegsMonopleLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddCivilWithoutLegsMonopleLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutMonopleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithoutMonopleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("AddMWODULibrary")]
        public IActionResult AddMWODULibrary([FromBody] ADDMWODULibraryObject addMW_ODULibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addMW_ODULibraryViewModel, nameof(ADDMWODULibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddCivilWithoutLegLibraryCapsule")]
        public IActionResult AddCivilWithoutLegLibraryCapsule([FromBody] AddCivilWithoutLegsLibraryObject addCivilWithoutLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegsLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, null, username);
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
        [HttpPost("AddCivilWithoutLegLibraryMonople")]
        public IActionResult AddCivilWithoutLegLibraryMonople([FromBody] AddCivilWithoutLegsLibraryObject addCivilWithoutLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegsLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, null, username);
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
        [HttpPost("AddCivilWithoutLegLibraryMast")]
        public IActionResult AddCivilWithoutLegLibraryMast([FromBody] AddCivilWithoutLegsLibraryObject addCivilWithoutLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegsLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, connectionString, null, username);
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
        public IActionResult AddCivilWithLegLibrary([FromBody] AddCivilWithLegsLibraryObject CivilWithLegLibraryViewModel)
        {
            try
            {
                if (TryValidateModel(CivilWithLegLibraryViewModel, nameof(AddCivilWithLegsLibraryObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("GetCivilWithLegLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithLegLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetSolarLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetSolarLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetSolarLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetSolarLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetAttForAddCivilWithLegsInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddCivilWithLegObject))]
        public IActionResult GetAttForAddCivilWithLegsInstallation(int CivilLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithLegInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithLegInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("GetCivilWithLegsBySiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithLegsBySiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpPost("GetCivilWithoutLegMastBySiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithoutLegMastBySiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpPost("GetCivilWithoutLegMonopoleBySiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        
        public IActionResult GetCivilWithoutLegMonopoleBySiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpPost("GetCivilWithoutLegCapsuleBySiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilWithoutLegCapsuleBySiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpPost("GetCivilNonSteelBySiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetCivilNonSteelBySiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
        [HttpGet("GetForAddCivilWithOutLeg_CapsuleInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetForAddCivilWithOutLeg_CapsuleInstallation(int CivilLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Capsule(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Capsule(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        ///[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLeg_MastInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetForAddCivilWithOutLeg_MastInstallation(int CivilLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Mast(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Mast(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLeg_MonopleInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetForAddCivilWithOutLeg_MonopleInstallation(int CivilLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Monople(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCivilWithOutLegInstallation_Monople(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetAttForAddCivilNonSteelInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetAttForAddCivilNonSteelInstallation(int CivilLibraryId, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetForAddCiviNonSteelInstallation(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddCiviNonSteelInstallation(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("AddCivilWithLegsInstallation")]
        public IActionResult AddCivilWithLegsInstallation([FromBody] AddCivilWithLegsViewModel addCivilWithLeg, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, connectionString, TaskId, userId, null);
                        return Ok(response);
                    }
                    else if (authHeader.ToLower().StartsWith("basic "))
                    {

                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                        var username = decodedUsernamePassword.Split(':')[0];
                        var password = decodedUsernamePassword.Split(':')[1];
                        var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                        var response = _unitOfWorkService.InternalApiService.AddCivilWithLegsInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, connectionString, TaskId, null, username);
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

        [HttpPost("AddCivilWithoutLegsInstallationMast")]
        public IActionResult AddCivilWithoutLegsInstallationMast([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddCivilWithoutLegsInstallationMonople")]
        public IActionResult AddCivilWithoutLegsInstallationMonople([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("AddCivilWithoutLegsInstallationCapsule")]
        public IActionResult AddCivilWithoutLegsInstallationCapsule([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("AddCivilNonSteelInstallation")]
        public IActionResult AddCivilNonSteelInstallation([FromBody] AddCivilNonSteelObject addCivilNonSteel, string SiteCode, int? TaskId)
        {
            try
            {
                if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpGet("GetForAddCivilWithLegIntsallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithLegObject>))]
        public IActionResult GetForAddCivilWithLegIntsallation(int CivilId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet("GetCivilWithoutLegsMastByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetCivilWithoutLegsMastByIdInstallation(int CivilId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 1, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 1, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetCivilWithoutLegsMonopleByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetCivilWithoutLegsMonopleByIdInstallation(int CivilId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 3, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 3, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetCivilWithoutLegsCapsuleByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetCivilWithoutLegsCapsuleByIdInstallation(int CivilId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 2, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), 2, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("GetCivilNonSteelByIdInstallation")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilWithOutLegInstallationcs>))]
        public IActionResult GetCivilNonSteelByIdInstallation(int CivilId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];


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
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost("EditCivilWithLegsInstallation")]
        public async Task<IActionResult> EditCivilWithLegsInstallation([FromBody] EditCivilWithLegsInstallationObject CivilWithLeg, int? TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithLeg, nameof(EditCivilWithLegsInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("EditCivilWithoutLegsInstallationCapsule")]
        public async Task<IActionResult> EditCivilWithoutLegsInstallationCapsule([FromBody] EditCivilWithoutLegsInstallationObject CivilWithoutLeg, int? TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegsInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("EditCivilWithoutLegsInstallationMast")]
        public async Task<IActionResult> EditCivilWithoutLegsInstallationMast([FromBody] EditCivilWithoutLegsInstallationObject CivilWithoutLeg, int? TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegsInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("EditCivilWithoutLegsInstallationMonople")]
        public async Task<IActionResult> EditCivilWithoutLegsInstallationMonople([FromBody] EditCivilWithoutLegsInstallationObject CivilWithoutLeg, int? TaskId)
        {
            try
            {
                if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegsInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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

        [HttpPost("EditCivilNonSteelInstallation")]
        public async Task<IActionResult> EditCivilNonSteelInstallation([FromBody] EditCivilNonSteelInstallationObject CivilNonSteel, int? TaskId)
        {
            try
            {
                if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelInstallationObject)))
                {
                    string authHeader = HttpContext.Request.Headers["Authorization"];



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
        [HttpPost("DismantleCivilWithoutLegsInstallationMonople")]
        public IActionResult DismantleCivilWithoutLegsInstallationMonople(string SiteCode, int CivilId, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.DismantleCivilWithoutLegsInstallation(userId, SiteCode, CivilId, TaskId, connectionString, null);
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

        [HttpPost("DismantleCivilWithoutLegsInstallationCapsule")]
        public IActionResult DismantleCivilWithoutLegsInstallationCapsule(string SiteCode, int CivilId, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.DismantleCivilWithoutLegsInstallation(userId, SiteCode, CivilId, TaskId, connectionString, null);
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

        [HttpPost("DismantleCivilWithoutLegsInstallationMast")]
        public IActionResult DismantleCivilWithoutLegsInstallationMast(string SiteCode, int CivilId, int? TaskId)
        {
            try
            {

                string authHeader = HttpContext.Request.Headers["Authorization"];



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
                    var response = _unitOfWorkService.InternalApiService.DismantleCivilWithoutLegsInstallation(userId, SiteCode, CivilId, TaskId, connectionString, null);
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

        [HttpPost("GetRadioAntennaLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetRadioAntennaLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetRadioAntennaLibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetRadioAntennaLibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetRadioRRULibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetRadioRRULibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetRadioOtherLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<GetEnableAttribute>))]
        public IActionResult GetRadioOtherLibraryEnabledAtt()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];



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
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetMWBULibrariesEnabledAtt(connectionString, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost("GetLoadsAndSideArmsForCivil")]
        [ProducesResponseType(200, Type = typeof(CheckLoadAndSideArmOnCivil))]
        public IActionResult GetLoadsAndSideArmsForCivil(int CivilId, string CivilType)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetLoadsAndSideArmsForCivil(CivilId, CivilType, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetLoadsAndSideArmsForCivil(CivilId, CivilType, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          
        }
        [HttpGet("GetRelationshipBetweenloads")]
        [ProducesResponseType(200, Type = typeof(CivilLoads))]
        public IActionResult GetRelationshipBetweenloads(int loadid, string Loadname, string SiteCode)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetRelationshipBetweenloads(loadid, Loadname, SiteCode, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetRelationshipBetweenloads(loadid, Loadname, SiteCode, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
           
        }
        [HttpGet("GetForAddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddGeneratorLibrary()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetForAddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          
        }
        [HttpGet("GetForAddSolarLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddSolarLibrary()

        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetForAddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetForAddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
         
        }
        [HttpPost("GeStaticAttsAndDynamicAttsByTableName")]
        [ProducesResponseType(200, Type = typeof(GetForAddDynamicAttribute))]
        public IActionResult GeStaticAttsAndDynamicAttsByTableName(String TabelName, bool IsLibrary, int? CategoryId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GeStaticAttsAndDynamicAttsByTableName(TabelName, IsLibrary, CategoryId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GeStaticAttsAndDynamicAttsByTableName(TabelName, IsLibrary, CategoryId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
           
        }
        [HttpPost("GetDynamicById")]
        [ProducesResponseType(200, Type = typeof(AddDynamicObject))]
        public IActionResult GetDynamicById(int DynamicAttributeId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetDynamicLibraryById(DynamicAttributeId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetDynamicLibraryById(DynamicAttributeId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
   
        }
        [HttpPost("GetDynamicAttsByTableName")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<DynamicAttViewModel>))]
        public IActionResult GetDynamicAttsByTableName([FromBody] List<FilterObjectList> filters, [FromQuery] ParameterPagination parameters, string TableName, int? CategoryId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetDynamicAttsByTableName(filters, parameters, TableName, CategoryId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetDynamicAttsByTableName(filters, parameters, TableName, CategoryId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
       
        }
        [HttpPost("GetLoadsOnSideArm")]
        [ProducesResponseType(200, Type = typeof(CivilLoads))]
        public IActionResult GetLoadsOnSideArm(int SideArmId)
        {

            string authHeader = HttpContext.Request.Headers["Authorization"];

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
                var response = _unitOfWorkService.InternalApiService.GetLoadsOnSideArm(SideArmId, userId, null);
                return Ok(response);
            }
            else if (authHeader.ToLower().StartsWith("basic "))
            {

                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':')[0];
                var password = decodedUsernamePassword.Split(':')[1];
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.GetLoadsOnSideArm(SideArmId, null, username);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
          
        }
        [HttpPost("getAllSites")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModelForGetAll>))]
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
    }
}