using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.PowerLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.Load
{
  //  [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class PowerLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public PowerLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<PowerLibraryViewModel>))]
        public IActionResult GetPowerLibrary([FromQuery]ParameterPagination parameterPagination,[FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetPowerLibraries(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("GetPowerLibrariesWithEnableAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetPowerLibrariesWithEnableAttributes([FromBody] CombineFilters CombineFilters, [FromQuery]ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetPowerLibrariesWithEnableAttributes(CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetPowerLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetPowerLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.PowerLibraryService.GetPowerLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetForAddPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddPowerLibrary()
        {
            var response = _unitOfWorkService.PowerLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(PowerLibraryViewModel))]
        public IActionResult GetPowerLibrary(int id)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetById(id,Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString());
            return Ok(response);

        }

        [HttpPost("AddPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddPowerLibrary([FromBody]AddPowerLibraryObject addPowerLibraryViewModel)
        {
            if(TryValidateModel(addPowerLibraryViewModel, nameof(AddPowerLibraryObject)))
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
                var response = _unitOfWorkService.PowerLibraryService.AddPowerLibrary(userId, addPowerLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]

        public async Task<IActionResult> EditPowerLibrary([FromBody] EditPowerLibraryObject editPowerLibraryViewModel)
        {
            if (TryValidateModel(editPowerLibraryViewModel, nameof(EditPowerLibraryObject)))
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
                var response = await _unitOfWorkService.PowerLibraryService.EditPowerLibrary(userId, editPowerLibraryViewModel, Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisablePowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisablePowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.PowerLibraryService.DisablePowerLibrary(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpPost("DeletePowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeletePowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.PowerLibraryService.DeletePowerLibrary(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), Id);
            return Ok(response);
        }
    }
}