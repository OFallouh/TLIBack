using System;
using System.Collections.Generic;
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
        [ProducesResponseType(200, Type = typeof(AddRadioOtherLibraryViewModel))]
        public IActionResult AddRadioOtherLibrary(AddRadioOtherLibraryViewModel addRadioOther)
        {
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), addRadioOther, ConnectionString);
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
        //[HttpPost("UpdateRadioOtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioOtherLibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioOtherLibrary(EditRadioOtherLibraryViewModel editRadioOther)
        //{
        //    if (TryValidateModel(editRadioOther, nameof(EditRadioOtherLibraryViewModel)))
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
        [HttpPost("DisableRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableRadioOtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpPost("DeleteRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteRadioOtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpPost("GetRadioOtherLibrariesWithEnabledAttribute")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetRadioRRULibrariesWithEnabledAttribute([FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioOtherLibrariesWithEnabledAttribute(CombineFilters, parameterPagination);
            return Ok(response);
        }
    }
}