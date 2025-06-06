﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
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
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCabinetPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCabinetPowerInstallation(int CabinetPowerLibraryId, string SiteCode)
        {
           
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddCabinetPowerInstallation(Helpers.Constants.TablesNames.TLIcabinetPower.ToString(), CabinetPowerLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCabinetTelecomInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCabinetTelecomInstallation(int CabinetTelecomLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddCabinetTelecomInstallation(Helpers.Constants.TablesNames.TLIcabinetTelecom.ToString(), CabinetTelecomLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddSolarInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddSolarInstallation( int SolarLibraryId, string SiteCode)

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
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddSolarInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SolarLibraryId, SiteCode,userId,false);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddGeneratorInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddGeneratorInstallation(int GeneratorIdLibraryId, string SiteCode)
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
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAddGeneratorInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), GeneratorIdLibraryId, SiteCode, userId, false);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddCabinetPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(AddCabinetPowerInstallation))]
        public IActionResult AddCabinetPowerInstallation([FromBody] AddCabinetPowerInstallation addCabinetViewModel, string SiteCode, int? TaskId)
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

            if (TryValidateModel(addCabinetViewModel, nameof(AddCabinetPowerInstallation)))
            {
                var response = _unitOfWorkService.OtherInventoryInstService.AddCabinetPowerInstallation(addCabinetViewModel, SiteCode, ConnectionString, TaskId, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetPowerInstallation>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddCabinetTelecomInstallation")]
        [ProducesResponseType(200, Type = typeof(AddCabinetTelecomInstallationObject))]
        public IActionResult AddCabinetTelecomInstallation([FromBody] AddCabinetTelecomInstallationObject addCabinetViewModel, string SiteCode, int? TaskId)
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

            if (TryValidateModel(addCabinetViewModel, nameof(AddCabinetTelecomInstallationObject)))
            {
                var response = _unitOfWorkService.OtherInventoryInstService.AddCabinetTelecomInstallation(addCabinetViewModel, SiteCode, ConnectionString, TaskId, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetTelecomInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
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
                var response = _unitOfWorkService.OtherInventoryInstService.AddSolarInstallation(addSolarViewModel, SiteCode, ConnectionString, TaskId, userId,false);
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
                var response = _unitOfWorkService.OtherInventoryInstService.AddGeneratorInstallation(addGeneratorViewModel, SiteCode, ConnectionString, TaskId, userId, false);
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
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCabinetTelecomInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCabinetTelecomInstallationById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetTelecomInstallationById(OtherInventoryInstId, Helpers.Constants.TablesNames.TLIcabinetTelecom.ToString());
            return Ok(response);

        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCabinetPowerInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCabinetPowerInstallationById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetPowerInstallationById(OtherInventoryInstId, Helpers.Constants.TablesNames.TLIcabinetPower.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetSolarInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetSolarInstallationById(int SolarId)
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
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarInstallationById(SolarId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), userId,false);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetGenertorInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetGenertorInstallationById(int GeneratorId)
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
            var response = _unitOfWorkService.OtherInventoryInstService.GetGenertorInstallationById(GeneratorId, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), userId, false);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCabinetPowerInstallation")]
        [ProducesResponseType(200, Type = typeof(EditCabinetPowerInstallationObject))]
        public async Task<IActionResult> EditCabinetPowerInstallation([FromBody] EditCabinetPowerInstallationObject editCabinetViewModel, int? TaskId)
        {
            if (TryValidateModel(editCabinetViewModel, nameof(EditCabinetPowerInstallationObject)))
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditCabinetPowerInstallation(editCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), TaskId, userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetPowerInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCabinetTelecomInstallation")]
        [ProducesResponseType(200, Type = typeof(EditCabinetTelecomInstallationObject))]
        public async Task<IActionResult> EditCabinetTelecomInstallation([FromBody] EditCabinetTelecomInstallationObject editCabinetViewModel, int? TaskId)
        {
            if (TryValidateModel(editCabinetViewModel, nameof(EditCabinetTelecomInstallationObject)))
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditCabinetTelecomInstallation(editCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), TaskId, userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetTelecomInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId, userId, ConnectionString,false);
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
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), TaskId, userId, ConnectionString,false);
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
            var response = _unitOfWorkService.OtherInventoryInstService.DismantleOtherInventory(userId,SiteCode, OtherInventoryId , OtherInventoryName, TaskId, ConnectionString,false);
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
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCabinetPowerWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCabinetPowerWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetPowerWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCabinetTelecomWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCabinetTelecomWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetTelecomWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}