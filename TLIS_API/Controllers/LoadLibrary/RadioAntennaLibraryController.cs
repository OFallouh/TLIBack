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
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RadioAntennaLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public RadioAntennaLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetRadioAntennaLibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<RadioAntennaLibraryViewModel>))]
        public IActionResult GetRadioAntennaLibraries([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> Filter = null)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioAntennaLibraries(parameterPagination, Filter);
            return Ok(response);
        }
        [HttpPost("GetRadioAntennaLibrariesWithEnabledAttribute")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetRadioAntennaLibrariesWithEnabledAttribute([FromBody] CombineFilters CombineFilters, [FromQuery]ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetRadioAntennaLibrariesWithEnabledAttribute(CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpGet("GetRadioAntennaLibraryById/{Id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetRadioAntennaLibraryById(int Id)
        {
            var response = _unitOfWorkService.RadioLibraryService.GetById(Id, Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaLibraryObject))]
        public IActionResult AddRadioAntennaLibrary(AddRadioAntennaLibraryObject addRadioAntenna)
        {
            if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaLibraryObject)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.RadioLibraryService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), addRadioAntenna, ConnectionString);
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
        //[HttpPost("UpdateRadioAntennaLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditRadioAntennaLibraryViewModel))]
        //public async Task<IActionResult> UpdateRadioAntennaLibrary(EditRadioAntennaLibraryViewModel editRadioAntenna)
        //{
        //    if (TryValidateModel(editRadioAntenna, nameof(EditRadioAntennaLibraryViewModel)))
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
        [HttpPost("DisableRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableRadioAntennaLibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DisableRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpGet("GetForAddRadioAntennLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddRadioAntennLibrary()
        {
            var response = _unitOfWorkService.RadioLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteRadioAntennaLibrary(int Id)
        {
            var response = await _unitOfWorkService.RadioLibraryService.DeletedRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), Id);
            return Ok(response);
        }
    }
}