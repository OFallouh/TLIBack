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
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class OtherInventoryInstController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public OtherInventoryInstController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCabinet")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCabinet(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAdd(Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddSolarInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddSolarInstallation( int SolarLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddSolarInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SolarLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddGeneratorInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddGeneratorInstallation(int GeneratorIdLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddGeneratorInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), GeneratorIdLibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddCabinet")]
        [ProducesResponseType(200, Type = typeof(AddCabinetViewModel))]
        public IActionResult AddCabinet([FromBody] AddCabinetViewModel addCabinetViewModel, string SiteCode, int? TaskId)
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

            if (TryValidateModel(addCabinetViewModel, nameof(AddCabinetViewModel)))
            {
                var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        
                

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddSolarInstallation")]
        [ProducesResponseType(200, Type = typeof(AddSolarInstallationObject))]
        public IActionResult AddSolarInstallation([FromBody] AddSolarInstallationObject addSolarViewModel, string SiteCode, int? TaskId)
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

            if (TryValidateModel(addSolarViewModel, nameof(AddSolarInstallationObject)))
            {
                var response = _unitOfWorkService.OtherInventoryInstService.AddSolarInstallation(addSolarViewModel, SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddSolarInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddGeneratorInstallation")]
        [ProducesResponseType(200, Type = typeof(AddGeneratorInstallationObject))]
        public IActionResult AddGeneratorInstallation([FromBody] AddGeneratorInstallationObject addGeneratorViewModel, string SiteCode, int ?TaskId)
        {
            if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorInstallationObject)))
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
                var response = _unitOfWorkService.OtherInventoryInstService.AddGeneratorInstallation(addGeneratorViewModel, SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGeneratorViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
             

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCabinetById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCabinetById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetById(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSolarInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetSolarInstallationById(int SolarId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarInstallationById(SolarId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetGenertorInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetGenertorInstallationById(int GeneratorId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetGenertorInstallationById(GeneratorId, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        //[HttpPost("EditCabinet")]
        //[ProducesResponseType(200, Type = typeof(EditCabinetViewModel))]
        //public async Task<IActionResult> EditCabinet([FromBody] EditCabinetViewModel editCabinetViewModel,int ?TaskId)
        //{
        //    if (TryValidateModel(editCabinetViewModel, nameof(EditCabinetViewModel)))
        //    {
        //        var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), TaskId);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditCabinetViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditSolarInstallation")]
        [ProducesResponseType(200, Type = typeof(EditSolarInstallationObject))]
        public async Task<IActionResult> EditSolarInstallation([FromBody] EditSolarInstallationObject editSolarViewModel, int? TaskId)
        {
            if (TryValidateModel(editSolarViewModel, nameof(EditSolarInstallationObject)))
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditSolarInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditGeneratorInstallation")]
        [ProducesResponseType(200, Type = typeof(EditGeneratorInstallationObject))]
        public async Task<IActionResult>EditGeneratorInstallation([FromBody] EditGeneratorInstallationObject editGeneratorViewModel,int ?TaskId)
        {
            if (TryValidateModel(editGeneratorViewModel, nameof(EditGeneratorInstallationObject)))
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditGeneratorInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleOtherInventory")]

        public IActionResult DismantleOtherInventory(string SiteCode, int OtherInventoryId, string OtherInventoryName,int ?TaskId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.DismantleOtherInventory(SiteCode, OtherInventoryId , OtherInventoryName, TaskId);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCabinetBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCabinetBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, LibraryType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSolarBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetSolarBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetGeneratorWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetGeneratorWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetGeneratorWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetSolarWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetSolarWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}