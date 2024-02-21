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
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CabinetTelecomLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CabinetTelecomLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetCabinetTelecomLibraries")]
        [ProducesResponseType(200, Type = typeof(List<CabinetTelecomLibraryViewModel>))]
        public IActionResult GetCabinetTelecomLibraries([FromBody]List<FilterObjectList> filters, [FromQuery]bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetCabinetTelecomLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetCabinetTelecomLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCabinetTelecomLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters, bool? isRefresh)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetCabinetTelecomLibraryEnabledAtt(CombineFilters, WithFilterData, parameters, isRefresh);
            return Ok(response);
        }
        [HttpGet("GetCabinetTelecomLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetCabinetTelecomLibraryById(int id)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetById(id, Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddCabinetTelecomLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCabinetTelecomLibrary([FromBody]AddCabinetTelecomLibraryViewModel addCabinetTelecomLibrary)
        {
            if (TryValidateModel(addCabinetTelecomLibrary, nameof(AddCabinetTelecomLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString(), addCabinetTelecomLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCabinetTelecomLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateCabinetTelecomLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateCabinetTelecomLibrary([FromBody]EditCabinetTelecomLibraryViewModel editCabinetTelecomLibrary)
        {
            if (TryValidateModel(editCabinetTelecomLibrary, nameof(EditCabinetTelecomLibraryViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryLibraryService.EditOtherInventoryLibrary(editCabinetTelecomLibrary, Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetTelecomLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCabinetTelecomLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableCabinetTelecomLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCabinetTelecomLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteCabinetTelecomLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString());
            return Ok(response);
        }
    }
}