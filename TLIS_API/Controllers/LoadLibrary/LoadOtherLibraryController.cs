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
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadOtherLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public LoadOtherLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetLoadOtherLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<LoadOtherLibraryViewModel>))]
        public IActionResult GetLoadOtherLibrariesWithEnableAtt()
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.LoadOtherLibraryService.GetLoadOtherLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        
        [HttpGet("GetLoadOtherLibraryById/{Id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetLoadOtherLibraryById(int Id)
        {
            var response = _unitOfWorkService.LoadOtherLibraryService.GetById(Id, Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddLoadOtherLibrary()
        {
            var response = _unitOfWorkService.LoadOtherLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddLoadOtherLibraryObject))]
        public IActionResult AddLoadOtherLibrary(AddLoadOtherLibraryObject addLoadOtherLibrary)
        {
            if (TryValidateModel(addLoadOtherLibrary, nameof(AddLoadOtherLibraryObject)))
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
                var response = _unitOfWorkService.LoadOtherLibraryService.AddLoadOtherLibrary(addLoadOtherLibrary, ConnectionString, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddLoadOtherLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(EditLoadOtherLibraryObject))]
        public async Task<IActionResult> EditLoadOtherLibrary(EditLoadOtherLibraryObject editLoadOther)
        {
            if (TryValidateModel(editLoadOther, nameof(EditLoadOtherLibraryObject)))
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
                var response = await _unitOfWorkService.LoadOtherLibraryService.EditLoadOtherLibrary(editLoadOther, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditLoadOtherLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableLoadOtherLibrary(int Id)
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
            var response = await _unitOfWorkService.LoadOtherLibraryService.DisableLoadOtherLibrary(Id, userId);
            return Ok(response);
        }
        [HttpPost("DeleteLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteLoadOtherLibrary(int Id)
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
            var response = await _unitOfWorkService.LoadOtherLibraryService.DeletedLoadOtherLibrary(Id, userId);
            return Ok(response);
        }
    }
}