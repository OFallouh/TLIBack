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
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CabinetPowerLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CabinetPowerLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GeneratorLibrarySeedDataForTest")]
        public IActionResult GeneratorLibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GeneratorLibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("SolarLibrarySeedDataForTest")]
        public IActionResult SolarLibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.SolarLibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpPost("RadioRRULibrarySeedDataForTest")]
        public IActionResult RadioRRULibrarySeedDataForTest()
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.RadioRRULibrarySeedDataForTest();
            return Ok(response);
        }
        [HttpGet("GetForAddCabinetPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddPGetForAddGeneratorLibraryowerLibrary()
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetForAdd(Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("GetCabinetPowerLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCabinetPowerLibrariesEnabledAtt()
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetCabinetPowerLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetCabinetPowerLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetCabinetPowerLibraryById(int id)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetById(id, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddCabinetPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCabinetPowerLibrary([FromBody]AddCabinetPowerLibraryObject addCabinetPowerLibrary)
        {
            if (TryValidateModel(addCabinetPowerLibrary, nameof(AddCabinetPowerLibraryObject)))
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
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddCabinetPowerLibrary(userId,Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString(), addCabinetPowerLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetPowerLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCabinetPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> EditCabinetPowerLibrary([FromBody] EditCabinetPowerLibraryObject editCabinetPowerLibrary)
        {
            if (TryValidateModel(editCabinetPowerLibrary, nameof(EditCabinetPowerLibraryObject)))
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
                var response = await _unitOfWorkService.OtherInventoryLibraryService.EditCabinetPowerLibrary(userId, editCabinetPowerLibrary, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString(), ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetPowerLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCabinetPowerLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableCabinetPowerLibrary(int Id)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString(), ConnectionString);
            return Ok(response);
        }
        [HttpPost("DeleteCabinetPowerLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteCabinetPowerLibrary(int Id)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString(), ConnectionString);
            return Ok(response);
        }
    }
}