using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadOtherLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public LoadOtherLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetLoadOtherLibraries")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<LoadOtherLibraryViewModel>))]
        public IActionResult GetLoadOtherLibraries([FromQuery]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = _unitOfWorkService.LoadOtherLibraryService.GetLoadOtherLibraries(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("GetLoadOtherLibrariesWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<object>))]
        public IActionResult GetLoadOtherLibrariesWithEnableAtt([FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, bool? isRefresh)
        {
            var response = _unitOfWorkService.LoadOtherLibraryService.GetLoadOtherLibrariesWithEnableAtt(CombineFilters, parameterPagination, isRefresh);
            return Ok(response);
        }
        [HttpGet("GetLoadOtherLibraryById/{Id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetLoadOtherLibraryById(int Id)
        {
            var response = _unitOfWorkService.LoadOtherLibraryService.GetById(Id);
            return Ok(response);
        }
        [HttpPost("AddLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddLoadOtherLibraryViewModel))]
        public IActionResult AddLoadOtherLibrary(AddLoadOtherLibraryViewModel addLoadOtherLibrary)
        {
            if (TryValidateModel(addLoadOtherLibrary, nameof(AddLoadOtherLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.LoadOtherLibraryService.AddLoadOtherLibrary(addLoadOtherLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddLoadOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(EditLoadOtherLibraryViewModel))]
        public async Task<IActionResult> UpdateLoadOtherLibrary(EditLoadOtherLibraryViewModel editLoadOther)
        {
            if (TryValidateModel(editLoadOther, nameof(EditLoadOtherLibraryViewModel)))
            {
                var response = await _unitOfWorkService.LoadOtherLibraryService.EditLoadOtherLibrary(editLoadOther);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditLoadOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableLoadOtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.LoadOtherLibraryService.DisableLoadOtherLibrary(Id);
            return Ok(response);
        }
        [HttpPost("DeleteLoadOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteLoadOtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.LoadOtherLibraryService.DeletedLoadOtherLibrary(Id);
            return Ok(response);
        }
    }
}