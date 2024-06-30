using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_API.Middleware.WorkFlow;
using System.IdentityModel.Tokens.Jwt;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public PowerController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.PowerService.GetAttForAddPowerInstallation(LibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult AddPower([FromBody] AddPowerInstallationObject Power, string SiteCode, int? TaskId)
        {
         
            if (TryValidateModel(Power, nameof(AddPowerInstallationObject)))
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
                var response = _unitOfWorkService.PowerService.AddPowerInstallation(Power, SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ObjectInstAtts>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantlePower")]

        public IActionResult DismantlePower(string sitecode, int Id , int? TaskId)
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
            var response = _unitOfWorkService.PowerService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIpower.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(EditPowerInstallationOject))]
        public async Task<IActionResult> EditPowerInstallation([FromBody] EditPowerInstallationOject Power,int? TaskId)
        {
            if (TryValidateModel(Power, nameof(EditPowerInstallationOject)))
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
                var response = await _unitOfWorkService.PowerService.EditPowerInstallation(Power, TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditPowerViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetPowerInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetPowerInstallationById(int Id)
        {
            var response = _unitOfWorkService.PowerService.GetPowerInstallationById(Id);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetList")]
        [ProducesResponseType(200, Type = typeof(List<PowerViewModel>))]
        public IActionResult GetList([FromBody] List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.PowerService.GetList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetPowerTypes")]
        [ProducesResponseType(200, Type = typeof(List<PowerTypeViewModel>))]
        public IActionResult GetPowerTypes()
        {
            var response = _unitOfWorkService.PowerService.GetPowerTypes();
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetPowerInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetPowerInstallationWithEnableAtt([FromQuery] string SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.PowerService.GetPowerInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}