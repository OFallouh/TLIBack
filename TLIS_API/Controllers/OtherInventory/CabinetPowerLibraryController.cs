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
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
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
        [HttpPost("GetCabinetPowerLibraries")]
        [ProducesResponseType(200, Type = typeof(List<CabinetPowerLibraryViewModel>))]
        public IActionResult GetCabinetPowerLibraries([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetCabinetPowerLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetCabinetPowerLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCabinetPowerLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters, bool? isRefresh)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetCabinetPowerLibraryEnabledAtt(CombineFilters, WithFilterData, parameters, isRefresh);
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
        public IActionResult AddCabinetPowerLibrary([FromBody]AddCabinetPowerLibraryViewModel addCabinetPowerLibrary)
        {
            if (TryValidateModel(addCabinetPowerLibrary, nameof(AddCabinetPowerLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString(), addCabinetPowerLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateCabinetPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateCabinetPowerLibrary([FromBody]EditCabinetPowerLibraryViewModel editCabinetPowerLibrary)
        {
            if (TryValidateModel(editCabinetPowerLibrary, nameof(EditCabinetPowerLibraryViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryLibraryService.EditOtherInventoryLibrary(editCabinetPowerLibrary, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCabinetPowerLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableCabinetPowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCabinetPowerLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteCabinetPowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString());
            return Ok(response);
        }
    }
}