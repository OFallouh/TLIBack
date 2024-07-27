using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadOtherController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public LoadOtherController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddLoadOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddLoadOtherInstallation(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.LoadOtherService.GetAttForAddLoadOtherInstallation(LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddLoadOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddLoadOtherInstallationObject))]
        public IActionResult AddLoadOther([FromBody] AddLoadOtherInstallationObject addLoadOther, string SiteCode, int? TaskId)
        {
            if (TryValidateModel(addLoadOther, nameof(AddLoadOtherInstallationObject)))
            {
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
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.LoadOtherService.AddLoadOther(addLoadOther, SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddLoadOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditLoadOther")]
        [ProducesResponseType(200, Type = typeof(EditLoadOtherInstallationObject))]
        public async Task<IActionResult> EditLoadOther([FromBody] EditLoadOtherInstallationObject LoadOtherViewModel, int? TaskId)
        {
            if (TryValidateModel(LoadOtherViewModel, nameof(EditLoadOtherInstallationObject)))
            {
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
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = await _unitOfWorkService.LoadOtherService.EditLoadOtherInstallation(LoadOtherViewModel, TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditLoadOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleLoadOther")]

        public IActionResult DismantleLoadOther(string sitecode, int Id , int? TaskId)
        {
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
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.LoadOtherService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIloadOther.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetLoadOtherInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetById(int Id)
        {
            var response = _unitOfWorkService.LoadOtherService.GetLoadOtherInstallationById(Id, Helpers.Constants.LoadSubType.TLIloadOther.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadOtherList")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<LoadOtherViewModel>))]
        public IActionResult GetLoadOtherList([FromBody] List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.LoadOtherService.GetLoadOtherList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        //[HttpPost("GetLoadsOtherBySite")]
        //[ProducesResponseType(200, Type = typeof(ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>))]
        //public IActionResult GetLoadsOtherBySite([FromBody] LoadsOnSiteFilter filters, bool WithFilterData, List<FilterObjectList> ObjectAttributeFilters, [FromQuery] ParameterPagination parameters)
        //{
        //    var response = _unitOfWorkService.LoadOtherService.GetLoadsOtherBySite(filters, WithFilterData, ObjectAttributeFilters, parameters);
        //    return Ok(response);
        //}
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadOtherInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<object>))]
        public IActionResult GetLoadOtherInstallationWithEnableAtt([FromQuery] string SiteCode)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.LoadOtherService.GetLoadOtherInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}