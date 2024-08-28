using System;
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
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
   // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RadioRRULibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RadioRRULibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetRadioRRULibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<GetForAddCivilLibrarybject>))]
        public IActionResult GetRadioRRULibraries([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioRRULibraries(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("GetForAddRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddRadioRRULibrary()
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
                var response = _unitOfWorkService.RadioLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString());
                return Ok(response);
            }
            
            else
            {
                return Unauthorized();
            }
           
        }
        [HttpPost("GetRadioRRULibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetRadioRRULibrariesEnabledAtt( )
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.RadioLibraryService.GetRadioRRULibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetRadioRRULibraryById/{Id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetRadioRRULibraryById(int Id)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetById(Id, Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString());
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
                var response = _unitOfWorkService.RadioLibraryService.AddRadioRRULibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, ConnectionString, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioRRULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(EditRadioRRULibraryObject))]
        public async Task<IActionResult> EditRadioRRULibrary(EditRadioRRULibraryObject editRadioRRU)
        {
            if (TryValidateModel(editRadioRRU, nameof(EditRadioRRULibraryObject)))
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
                var response = await _unitOfWorkService.RadioLibraryService.EditRadioRRULibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), editRadioRRU, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditRadioRRULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableRadioRRULibrary(int Id)
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
            var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id, userId, ConnectionString); return Ok(response);
        }
        [HttpPost("DeleteRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteRadioRRULibrary(int Id)
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
            var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id, userId, ConnectionString);
            return Ok(response);
        
        }
    }
}