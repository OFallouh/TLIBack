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
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
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
            var response = _unitOfWorkService.RadioLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString());
            return Ok(response);
        }
        [HttpPost("GetRadioRRULibrariesWithEnabledAttribute")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetRadioRRULibrariesWithEnabledAttribute([FromBody] CombineFilters CombineFilters, [FromQuery]ParameterPagination parameterPagination )
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioRRULibrariesWithEnabledAttribute(CombineFilters, parameterPagination);
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
        [ProducesResponseType(200, Type = typeof(AddRadioRRULibraryViewModel))]
        public IActionResult AddRadioRRULibrary(AddRadioRRULibraryViewModel addRadioRRU)
        {
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, ConnectionString);
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
        //[HttpPost("UpdateRadioRRULibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioRRULibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioRRULibrary(EditRadioRRULibraryViewModel editRadioRRU)
        //{
        //    if (TryValidateModel(editRadioRRU, nameof(EditRadioRRULibraryViewModel)))
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
        [HttpPost("DisableRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableRadioRRULibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpPost("DeleteRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteRadioRRULibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), Id);
            return Ok(response);
        }
    }
}