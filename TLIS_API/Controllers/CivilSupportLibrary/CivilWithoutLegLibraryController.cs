using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
   // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilWithoutLegLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilWithoutLegLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CivilWithoutLegLibraryViewModel>))]
        public IActionResult GetCivilWithoutLegLibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilWithoutLegLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetCivilWithoutLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegLibrariesEnabledAtt([FromBody] CombineFilters ComineOutPut, bool WithFilterData, int CategoryId, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegLibrariesEnabledAtt(ComineOutPut, WithFilterData, CategoryId, parameters);
            return Ok(response);
        }
        [HttpGet("GetCivilWithoutLegsLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegLibraryViewModel))]
        public IActionResult GetCivilWithoutLegsLibraryById(int id)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegsLibraryById(id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);

        }
        [HttpPost("AddCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegsLibraryObject))]
        public IActionResult AddCivilWithoutLegLibrary([FromBody] AddCivilWithoutLegsLibraryObject addCivilWithoutLegLibraryViewModel)
        {
            if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegsLibraryObject)))
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
                var response = _unitOfWorkService.CivilLibraryService.AddCivilWithoutLegsLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, ConnectionString,userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithoutLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithoutLegsLibraryObject))]
        public async Task<IActionResult> EditCivilWithoutLegLibrary([FromBody] EditCivilWithoutLegsLibraryObject editCivilWithoutLegLibraryViewModel)
        {
            if (TryValidateModel(editCivilWithoutLegLibraryViewModel, nameof(EditCivilWithoutLegsLibraryObject)))
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
                var response = await _unitOfWorkService.CivilLibraryService.EditCivilWithoutlegsLibrary(editCivilWithoutLegLibraryViewModel, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCivilWithoutLegLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCivilWithoutLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegLibraryViewModel))]
        public async Task<IActionResult> DisableCivilWithoutLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Disable(Id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCivilWithoutLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteCivilWithoutLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Delete(Id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsMastLibrary")]
        public IActionResult GetForAddCivilWithoutLegsMastLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutMastLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsCapsuleLibrary")]
        public IActionResult GetForAddCivilWithoutLegsCapsuleLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutCapsuleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithoutLegsMonopleLibrary")]
        public IActionResult GetForAddCivilWithoutLegsMonopleLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAddCivilWithoutMonopleLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
    }
}
