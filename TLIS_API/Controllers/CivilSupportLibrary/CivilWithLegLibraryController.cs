using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.CivilLibraryService;

namespace TLIS_API.Controllers
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilWithLegLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilWithLegLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<CivilWithLegLibraryViewModel>))]
        public IActionResult getCivilWithLegLibraries([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilWithLegLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }

        
        [HttpPost("GetCivilWithLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithLegLibrariesEnabledAtt( ConnectionString);
            return Ok(response);
        }
        [HttpGet("getCivilWithLegsLibraryById")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public  IActionResult GetCivilWithLegLibrary(int id)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithLegsLibraryById(id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);

        }
        [HttpPost("AddCivilWithLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsLibraryObject))]
        public IActionResult AddCivilWithLegLibrary([FromBody] AddCivilWithLegsLibraryObject CivilWithLegLibraryViewModel, [FromServices] IConfiguration configuration)
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
            var userId= Convert.ToInt32(userInfo);
            var ConnectionString = configuration["ConnectionStrings:ActiveConnection"];

            if (!ModelState.IsValid)
            {
                var ErrorMessages = ModelState.Values.SelectMany(state => state.Errors.Select(error => error.ErrorMessage));
                return BadRequest(new Response<AddCivilWithoutLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

            var response = _unitOfWorkService.CivilLibraryService.AddCivilWithLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibraryViewModel, ConnectionString,userId);
            return Ok(response);
        }
        [HttpPost("EditCivilWithLegLibrary")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithLegsLibraryObject))]

        public async Task<IActionResult> EditCivilWithLegLibrary([FromBody] EditCivilWithLegsLibraryObject editCivilWithLegLibraryViewModel)
        {
            if (TryValidateModel(editCivilWithLegLibraryViewModel, nameof(EditCivilWithLegsLibraryObject)))
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
                var response = await _unitOfWorkService.CivilLibraryService.EditCivilWithLegsLibrary(editCivilWithLegLibraryViewModel, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCivilWithLegLibraryViewModels>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCivilWithLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DisableCivilWithLegLibrary(int Id)
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
            var response = await _unitOfWorkService.CivilLibraryService.Disable(Id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), userId);
            return Ok(response);
        }
        [HttpPost("DeleteCivilWithLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteCivilWithLegLibrary(int Id)
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
            var response = await _unitOfWorkService.CivilLibraryService.Delete(Id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), userId);
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithLegsLibrary")]
        public IActionResult GetForAddCivilWithLegsLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);
        }
    }
}
