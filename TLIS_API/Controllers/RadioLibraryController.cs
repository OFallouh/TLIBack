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
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RadioLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RadioLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
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
        [HttpPost("GetRadioAntennaLibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioAntennaLibraryViewModel>))]
        public IActionResult GetRadioAntennaLibraries([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> Filter = null)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioAntennaLibraries(parameterPagination, Filter);
            return Ok(response);
        }
        [HttpPost("GetRadioRRULibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioRRULibraryViewModel>))]
        public IActionResult GetRadioRRULibraries([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioRRULibraries(parameterPagination, filters);
            return Ok(response);
        }
        //[HttpPost("AddRadioOtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(AddRadioOtherLibraryViewModel))]
        //public IActionResult AddRadioOtherLibrary(AddRadioOtherLibraryViewModel addRadioOther)
        //{
        //    if(TryValidateModel(addRadioOther, nameof(AddRadioOtherLibraryViewModel)))
        //    {
        //        var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //        var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), addRadioOther, ConnectionString);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<AddRadioOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        [HttpPost("AddRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaLibraryObject))]
        public IActionResult AddRadioAntennaLibrary(AddRadioAntennaLibraryObject addRadioAntenna)
        {
            if(TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaLibraryObject)))
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
                var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), addRadioAntenna, ConnectionString,userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioAntennaLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[HttpPost("AddRadioRRULibrary")]
        //[ProducesResponseType(200, Type = typeof(AddRadioRRULibraryViewModel))]
        //public IActionResult AddRadioRRULibrary(AddRadioRRULibraryViewModel addRadioRRU)
        //{
        //    if(TryValidateModel(addRadioRRU, nameof(AddRadioRRULibraryViewModel)))
        //    {
        //        var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //        var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, ConnectionString);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<AddRadioRRULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        //[HttpPost("UpdateRadioOtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioOtherLibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioOtherLibrary(EditRadioOtherLibraryViewModel editRadioOther)
        //{
        //    if(TryValidateModel(editRadioOther, nameof(EditRadioOtherLibraryViewModel)))
        //    {
        //        var response = await _unitOfWorkService.RadioLibraryService.EditRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), editRadioOther);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditRadioOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        //[HttpPost("UpdateRadioAntennaLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioAntennaLibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioAntennaLibrary(EditRadioAntennaLibraryViewModel editRadioAntenna)
        //{
        //    if(TryValidateModel(editRadioAntenna, nameof(EditRadioAntennaLibraryViewModel)))
        //    {
        //        var response = await _unitOfWorkService.RadioLibraryService.EditRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), editRadioAntenna);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditRadioAntennaLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        //[HttpPost("UpdateRadioRRULibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioRRULibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioRRULibrary(EditRadioRRULibraryViewModel editRadioRRU)
        //{
        //    if(TryValidateModel(editRadioRRU, nameof(EditRadioRRULibraryViewModel)))
        //    {
        //        var response = await _unitOfWorkService.RadioLibraryService.EditRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), editRadioRRU);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditRadioRRULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}



        //[HttpPost("DisableRadioOtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DisableRadioOtherLibrary(int Id)
        //{
        //    var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id);
        //    return Ok(response);
        //}
        //[HttpPost("DisableRadioAntennaLibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DisableRadioAntennaLibrary(int Id)
        //{
        //    var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), Id);
        //    return Ok(response);
        //}
        //[HttpPost("DisableRadioRRULibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DisableRadioRRULibrary(int Id)
        //{
        //    var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id);
        //    return Ok(response);
        //}
        //[HttpGet("GetForAdd/{RadioType}")]
        //[ProducesResponseType(200, Type = typeof(List<AllItemAttributes>))]
        //public IActionResult GetForAdd(string RadioType)
        //{
        //    var response = _unitOfWorkService.RadioLibraryService.GetForAdd(RadioType);
        //    return Ok(response);
        //}
        //[HttpPost("DeleteRadioOtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DeleteRadioOtherLibrary(int Id)
        //{
        //    string authHeader = HttpContext.Request.Headers["Authorization"];

        //    if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
        //    {
        //        return Unauthorized();
        //    }

        //    var token = authHeader.Substring("Bearer ".Length).Trim();
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        //    if (jsonToken == null)
        //    {
        //        return Unauthorized();
        //    }

        //    string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
        //    var userId = Convert.ToInt32(userInfo);
        //    var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id, userId);
        //    return Ok(response);
        //}
        //[HttpPost("DeleteRadioAntennaLibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DeleteRadioAntennaLibrary(int Id)
        //{
        //    string authHeader = HttpContext.Request.Headers["Authorization"];

        //    if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
        //    {
        //        return Unauthorized();
        //    }

        //    var token = authHeader.Substring("Bearer ".Length).Trim();
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        //    if (jsonToken == null)
        //    {
        //        return Unauthorized();
        //    }

        //    string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
        //    var userId = Convert.ToInt32(userInfo);
        //    var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), Id, userId,connectionString);
        //    return Ok(response);
        //}
        //[HttpPost("DeleteRadioRRULibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DeleteRadioRRULibrary(int Id)
        //{
        //    string authHeader = HttpContext.Request.Headers["Authorization"];

        //    if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
        //    {
        //        return Unauthorized();
        //    }

        //    var token = authHeader.Substring("Bearer ".Length).Trim();
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        //    if (jsonToken == null)
        //    {
        //        return Unauthorized();
        //    }

        //    string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
        //    var userId = Convert.ToInt32(userInfo);
        //    var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id, userId);
        //    return Ok(response);
        //}
    }
}