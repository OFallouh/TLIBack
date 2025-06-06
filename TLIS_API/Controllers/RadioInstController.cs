﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RadioInstController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RadioInstController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        //[HttpGet("GetAttForAdd")]
        //[ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        //public IActionResult GetAttForAdd(string RadioInstType, int LibId, string SiteCode)
        //{
        //    var response = _unitOfWorkService.RadioInstService.GetAttForAdd(RadioInstType, LibId, SiteCode);
        //    return Ok(response);
        //}
      //  [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioAntennaInstallation(int LibId, string SiteCode)
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
            var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioAntennaInstallation(LibId, SiteCode, userId,false);
            return Ok(response);
         
        }
        //  [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioRRUInstallation(int LibId, string SiteCode)
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
            var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioRRUInstallation(LibId, SiteCode, userId,false);
            return Ok(response);
           
        }
        //  [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddRadioOtherInstallation(int LibId, string SiteCode)
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
            var response = _unitOfWorkService.RadioInstService.GetAttForAddRadioOtherInstallation(LibId, SiteCode, userId,false);
            return Ok(response);
           
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaInstallationObject))]
        public IActionResult AddRadioAntennaInstallation([FromBody] AddRadioAntennaInstallationObject addRadioAntenna, string SiteCode, int? TaskId)
        { 
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaInstallationObject)))
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
                var response = _unitOfWorkService.RadioInstService.AddRadioInstallation(addRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), SiteCode, ConnectionString , TaskId, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioAntennaViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherInstallationObject))]
        public IActionResult AddRadioOtherInstallation([FromBody] AddRadioOtherInstallationObject addRadioOtherInstallationObject, string SiteCode, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioOtherInstallationObject, nameof(AddRadioOtherInstallationObject)))
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
                var response = _unitOfWorkService.RadioInstService.AddRadioInstallation(addRadioOtherInstallationObject, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), SiteCode, ConnectionString, TaskId, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRUInstallationObject))]
        public IActionResult AddRadioRRUInstallation([FromBody] AddRadioRRUInstallationObject addRadioRRU, string SiteCode, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRUInstallationObject)))
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
                var response = _unitOfWorkService.RadioInstService.AddRadioInstallation(addRadioRRU, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), SiteCode, ConnectionString, TaskId, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioAntennaViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }
        // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetRadioAntennaInstallationById")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetRadioAntennaInstallationById(int RadioId)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioAntennaInstallationById(RadioId, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString());
            return Ok(response);
        }
        // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetRadioRRUInstallationById")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetRadioRRUInstallationById(int RadioId)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioRRUInstallationById(RadioId, Helpers.Constants.LoadSubType.TLIradioRRU.ToString());
            return Ok(response);
        }
        // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetRadioOtherInstallationById")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetRadioOtherInstallationById(int RadioId)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioOtherInstallationById(RadioId, Helpers.Constants.LoadSubType.TLIradioOther.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetById(int Id, string RadioType)
        {
            var response = _unitOfWorkService.RadioInstService.GetById(Id, RadioType);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleRadioRRU")]

        public IActionResult DismantleRadioRRU(string sitecode, int Id, int? TaskId)
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
            var response = _unitOfWorkService.RadioInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleRadioOther")]

        public IActionResult DismantleRadioOther(string sitecode, int Id ,int? TaskId)
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
            var response = _unitOfWorkService.RadioInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantleRadioAntenna")]
        public IActionResult DismatleRadioAntenna(string sitecode, int Id ,int? TaskId)
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
            var response = _unitOfWorkService.RadioInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(EditRadioAntennaInstallationObject))]
        public async Task<IActionResult> EditRadioAntennaInstallation([FromBody] EditRadioAntennaInstallationObject editRadioAntenna,int? TaskId)
        {
            if(TryValidateModel(editRadioAntenna, nameof(EditRadioAntennaInstallationObject)))
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
                var response = await _unitOfWorkService.RadioInstService.EditRadioInstallation(editRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), TaskId, userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRadioAntennaViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(EditRadioRRUInstallationObject))]
        public async Task<IActionResult> EditRadioRRUInstallation([FromBody] EditRadioRRUInstallationObject editRadioRRUInstallationObject, int? TaskId)
        {
            if (TryValidateModel(editRadioRRUInstallationObject, nameof(EditRadioRRUInstallationObject)))
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
                var response = await _unitOfWorkService.RadioInstService.EditRadioInstallation(editRadioRRUInstallationObject, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), TaskId, userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRadioAntennaViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(EditRadioOtherInstallationObject))]
        public async Task<IActionResult> EditRadioOtherInstallation([FromBody] EditRadioOtherInstallationObject editRadioOtherInstallationObject, int? TaskId)
        {
            if (TryValidateModel(editRadioOtherInstallationObject, nameof(EditRadioOtherInstallationObject)))
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
                var response = await _unitOfWorkService.RadioInstService.EditRadioInstallation(editRadioOtherInstallationObject, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), TaskId, userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRadioOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioRRUsList")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioRRUViewModel>))]
        public IActionResult GetRadioRRUsList([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioRRUsList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioAntennasList")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioAntennaViewModel>))]
        public IActionResult GetRadioAntennasList([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioAntennasList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioOtherList")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioOtherViewModel>))]
        public IActionResult GetRadioOtherList([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.RadioInstService.GetRadioOtherList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioAntennaInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioAntennaInstallationWithEnableAtt([FromQuery] string SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.RadioInstService.GetRadioAntennaInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioRRUInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioRRUInstallationWithEnableAtt([FromQuery] string SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.RadioInstService.GetRadioRRUInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetRadioOtherInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetRadioOtherInstallationWithEnableAtt([FromQuery] string SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.RadioInstService.GetRadioOtherInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}