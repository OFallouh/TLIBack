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
        [HttpGet("GetAttForAdd")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.LoadOtherService.GetAttForAdd(LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddLoadOther")]
        [ProducesResponseType(200, Type = typeof(AddLoadOtherViewModel))]
        public IActionResult AddLoadOther([FromBody] AddLoadOtherViewModel addLoadOther, string SiteCode, int TaskId)
        {
            if (addLoadOther.TLIcivilLoads.sideArmId == 0)
                addLoadOther.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addLoadOther, nameof(AddLoadOtherViewModel)))
            {
                var response = _unitOfWorkService.LoadOtherService.AddLoadOther(addLoadOther, SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddLoadOtherViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditLoadOther")]
        [ProducesResponseType(200, Type = typeof(EditLoadOtherViewModel))]
        public async Task<IActionResult> EditLoadOther([FromBody] EditLoadOtherViewModel LoadOtherViewModel, int TaskId)
        {
            if (TryValidateModel(LoadOtherViewModel, nameof(EditLoadOtherViewModel)))
            {
                var response = await _unitOfWorkService.LoadOtherService.EditLoadOther(LoadOtherViewModel, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditLoadOtherViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleLoadOther")]

        public IActionResult DismantleLoadOther(string sitecode, int Id , int TaskId)
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
        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetById(int Id)
        {
            var response = _unitOfWorkService.LoadOtherService.GetById(Id);
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
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadOtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<object>))]
        public IActionResult GetLoadOtherOnSiteWithEnableAtt([FromBody] CombineFilters CombineFilters, [FromQuery] LoadsOnSiteFilter filters, bool WithFilterData, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.LoadOtherService.GetLoadOtherOnSiteWithEnableAtt(filters, WithFilterData, CombineFilters, parameters, CivilId, CivilType);
            return Ok(response);
        }
    }
}