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
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RadioOtherLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RadioOtherLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetOtherRadioLibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioOtherLibraryViewModel>))]
        public IActionResult GetOtherRadioLibraries([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetOtherRadioLibraries(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("GetOtherRadioLibraryById/{Id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetOtherRadioLibraryById(int Id)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetById(Id, Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString());
            return Ok(response);
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
                var response = _unitOfWorkService.RadioLibraryService.AddRadioOtherLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), addRadioOther, ConnectionString, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetForAddRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddRadioOtherLibrary()
        {
            var response = _unitOfWorkService.RadioLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("EditRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(EditRadioOtherLibraryObject))]
        public async Task<IActionResult> EditRadioOtherLibrary(EditRadioOtherLibraryObject editRadioOther)
        {
            if (TryValidateModel(editRadioOther, nameof(EditRadioOtherLibraryObject)))
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
                var response = await _unitOfWorkService.RadioLibraryService.EditRadioOtherLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), editRadioOther, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRadioOtherLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableRadioOtherLibrary(int Id)
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
            var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id, userId, ConnectionString);
            return Ok(response);
        }
    
        [HttpPost("DeleteRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteRadioOtherLibrary(int Id)
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
            var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id, userId, ConnectionString);
            return Ok(response);
        }
    
        [HttpPost("GetRadioOtherLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetRadioOtherLibrariesEnabledAtt( )
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.RadioLibraryService.GetRadioOtherLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
    }
}