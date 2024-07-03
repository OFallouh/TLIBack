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
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratorLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public GeneratorLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetGeneratorLibraries")]
        [ProducesResponseType(200, Type = typeof(List<GeneratorLibraryViewModel>))]
        public IActionResult GetGeneratorLibraries([FromBody] List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetGeneratorLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetGeneratorLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetGeneratorLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetGeneratorLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetGeneratorLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetGeneratorLibraryById(int id)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetById(id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddGeneratorLibrary([FromBody]AddGeneratorLibraryObject addGeneratorLibrary)
        {
            if (TryValidateModel(addGeneratorLibrary, nameof(AddGeneratorLibraryObject)))
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
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddGenertatoLibrary(userId, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), addGeneratorLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGeneratorLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> EditGeneratorLibrary([FromBody] EditGeneratorLibraryObject editGeneratorLibrary)
        {
            if (TryValidateModel(editGeneratorLibrary, nameof(EditGeneratorLibraryViewModel)))

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
                var response = await _unitOfWorkService.OtherInventoryLibraryService.EditGeneratorLibrary(userId,editGeneratorLibrary, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditGeneratorLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableGeneratorLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableGeneratorLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteGeneratorLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteGeneratorLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddPGetForAddGeneratorLibraryowerLibrary()
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetForAdd(Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
    }
}