using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
   
    public class MW_BULibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MW_BULibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("MW_BULibrarySeedDataForTest")]
        public IActionResult MW_BULibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.MW_BULibraryService.MW_BULibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("MW_OtherLibrarySeedDataForTest")]
        public IActionResult MW_OtherLibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.MWLibraryService.MW_OtherLibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("MW_ODULibrarySeedDataForTest")]
        public IActionResult MW_ODULibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.MWLibraryService.MW_ODULibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("MW_DishLibrarySeedDataForTest")]
        public IActionResult MW_DishLibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.MWLibraryService.MW_DishLibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<MW_BULibraryViewModel>))]
        public IActionResult GetMW_BULibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.get_MW_BU_LibrariesAsync(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetMW_BULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_BULibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.GetMW_BULibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetPowerLibrary(int id)
        {
            var response = _unitOfWorkService.MWLibraryService.GetById(id,Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
            return Ok(response);
        }

        [HttpGet("GetForAddMWBUibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWBUibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddMW_BULibrary")]
        [ProducesResponseType(200, Type = typeof(GetForAddCivilLibrarybject))]
        public IActionResult AddMW_BULibrary([FromBody] AddMWBULibraryObject addMW_BULibraryViewModel)
        {
            if (TryValidateModel(addMW_BULibraryViewModel, nameof(EditCivilWithLegsLibraryObject)))
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
                var response = _unitOfWorkService.MWLibraryService.AddMWLibrary(userId, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), addMW_BULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_BULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditMW_BULibrary")]
        [ProducesResponseType(200, Type = typeof(EditMWBULibraryObject))]
        public async Task<IActionResult> EditMW_BULibrary([FromBody] EditMWBULibraryObject  editMWBULibraryObject)
        {
            if(TryValidateModel(editMWBULibraryObject, nameof(EditCivilWithLegsLibraryObject)))
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
                var response = await _unitOfWorkService.MWLibraryService.EditMWBULibrary(userId, editMWBULibraryObject, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_BULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DisableMW_BULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(EditMW_BULibraryViewModel))]
        public async Task<IActionResult> DisableMW_BULibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Disable(Id, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteMW_BULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(EditMW_BULibraryViewModel))]
        public async Task<IActionResult> DeleteMW_BULibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Delete(Id, Helpers.Constants.LoadSubType.TLImwBULibrary.ToString());
            return Ok(response);
        }

    }
}